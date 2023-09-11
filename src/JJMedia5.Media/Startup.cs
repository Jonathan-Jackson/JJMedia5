using FluentValidation.AspNetCore;
using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Interfaces;
using JJMedia5.Media.Services;
using JJMedia5.Media.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMDbLib.Client;

namespace JJMedia5 {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            services
                .AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RssFeedValidator>());

            services
                .AddSingleton(new JJMediaDbManager(Configuration.GetConnectionString("JJMediaDb")))
                .AddSingleton<IRepository<RssFeed>, EntityRepository<RssFeed>>()
                .AddSingleton(new TMDbClient(Configuration.GetValue<string>("TmdbKey")))
                .AddSingleton<ISeriesEpisodeSearchService, SeriesEpisodeSearchService>()
                .AddSingleton<SeriesRepository>()
                .AddSingleton<EpisodeRepository>()
                .AddTransient<SeriesSearchService>()
                .AddTransient<EpisodeSearchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}