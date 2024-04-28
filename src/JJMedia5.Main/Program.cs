using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Interfaces;
using JJMedia5.Core.Models;
using JJMedia5.FileManager.Clients;
using JJMedia5.FileManager.Options;
using JJMedia5.FileManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using JJMedia5.FileManager;
using JJMedia5.Media.Services;
using TMDbLib.Client;

namespace JJMedia5.Main {
    static class Program {
        private static IServiceProvider _provider;
        private static ILogger<object> _logger;

        private static async Task Main(string[] args) {
            _provider = SetupDependencies();

            _logger = _provider.GetRequiredService<ILogger<object>>();
            var fileManager = _provider.GetRequiredService<FileManagerService>();

            _logger.LogInformation("Application Startup Complete.");
            await Task.WhenAll(
               // Disabled the below for now, as we are using qBittorrent's native
               // RSS auto-downloader + moving completed files into seperate folders.
               // We're also keeping a FULL history of downloaded files in there, so we
               // don't need a history..
               //fileManager.PollFeeds(), 
               //fileManager.PollCompleteFiles(),
               fileManager.PollPendingFiles()
             );
        }

        private static IServiceProvider SetupDependencies() {
            IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();

            IServiceCollection services = new ServiceCollection();

            SetupFileManagerDependencies(config, services);
            SetupMediaDependencies(config, services);

            return services.BuildServiceProvider();
        }

        private static void SetupFileManagerDependencies(IConfiguration config, IServiceCollection services) {
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
                .AddSingleton<FileManagerService>()
                .AddLogging(configure => configure.AddConsole());
        }

        private static void SetupMediaDependencies(IConfiguration config, IServiceCollection services) {
            // Local Dependencies.
            services
                .AddSingleton<IRepository<RssFeed>, EntityRepository<RssFeed>>()
                .AddSingleton(new TMDbClient(config.GetValue<string>("TmdbKey")))
                .AddSingleton<ISeriesEpisodeSearchService, SeriesEpisodeSearchService>()
                .AddSingleton<SeriesRepository>()
                .AddSingleton<EpisodeRepository>()
                .AddTransient<SeriesSearchService>()
                .AddTransient<EpisodeSearchService>();
        }
    }
}
