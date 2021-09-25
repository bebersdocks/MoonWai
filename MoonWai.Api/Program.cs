using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using MoonWai.Api.Resources.Translations;

namespace MoonWai.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Translations.Load();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
