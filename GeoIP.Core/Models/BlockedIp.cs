using System.Collections.Generic;
using System.Linq;

namespace GeoIP.Core.Models
{
    public class BlockedIp
    {
        private readonly List<string> _ipList;

        public BlockedIp(List<string> ipList)
        {
            _ipList = ipList;
        }

        public bool Exists(string ip)
        {
            return _ipList.Contains(ip);
        }
    }
}