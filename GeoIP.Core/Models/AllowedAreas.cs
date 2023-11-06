using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GeoIP.Core.Models
{
    public class AllowedAreas
    {
        private readonly List<IPRange> _ipRanges;

        public AllowedAreas(List<IPRange> ipRanges)
        {
            _ipRanges = ipRanges;
        }

        public bool Exists(uint uintIp)
        {
            var range = _ipRanges.SingleOrDefault(o => uintIp >= o.From && uintIp <= o.To);
            return range != null;
        }

        public uint GetIpToLong(string ipAddress)
        {
            if (ipAddress.IndexOf(".", StringComparison.Ordinal) > 0)
            {
                try
                {
                    var b = IPAddress.Parse(ipAddress).GetAddressBytes();
                    Array.Reverse(b);
                    return BitConverter.ToUInt32(b, 0);
                }
                catch (Exception e)
                {
                    throw new IPFormatException("ipv4 format error");
                }
            }
            else if (ipAddress.IndexOf(":", StringComparison.Ordinal) > 0)
            {
                throw new IPFormatException("no support ipv6");
            }
            else
            {
                throw new IPFormatException("ip format error");
            }
        }
    }
}