using JJMedia5.FileManager.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class TorrentService {
        private ITorrentClient _client;

        public TorrentService(ITorrentClient client) {
            _client = client;
        }

        public Task RemoveCompleteTorrents()
            => _client.RemoveCompleteTorrents();

        public Task<IEnumerable<string>> GetActiveTorrentPaths()
            => _client.GetActiveTorrentPaths();

        public async Task DownloadHashes(IEnumerable<string> hashes) {
            foreach (var hash in hashes)
                await _client.AddHash(hash);
        }
    }
}