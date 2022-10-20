using System;
using System.Linq;
using GeoIP.Core;
using GeoIP.Core.DAL;
using GeoIP.Core.Library;
using GeoIP.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeoIP.WebApi.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? config.GetValue<string>("ServiceType");

            services.AddSingleton<IMySqlClient>(param =>
                    new MySqlClient(config.GetConnectionString($"{serviceType}_ReportDB"), logger, DataBase.Report))
                .AddSingleton<IMySqlClient>(param =>
                    new MySqlClient(config.GetConnectionString($"{serviceType}_IPTable"), logger, DataBase.IPTable));

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<GeoIpSettings>(option =>
                {
                    configuration.GetSection("GeoIpSettings").Bind(option);
                })
                .AddSingleton<ILogger<GeoIpService>, Logger<GeoIpService>>()
                .AddSingleton<GeoIpService>()
                .AddSingleton<GeoIpSettings>()
                .AddSingleton<IFile, FileLoader>()
                .AddSingleton<IIpCountryDataLayers, IpCountryDataLayers>()
                .AddSingleton<IMemberBlockIpDataLayers, MemberBlockIpDataLayers>()
                .AddSingleton<IMemberAllowIpDataLayers, MemberAllowIpDataLayers>()
                .AddBlockIpByDb()
                .AddAllowedIpByDb();
            return services;
        }

        public static IServiceCollection AddBlockIpByDb(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var dataLayers = provider.GetRequiredService<IMemberBlockIpDataLayers>();
            services.AddSingleton(p => new BlockedIp(dataLayers.GetBlockIp().ToList()));
            return services;
        }
        public static IServiceCollection AddAllowedIpByDb(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var dataLayers = provider.GetRequiredService<IMemberAllowIpDataLayers>();
            services.AddSingleton(p => new AllowedIp(dataLayers.GetAllowIp().ToList()));
            return services;
        } 
    }
}