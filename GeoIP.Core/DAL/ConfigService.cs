using GeoIP.Core.Library;
using GeoIP.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoIP.Core.DAL
{
    public class ConfigService
    {
        private readonly IMySqlClient _sqlClient;

        public ConfigService(IEnumerable<IMySqlClient> sqlClients)
        {
            _sqlClient = sqlClients.First(a => a.DataBase == DataBase.Report);
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            return _sqlClient.Query<T>(sql, parameters);
        }
    }
}
