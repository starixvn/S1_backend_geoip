using System.Data;
using System.Threading.Tasks;
using Dapper;
using GeoIP.Library;
using GeoIP.Models;

namespace GeoIP.DAL
{
    public class IpCountryDataLayers : IIpCountryDataLayers
    {
        private readonly IMySqlClient _sqlClient;

        public IpCountryDataLayers(IMySqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public async Task<IPCountry> GetIpCountry(uint ip)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Ip", ip, DbType.UInt32);
            return await _sqlClient.QueryFirstAsync<IPCountry>("Service_IPCountry_Get", parameters);
        }
    }
}