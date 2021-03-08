using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.FileManager.Clients;
using JJMedia5.FileManager.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JJMedia5.FileManager {

    internal class Program {

        // to do - clean up this.
        private static HttpClient _client = new HttpClient();

        private static RssService _rssService = new RssService(new EntityRepository<RssFeed>(new JJMediaDbManager {
            ConnString = "Data Source=htpc;Persist Security Info=True;User ID=jon;Password=jon;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;Database=JJMedia5"
        }), _client);

        private static TorrentService _torrentService = new TorrentService(new QBitClient(_client, address: "http://localhost:8080", userName: "admin", password: "adminadmin"));
        private static FileService _fileService = new FileService(storePaths: new[] { @"G:\JJStores" }, sourcePaths: new[] { @"G:\JJDownloads" });

        private static async Task Main(string[] args) {
            Console.WriteLine("Starting..");

            for (; ; await Task.Delay(600_000)) {
                await Scan();
            }
        }

        private static async Task Scan() {
            // We want to process these
            // in sequence, to avoid disrupting a previous step.
            var hashes = await _rssService.GetHashesFromFeeds();
            // filter out already downloaded hashes
            await _torrentService.DownloadHashes(hashes);
            // add hashes as downloaded.

            await _torrentService.RemoveCompleteTorrents();

            // There may have been torrents that failed to move in the past,
            // so we just re-process everything that isn't active.
            IEnumerable<string> paths = await _torrentService.GetActiveTorrentFilePaths();
            await _fileService.ProcessMedia(ignoredPaths: paths);
        }
    }
}