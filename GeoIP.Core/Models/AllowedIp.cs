using System.Collections.Generic;

namespace GeoIP.Core.Models
{
    public class AllowedIp
    {
        private readonly List<string> _ipList;

        public AllowedIp(List<string> ipList)
        {
            _ipList = ipList;
        }
        public bool Exists(string ip)
        {
            return _ipList.Contains(ip);
        }
    }
}