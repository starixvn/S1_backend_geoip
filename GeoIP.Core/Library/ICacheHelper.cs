using System.Threading.Tasks;

using GeoIP.Core.Models;

namespace GeoIP.Core.Library
{
    public interface ICacheHelper
    {
        Task<bool> KeyExistsAsync(string key, RedisDataBase redisDataBase = RedisDataBase.IP);
    }
}
