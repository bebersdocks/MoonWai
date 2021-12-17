using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using MoonWai.Api.Resources;

using Serilog;

namespace MoonWai.Api
{
    public class Program
    {
        public static string defaultAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        public static Translations Translations;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
