using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Clients {

    public class QBitClient : ITorrentClient {
        private readonly string _userName;
        private readonly string _address;
        private readonly string _password;
        private readonly HttpClient _client;

        public QBitClient(HttpClient client, string address, string userName, string password) {
            _client = client;
            _userName = userName;
            _password = password;
            _address = address;
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

                return response.Headers.FirstOrDefault(x => string.Equals(x.Key, "Set-Cookie", StringComparison.OrdinalIgnoreCase)).Value.FirstOrDefault()
                    ?? throw new HttpRequestException("Did not recieve a Set-Cookie header from QBittorent for Authentication.");
            }
        }

        public async Task AddHash(string hash) {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{_address}/api/v2/torrents/add")) {
                request.Headers.TryAddWithoutValidation("Cookie", $"{await GetSSID()}");

                using (var multipartContent = new MultipartFormDataContent()) {
                    multipartContent.Add(new StringContent(hash), "urls");
                    request.Content = multipartContent;

                    await _client.SendAsync(request);
                }
            }
        }
    }
}