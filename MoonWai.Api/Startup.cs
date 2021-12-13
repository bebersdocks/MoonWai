using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using MoonWai.Api.Utils;
using MoonWai.Dal;

using Serilog;

namespace MoonWai.Api
{
    public class Startup
    {   
        private readonly string devPolicy = "dev";

        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var loggingSetings = configuration
                .GetSection(nameof(LoggingSettings))
                .Get<LoggingSettings>();

            Log.Logger = Logging.CreateLogger(loggingSetings);

            var dbSettings = configuration
                .GetSection(nameof(DbSettings))
                .Get<DbSettings>();

            Dc.CreateDefaultConfiguration(dbSettings);

            using var dc = new Dc();

            dc.CreateTables();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options => 
            {
                options.AddPolicy(name: devPolicy, builder => 
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddAuthorization();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(devPolicy);
            }
            else
            {
                app.UseExceptionHandler("/Error/Exception");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseSerilogRequestLogging();       
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
