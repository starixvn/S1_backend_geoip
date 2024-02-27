using System;
using System.Collections.Generic;
using System.Text;

namespace GeoIP.Core.Models
{
    public class BlockedCountry
    {
        private readonly List<string> _countryList;

        public BlockedCountry(List<string> countryList)
        {
            _countryList = countryList;
        }

        public bool Exists(string country)
        {
            return _countryList.Contains(country);
        }
    }
}
