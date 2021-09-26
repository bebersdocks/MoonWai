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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Exception");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var elmPath = Path.Combine(env.ContentRootPath, "Resources/Elm");
            if (Directory.Exists(elmPath))
            {
                var fileProvider = new PhysicalFileProvider(elmPath);
                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    FileProvider = fileProvider
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = fileProvider
                });
            }

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
