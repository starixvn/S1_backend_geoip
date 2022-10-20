using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeoIP.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace GeoIP.WebApi
{
    public class Program
    {
        private static string _env;
        public static void Main(string[] args)
        {
            _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"; 
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(Configuration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration((hostContext, builder) =>
                        {
                            builder.AddJsonStream(GetRemoteConfig(hostContext.Configuration.GetSection("ConfigService").Get<string[]>()));
                        })
                        .UseSerilog((hostContext, loggerConfiguration) =>
                        {
                            var serilogOption = new SerilogOption();
                            hostContext.Configuration.GetSection("Serilog").Bind(serilogOption);
                            
                            var fullLogPath = $"{serilogOption.Path}/{serilogOption.FileName}";
                            
                            loggerConfiguration.Enrich.FromLogContext()
                                .Enrich.WithExceptionDetails();

                            foreach (var log in serilogOption.MinimumLevel.Overwrite)
                            {
                                loggerConfiguration.MinimumLevel.Override(log.Name, log.Level);
                            }

                            var excludesApiEndpoints = hostContext.Configuration.GetSection("ExcludesApiEndpoint").Get<string[]>();
                            loggerConfiguration.Filter.ByExcluding(p =>
                                p.Properties.Any(q =>
                                    excludesApiEndpoints.Any(item => q.Value.ToString().Contains(item))
                                    ));
                            
                            switch (hostContext.HostingEnvironment.EnvironmentName)
                            {
                                case "Local":
                                    loggerConfiguration
                                        .MinimumLevel.Debug()
                                        .WriteTo
                                        .LiterateConsole();
                                    break;
                                default:
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
                                    break;
                            }
                        })
                        .UseStartup<Startup>();
                });

        private static readonly Action<IConfigurationBuilder> Configuration =
            builder => builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile($"appsettings.{_env}.json", false, true);

        private static MemoryStream GetRemoteConfig(IEnumerable<string> remoteUrls)
        {
            using var client = new HttpClient();
            Stream result = null;
            foreach (var url in remoteUrls)
            {
                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }

                try
                {
                    result = client.GetAsync(url).Result.Content.ReadAsStreamAsync().Result;
                    break;
                }
                catch { }
            }

            using var streamReader = new StreamReader(result);
            return new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()));
        }
    }
}