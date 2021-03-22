using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.FileManager.Clients;
using JJMedia5.FileManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JJMedia5.FileManager {

    internal class Program {

        // to do - clean up this.
        private static HttpClient _client = new HttpClient();

        private static JJMediaDbManager _dbManager = new JJMediaDbManager {
            ConnString = "Data Source=htpc;Persist Security Info=True;User ID=jon;Password=jon;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;Database=JJMedia5"
        };

        private static RssService _rssService = new RssService(new EntityRepository<RssFeed>(_dbManager), new EntityRepository<RssDownload>(_dbManager), _client);
        private static TorrentService _torrentService = new TorrentService(new QBitClient(_client, address: "http://localhost:8080", userName: "admin", password: "adminadmin"));
        private static FileService _fileService = new FileService(storePaths: new[] { @"G:\JJStores" }, sourcePaths: new[] { @"G:\JJDownloads" }, @"G:\JJProcessing");

        private static async Task Main(string[] args) {
            Console.WriteLine("Starting..");

            for (; ; await Task.Delay(600_000)) {
                await Scan();
            }
        }

        private static async Task Scan() {
            // We want to process these
            // in sequence, to avoid disrupting a previous step.
            var toDownload = await _rssService.GetNewHashesFromFeeds();
            await _torrentService.DownloadHashes(toDownload.Select(d => d.Hash));
            await _rssService.AddHashDownloads(toDownload);
            await _torrentService.RemoveCompleteTorrents();

            // There may have been torrents that failed to move in the past,
            // so we just re-process everything that isn't active.
            IEnumerable<string> paths = await _torrentService.GetActiveTorrentPaths();
            await _fileService.ProcessMedia(ignoredPaths: paths);
        }
    }
}