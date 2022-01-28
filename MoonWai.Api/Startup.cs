using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using LinqToDB.AspNet;
using LinqToDB.Configuration;

using MoonWai.Api.Resources;
using MoonWai.Api.Services;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;

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

            var languageId = configuration.GetValue<LanguageId>(nameof(LanguageId));
            
            Program.Translations = Translations.Load(languageId);
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
            
            using var dc = new Dc();

            var connStr = dc.ConnectionString;
                
            services.AddLinqToDbContext<Dc>((provider, options) => options.UsePostgreSQL(connStr));

            services.AddSingleton<ThreadService, ThreadService>();
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

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../MoonWai.Ui";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
