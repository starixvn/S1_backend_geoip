using System.Collections.Generic;
using System.Linq;

namespace GeoIP.Core.Models
{
    public class ForbiddenCountries
    {
        public List<string> Countries { get; }

        public ForbiddenCountries(List<string> countries)
        {
            Countries = countries;
        }

        public bool Exists(string countryCode)
        {
            return string.IsNullOrEmpty(countryCode) || Countries.Contains(countryCode);
        }
    }
}