using System;
using System.Collections.Generic;
using GeoIP.Core;
using GeoIP.Core.DAL;
using GeoIP.Core.Models;
using GeoIP.WebApi.Extension;
using GeoIP.WebApi.Extension.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeoIP.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddDatabaseConnection(Configuration)
                .Configure<GeoIpSettings>(option =>
                {
                    Configuration.GetSection("GeoIpSettings").Bind(option);
                })
                .AddSingleton<ILogger<GeoIpService>, Logger<GeoIpService>>()
                .AddSingleton<GeoIpService>()
                .AddSingleton<GeoIpSettings>()
                .AddSingleton<IFile, FileLoader>()
                .AddSingleton<IIpCountryDataLayers, IpCountryDataLayers>()
                .AddSingleton<IMemberBlockIpDataLayers, MemberBlockIpDataLayers>()
                .AddSingleton<IMemberAllowIpDataLayers, MemberAllowIpDataLayers>()
                .AddBlockIpByDb()
                .AddAllowedIpByDb()
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                })
                .AddControllers()
                .AddMvcOptions(x => { x.EnableEndpointRouting = false; })
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddOptionListByDB<IEnumerable<IPCountry>>("Service_IPCode_Get");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseRequestLogMiddleware();
            app.UseResponseLogMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/health", async context =>
                {
                    await context.Response.WriteAsync($"{env.EnvironmentName}-{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                });

                endpoints.MapControllers();
            });
        }
    }
}