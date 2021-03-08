using JJMedia5.FileManager.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class TorrentService {
        private ITorrentClient _client;

        public TorrentService(ITorrentClient client) {
            _client = client;
        }

        public Task RemoveCompleteTorrents() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetActiveTorrentFilePaths() {
            throw new NotImplementedException();
        }

        public async Task DownloadHashes(IEnumerable<string> hashes) {
            foreach (var hash in hashes)
                await _client.AddHash(hash);
        }
    }
}