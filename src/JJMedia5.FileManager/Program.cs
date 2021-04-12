using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Models;
using JJMedia5.FileManager.Clients;
using JJMedia5.FileManager.Options;
using JJMedia5.FileManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JJMedia5.FileManager {

    internal static class Program {
        private static FileService _fileService;
        private static ILogger<object> _logger;
        private static IServiceProvider _provider;
        private static RssService _rssService;
        private static SemaphoreSlim _semaphore;
        private static TorrentService _torrentService;

        private static async Task Main(string[] args) {
            _provider = SetupDependencies();

            _semaphore = new SemaphoreSlim(1, 1);
            _logger = _provider.GetRequiredService<ILogger<object>>();
            _fileService = _provider.GetRequiredService<FileService>();
            _torrentService = _provider.GetRequiredService<TorrentService>();
            _rssService = _provider.GetRequiredService<RssService>();

            _logger.LogInformation("Application Startup Complete.");
            await Task.WhenAll(PollFeeds(), PollCompleteFiles());
        }

        private static async Task PollCompleteFiles() {
            for (; ; await Task.Delay(60_000)) {
                try {
                    await _semaphore.WaitAsync();
                    // Clear out any empty ones..
                    await _torrentService.RemoveCompleteTorrents();

                    // There may have been torrents that failed to move in the past,
                    // so we just re-process everything that isn't active.
                    IEnumerable<string> paths = await _torrentService.GetActiveTorrentPaths();
                    await _fileService.ProcessMedia(ignoredPaths: paths);

                    _logger.LogInformation("Poll for completed files has finished.");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error thrown on polling for complete files.");
                }
                finally {
                    _semaphore.Release();
                }
            }
        }

        private static async Task PollFeeds() {
            for (; ; await Task.Delay(600_000)) {
                try {
                    await _semaphore.WaitAsync();

                    // We want to process these
                    // in sequence, to avoid disrupting a previous step.
                    var toDownload = (await _rssService.GetNewHashesFromFeeds()).TakeLast(10);

                    if (toDownload.Any()) {
                        await _torrentService.DownloadHashes(toDownload.Select(d => d.Hash));
                        await _rssService.AddHashDownloads(toDownload);
                        foreach (var download in toDownload) {
                            _logger.LogInformation($"Downloaded New Hash: {download.Id}");
                        }

                        // add a slight delay for the above to register correctly in
                        // the torrent client - otherwise we'll skip too far ahead &
                        // pick up files that aren't properly registered in our torrent client.
                        await Task.Delay(5000);
                    }

                    _logger.LogInformation("Poll for feeds has finished.");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error thrown on polling feeds.");
                }
                finally {
                    _semaphore.Release();
                }
            }
        }

        private static IServiceProvider SetupDependencies() {
            IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();

            IServiceCollection services = new ServiceCollection();
            // Remote Dependencies.
            services.AddSingleton<HttpClient>()
                .AddSingleton(config.GetSection("TorrentClientOptions").Get<BasicAuthEndPoint>())
                .AddSingleton(config.GetSection("StorageOptions").Get<StorageOptions>())
                .AddSingleton(new JJMediaDbManager(config.GetConnectionString("JJMediaDb")));
            // Local Dependencies.
            services.AddSingleton<IRepository<RssFeed>, EntityRepository<RssFeed>>()
                .AddSingleton<IRepository<RssDownload>, EntityRepository<RssDownload>>()
                .AddSingleton<RssService>()
                .AddSingleton<ITorrentClient, QBitClient>()
                .AddSingleton<TorrentService>()
                .AddSingleton<FileService>()
                .AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }
    }
}