using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using GeoIP.Core.Library;
using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public class IpCountryDataLayers : IIpCountryDataLayers
    {
        private readonly IMySqlClient _sqlClient;

        public IpCountryDataLayers(IEnumerable<IMySqlClient> sqlClients)
        {
            _sqlClient = sqlClients.First(a => a.DataBase == DataBase.ReportIPTable);
        }
        public async Task<IPCountry> GetIpCountry(uint ip)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Ip", ip, DbType.UInt32);
            return await _sqlClient.QueryFirstAsync<IPCountry>("Service_IPCountry_Get", parameters);
        }
    }
}