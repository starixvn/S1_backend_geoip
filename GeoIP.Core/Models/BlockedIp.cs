using System.Collections.Generic;

namespace GeoIP.Core.Models
{
    public class BlockedIp
    {
        private readonly List<string> _ipList;

        public BlockedIp(IEnumerable<string> ipList)
        {
            _ipList = (List<string>)ipList;
        }

        public bool Exists(string ip)
        {
            return _ipList.Contains(ip);
        }
    }
}