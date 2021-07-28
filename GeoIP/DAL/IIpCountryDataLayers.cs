using System.Threading.Tasks;
using GeoIP.Models;

namespace GeoIP.DAL
{
    public interface IIpCountryDataLayers
    {
        Task<IPCountry> GetIpCountry(uint ip);
    }
}