using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JJMedia5 {

    public static class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://*:4444");
                });//.UseAllElasticApm();
    }
}