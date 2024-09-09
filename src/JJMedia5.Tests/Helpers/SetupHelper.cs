using JJMedia5.Core.Database;
using JJMedia5.Core.Database.DbLite;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Interfaces;
using JJMedia5.Media.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TMDbLib.Client;

namespace JJMedia5.Tests.Helpers {

    /// <summary>
    /// This class is probably pointless, but I can't be bothered to look into how you do this properly (yet).
    /// </summary>
    public static class SetupHelper {
        public static IServiceProvider GetDependencyProvider() {
            var services = new ServiceCollection();
            var config = GetConfiguration();

            services
                .AddSingleton<IRepository<RssFeed>, DbLiteRepository<RssFeed>>()
                .AddSingleton(new TMDbClient(config.GetValue<string>("TmdbKey")))
                .AddSingleton<ISeriesEpisodeSearchService, SeriesEpisodeSearchService>()
                .AddSingleton<DbLiteSeriesRepository>()
                .AddSingleton<DbLiteEpisodeRepository>()
                .AddTransient<SeriesSearchService>()
                .AddTransient<EpisodeSearchService>()
                .AddSingleton<HttpClient>()
                .AddSingleton(new JJMediaDbManager(config.GetConnectionString("JJMediaDb")))
                .AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }

        public static IConfiguration GetConfiguration() {
            IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();

            return config;
        }
    }
}
