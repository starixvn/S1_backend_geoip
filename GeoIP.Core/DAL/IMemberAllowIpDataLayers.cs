using System.Collections.Generic;

namespace GeoIP.Core.DAL
{
    public interface IMemberAllowIpDataLayers
    {
        IEnumerable<string> GetAllowIp();
    }
}