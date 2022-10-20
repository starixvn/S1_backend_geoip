using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GeoIP.Core.Library;
using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public class MemberAllowIpDataLayers : IMemberAllowIpDataLayers
    {
        private readonly IMySqlClient _sqlClient;

        public MemberAllowIpDataLayers(IEnumerable<IMySqlClient> sqlClients)
        {
            _sqlClient = sqlClients.First(a => a.DataBase == DataBase.Report);
        }

        public IEnumerable<string> GetAllowIp()
        {
            var p = new DynamicParameters();
            p.Add("_Type", 1);
            return _sqlClient.Query<string>("Service_AllowIP_Get", p);
        }
    }
}