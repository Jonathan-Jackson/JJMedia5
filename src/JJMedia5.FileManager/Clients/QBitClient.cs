using JJMedia5.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Clients {

    public class QBitClient : ClientBase, ITorrentClient {

        // hold the previous ssid we got,
        // as QBittorrent won't send a new one
        // if we're already authenticated.
        // We still request on every request,
        // because we want to make sure QBit hasn't closed
        // and cleared out our authentication.
        private string _lastSsid;

        public QBitClient(HttpClient client, BasicAuthEndPoint auth)
            : base(client, auth) {
        }

        private async Task<string> GetSSID() {
            using (var authToPass = new HttpRequestMessage(HttpMethod.Post, $"{_address}/api/v2/auth/login")) {
                string body = $"username={_userName}&password={_password}";
                authToPass.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                authToPass.Content.Headers.Add("Content-Length", body.Length.ToString());
                authToPass.Headers.Add("Referer", _address);

                var response = await _client.SendAsync(authToPass);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to authenticate to QBittorrent: {response.ReasonPhrase}");

                _lastSsid = response.Headers.FirstOrDefault(x => string.Equals(x.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault()
                    ?? _lastSsid;

                return _lastSsid ?? throw new HttpRequestException($"Could not authenticate with QBit on {_address}");
            }
        }

        private Task<HttpResponseMessage> SendGetRequest(string path, HttpContent content = null)
            => SendRequest(path, HttpMethod.Get, content);

        private Task<HttpResponseMessage> SendPostRequest(string path, HttpContent content = null)
            => SendRequest(path, HttpMethod.Post, content);

        private async Task<HttpResponseMessage> SendRequest(string path, HttpMethod method, HttpContent content = null) {
            using (var request = new HttpRequestMessage(method, $"{_address}/{path}")) {
                // This may seem silly - but it's the safest way to operate.
                // If every request checks if a new SSID is required, then if the
                // torrent client is restarted - no requests will error.
                request.Headers.TryAddWithoutValidation("Cookie", $"{await GetSSID()}");

                if (content != null) request.Content = content;
                return await _client.SendAsync(request);
            }
        }

        public async Task AddHash(string hash) {
            using (var multipartContent = new MultipartFormDataContent()) {
                multipartContent.Add(new StringContent(hash), "urls");
                await SendPostRequest($"api/v2/torrents/add", multipartContent);
            }
        }

        public async Task<IEnumerable<string>> GetActiveTorrentPaths() {
            var response = await SendGetRequest("api/v2/torrents/info");
            if (response.IsSuccessStatusCode) {
                string json = await response.Content.ReadAsStringAsync();
                // dynamics with strings seem to be the safest here
                // as we get unpredictable results from qbittorrent.
                var torrents = JArray.Parse(json);
                return torrents.Where(x => x["amount_left"]?.Value<string>() != "0" || string.Equals(x["state"]?.Value<string>(), "queuedDL", StringComparison.OrdinalIgnoreCase))
                                .Select(x => Path.Join(x["save_path"]?.Value<string>(), x["name"]?.Value<string>())).ToArray();
            }
            else throw new HttpRequestException($"Failed to connect to QBittorrent: {response.ReasonPhrase}");
        }

        public async Task RemoveCompleteTorrents() {
            var response = await SendGetRequest("api/v2/torrents/info");
            if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to connect to QBittorrent: {response.ReasonPhrase}");

            string json = await response.Content.ReadAsStringAsync();

            // dynamics with strings seem to be the safest here
            // as we get unpredictable results from qbittorrent.
            var torrents = JArray.Parse(json);
            var toRemoveHashes = torrents.Where(x => x["amount_left"]?.Value<string>() == "0")
                                            .Select(x => x["hash"]?.Value<string>());

            if (toRemoveHashes.Any()) {
                string address = $"api/v2/torrents/delete?hashes={string.Join('|', toRemoveHashes)}&deleteFiles=false";
                response = await SendGetRequest(address);
                if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Failed to delete torrents from QBittorrent: {response.ReasonPhrase}");
            }
        }
    }
}