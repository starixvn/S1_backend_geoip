using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoIP.Core.Library;
using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public class MemberBlockIpDataLayers : IMemberBlockIpDataLayers
    {
        private readonly IMySqlClient _sqlClient;

        public MemberBlockIpDataLayers(IEnumerable<IMySqlClient> sqlClients)
        {
            _sqlClient = sqlClients.First(a => a.DataBase == DataBase.IPTable);
        }

        public async Task<IEnumerable<string>> GetBlockIp()
        {
            return await _sqlClient.QueryAsync<string>("Service_BlockIP_Get", null);
        }
    }
}