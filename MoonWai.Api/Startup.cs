using System;
using System.IO;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using MoonWai.Dal;

namespace MoonWai.Api
{
    public class Startup
    {   
        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var dbSettings = configuration.GetSection("DbSettings").Get<DbSettings>();

            Dc.CreateDefaultConfiguration(dbSettings);

            using var dc = new Dc();

            dc.CreateTables();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddAuthorization();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string clientPath = null;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                clientPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "MoonWai.Elmish", "public"));
            }
            else
            {
                app.UseExceptionHandler("/Error/Exception");
                app.UseHsts();

                clientPath = Path.Combine(env.ContentRootPath, "wwwroot", "dist");
            }

            app.UseHttpsRedirection();

            var fileProvider = new PhysicalFileProvider(clientPath);
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = fileProvider,
                RequestPath = ""
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = ""
            });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
#if DEBUG
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "../MoonWai.Elmish/public";
                    spa.Options.StartupTimeout = TimeSpan.FromSeconds(5);
                }
#endif
            });
        }
    }
}
