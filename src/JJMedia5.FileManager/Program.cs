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

namespace JJMedia5.FileManager {

    internal static class Program {
        private static IServiceProvider _provider;
        private static ILogger<object> _logger;

        private static async Task Main(string[] args) {
            _provider = SetupDependencies();

            _logger = _provider.GetRequiredService<ILogger<object>>();
            var fileManager = _provider.GetRequiredService<FileManagerService>();

            _logger.LogInformation("Application Startup Complete.");
            await Task.WhenAll(
                fileManager.PollFeeds(), 
                fileManager.PollCompleteFiles(), 
                fileManager.PollPendingFiles()
            );
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
                .AddSingleton<IEpisodeLookupService, HttpShowService>()
                .AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }
    }
}