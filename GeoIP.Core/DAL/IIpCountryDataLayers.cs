using System.Threading.Tasks;

using GeoIP.Core.Models;

namespace GeoIP.Core.DAL
{
    public interface IIpCountryDataLayers
    {
        Task<IPCountry> GetIpCountry(uint ip);
    }
}