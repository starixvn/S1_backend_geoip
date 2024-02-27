using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IServiceCollection AddOptionListByDB<T>(this IServiceCollection services, string sql, object parameters = null)
        {
            var provider = services.BuildServiceProvider();
            var configService = provider.GetRequiredService<ConfigService>();
            var option = configService.Query<T>(sql, parameters);
            services.AddSingleton(typeof(IEnumerable<T>), p => option);
            return services;
        }
    }
}