using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace GeoIP.Library
{
    public interface IMySqlClient
    {
        Task<IEnumerable<T>> QueryAsync<T>(string name, SqlMapper.IDynamicParameters parameters);
        Task<T> QueryFirstAsync<T>(string name, SqlMapper.IDynamicParameters parameters);
    }
}