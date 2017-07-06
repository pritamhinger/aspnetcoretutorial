using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using HelloWorldAspNetCore.Extensions;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace HelloWorldAspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Warning);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("Home/Error");
            }

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("defaults/cutomLandingPage.html");
            app.UseDefaultFiles(options);

            var provider = new FileExtensionContentTypeProvider();
            // Add New Mappings
            provider.Mappings[".myapp"] = "application/x-msdownload";
            provider.Mappings[".htm3"] = "text/html";
            provider.Mappings[".image"] = "image/png";
            // Replace an Existing mapping
            provider.Mappings[".rtf"] = "application/x-msdownload";
            // Remove MP4 Videos.
            provider.Mappings.Remove(".mp4");

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/resrc"),
                ContentTypeProvider = provider,
                /* From the Documentation: 
                 * Enabling ServeUnknownFileTypes is a security risk and using it is discouraged.
                 * FileExtensionContentTypeProvider provides a safer alternative to serving files with non-standard extensions.
                */
                ServeUnknownFileTypes = true,
                DefaultContentType = "image/png",
                OnPrepareResponse = context => {
                    context.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                }
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions() {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
                RequestPath = new PathString("/MyImages")
            });

            app.UseRequestCulture();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });

            //app.Run(async (context) => {
            //    await context.Response.WriteAsync($"Hello {CultureInfo.CurrentCulture.DisplayName}");
            //});
        }
    }
}
