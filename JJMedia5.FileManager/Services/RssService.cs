using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using System;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class RssService {
        private readonly IRepository<RssFeed> _repo;

        public RssService(IRepository<RssFeed> repo) {
            _repo = repo;
        }

        public async Task PollFeeds() {
            var feeds = await _repo.WhereLessThanAsync(r => r.StartDate, DateTimeOffset.UtcNow, 1000);

            foreach (var feed in feeds)
                await ProcessFeed(feed); // yes wait on each one, we don't want to ddos the endpoints.
        }

        public async Task ProcessFeed(RssFeed feed) {
            // Find what we should download.
            await Task.Delay(500);

            // Send it to the torrent client.
        }
    }
}