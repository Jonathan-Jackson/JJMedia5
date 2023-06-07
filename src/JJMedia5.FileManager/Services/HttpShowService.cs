using JJMedia5.Core.Interfaces;
using JJMedia5.Core.Models;
using JJMedia5.FileManager.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace JJMedia5.FileManager.Services {
    public class HttpShowService : IEpisodeLookupService {

        private readonly HttpClient _client;
        private readonly string _mediaApiAddress;

        public HttpShowService(StorageOptions options, HttpClient client) {
            _mediaApiAddress = options.MediaApiAddress;
            _client = client;
        }


        public async Task<EpisodeSeriesInfo> FindEpisodeSeriesInfoAsync(string fileName) {
            var result = await _client.GetAsync($"{_mediaApiAddress}search/seriesepisode?filename={HttpUtility.UrlEncode(fileName)}");

            if (result.IsSuccessStatusCode) {
                string body = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EpisodeSeriesInfo>(body, new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else {
                throw new HttpRequestException($"Failed to get episode info. Reason: {result.ReasonPhrase}");
            }
        }

    }
}
