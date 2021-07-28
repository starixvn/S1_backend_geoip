using System.Drawing;

namespace GeoIP.Models
{
    public class IPCountry
    {
        public uint IpFrom { get; set; }
        public uint IpTo { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
    }
}