using Elastic.Apm.StackExchange.Redis;

using GeoIP.Core;
using GeoIP.Core.BLL;
using GeoIP.Core.DAL;
using GeoIP.Core.Library;
using GeoIP.Core.Models;

using StackExchange.Redis;

namespace GeoIP.WebApi.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? config.GetValue<string>("ServiceType");
            services.AddSingleton(provider => new MySqlClient(provider, DataBase.Report, config.GetConnectionString($"{serviceType}_ReportDB")));
            services.AddSingleton(provider => new MySqlClient(provider, DataBase.ReportIPTable, config.GetConnectionString($"{serviceType}_ReportIPTable")));
            return services;
        }

        public static IServiceCollection AddRedisConnection(this IServiceCollection services, IConfiguration config)
        {
            var redisConnection = ConnectionMultiplexer.Connect(config.GetValue<string>("RedisConnection"));
            redisConnection.UseElasticApm();
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddSingleton<ICacheHelper, RedisCacheHelper>();

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<GeoIpService>();
            services.AddSingleton<ConfigService>();
            services.AddOptionList<IPCountry>("Service_IPCode_Get", dataBase: DataBase.ReportIPTable);

            //處理SiteConfig
            services.AddOptionList<ServerInfo>("ServerInfoList_Get", dataBase: DataBase.Report);
            services.AddOptionList<ConfigObject>("SiteConfig_Get", new { ConfigGroup = "GeoIp" }, dataBase: DataBase.Report);
            services.AddSiteConfig();
            services.AddSingleton<ConfigBLL>();
            return services;
        }

        public static IServiceCollection AddSiteConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            // 準備ServerInfo
            var serverInfoList = new List<ServerInfo>() { new ServerInfo() { SiteId = 0, SiteName = "default" } };
            serverInfoList.AddRange(provider.GetRequiredService<IEnumerable<ServerInfo>>());

            // 準備Config
            var siteConfigGroup = provider.GetRequiredService<IEnumerable<ConfigObject>>().GroupBy(i => i.SiteId);

            foreach (var serverInfo in serverInfoList)
            {
                //設定SiteConfig
                var siteConfig = siteConfigGroup.FirstOrDefault(i => i.Key == serverInfo.SiteId) ?? siteConfigGroup.First(i => i.Key == 0);
                var config = siteConfig.ToDictionary(i => i.ConfigKey, i => i.ConfigValue);
                services.AddSingleton(new SiteConfigObject(serverInfo, config));
            }

            return services;
        }

        public static IServiceCollection AddOptionList<T>(this IServiceCollection services, string sql, object parameters = null, DataBase dataBase = DataBase.Report)
        {
            var provider = services.BuildServiceProvider();
            var configService = provider.GetRequiredService<ConfigService>();
            var option = configService.Query<T>(sql, parameters, dataBase);
            services.AddSingleton(typeof(IEnumerable<T>), p => option);
            return services;
        }
    }
}