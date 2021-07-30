using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoIP.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace GeoIP.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(Configuration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog((hostContext, loggerConfiguration) =>
                        {
                            var serilogOption = new SerilogOption();
                            hostContext.Configuration.GetSection("Serilog").Bind(serilogOption);
                            
                            var fullLogPath = $"{serilogOption.Path}/{serilogOption.FileName}";
                            
                            loggerConfiguration.Enrich.FromLogContext()
                                .Enrich.WithExceptionDetails();
                            
                            loggerConfiguration.WriteTo.File(new JsonFormatter(),
                                    fullLogPath,
                                    serilogOption.MinimumLevel.Default,
                                    serilogOption.FileSizeLimitBytes,
                                    null,
                                    false,
                                    true,
                                    null,
                                    RollingInterval.Day,
                                    serilogOption.RollOnFileSizeLimit,
                                    null)
                                .MinimumLevel.Information();
                        })
                        .UseStartup<Startup>();
                });

        private static readonly Action<IConfigurationBuilder> Configuration =
            builder => builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    false, true);
    }
}