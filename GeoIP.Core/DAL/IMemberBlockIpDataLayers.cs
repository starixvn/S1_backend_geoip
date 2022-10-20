using System.Collections.Generic;
using System.Threading.Tasks;
using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public interface IMemberBlockIpDataLayers
    {
        IEnumerable<string> GetBlockIp();
    }
}