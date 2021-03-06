using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.FileManager.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JJMedia5.FileManager {

    internal class Program {

        // to do - clean up this.
        private static RssService _rssService = new RssService(new EntityRepository<RssFeed>(new JJMediaDbManager {
            ConnString = "Data Source=htpc;Persist Security Info=True;User ID=jon;Password=jon;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;Database=JJMedia5"
        }), new HttpClient());

        private static async Task Main(string[] args) {
            Console.WriteLine("Starting..");

            for (; ; await Task.Delay(600_000)) {
                await Scan();
            }
        }

        private static async Task Scan() {
            // Download RSS feeds required.
            await _rssService.PollFeeds();

            // Delete active torrents that have complete.

            // Check the file store for files to move.
        }
    }
}