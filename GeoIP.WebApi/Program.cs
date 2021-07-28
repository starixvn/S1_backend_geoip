using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureAppConfiguration((context, builder) => { Configuration(args, builder, context); });
                });
        
        private static void Configuration(string[] args, IConfigurationBuilder builder, WebHostBuilderContext context)
        {
            builder.SetBasePath(AppContext.BaseDirectory)
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    false, true);
            context.Configuration = builder.Build();
        }
    }
}