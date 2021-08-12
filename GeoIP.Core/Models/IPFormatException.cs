using System;

namespace GeoIP.Core.Models
{
    public class IPFormatException : Exception
    {
        public IPFormatException(string name) : base(name)
        {
            
        }
    }
}