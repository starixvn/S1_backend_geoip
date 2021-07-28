using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GeoIP.Models
{
    public class AllowedAreas
    {
        private readonly List<IPRange> _ipRanges;

        public AllowedAreas(List<IPRange> ipRanges)
        {
            _ipRanges = ipRanges;
        }

        public bool Exists(string ip)
        {
            var uintIP = GetIpToLong(ip);
            var range = _ipRanges.SingleOrDefault(o => uintIP >= o.From && uintIP <= o.To);
            return range != null;
        }

        public uint GetIpToLong(string ipAddr)
        {
            byte[] b = IPAddress.Parse(ipAddr).GetAddressBytes();
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }
    }
}