using System.Collections.Generic;

namespace GeoIP.Core.DAL
{
    public interface IMemberBlockIpDataLayers
    {
        IEnumerable<string> GetBlockIp();
    }
}