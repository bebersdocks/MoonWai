using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using MoonWai.Api.Resources.Translations;

using Serilog;

namespace MoonWai.Api
{
    public class Program
    {
        public static string defaultAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        
        public static void Main(string[] args)
        {
            Log.Logger = Logging.CreateLogger();
                
            Translations.Load();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
