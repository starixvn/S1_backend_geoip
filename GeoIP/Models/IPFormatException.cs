using System;

namespace GeoIP.Models
{
    public class IPFormatException : Exception
    {
        public IPFormatException(string name) : base(name)
        {
            
        }
    }
}