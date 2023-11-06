using System.Collections.Generic;
using System.Linq;

using GeoIP.Core.Library;
using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public class ConfigService
    {
        private readonly IMySqlClient _sqlClient;

        public ConfigService(IEnumerable<IMySqlClient> sqlClients)
        {
            _sqlClient = sqlClients.First(p => p.DataBase == DataBase.ReportIPTable);
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            return _sqlClient.Query<T>(sql, parameters);
        }
    }
}
