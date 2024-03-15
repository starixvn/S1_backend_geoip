using System;
using System.Collections.Generic;
using System.Linq;

using GeoIP.Core.Library;
using GeoIP.Core.Models;

using Microsoft.Extensions.DependencyInjection;

namespace GeoIP.Core.DAL
{
    public class ConfigService
    {
        private readonly MySqlClient _reportIpTableClient;
        private readonly MySqlClient _reportDbClient;

        public ConfigService(IServiceProvider provider)
        {
            _reportIpTableClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.ReportIPTable);
            _reportDbClient = provider.GetServices<MySqlClient>().First(p => p.DataBase == DataBase.Report);
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null, DataBase dataBase = DataBase.Report)
        {
            return dataBase switch
            {
                DataBase.Report => _reportDbClient.Query<T>(sql, parameters),
                DataBase.ReportIPTable => _reportIpTableClient.Query<T>(sql, parameters),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
