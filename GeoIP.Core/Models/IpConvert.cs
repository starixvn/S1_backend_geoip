using System;
using System.Net;

namespace GeoIP.Core.Models
{
    public class IpConvert
    {
        public static uint GetIpToLong(string ipAddress)
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
            else if(ipAddress.IndexOf(":", StringComparison.Ordinal) > 0)
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