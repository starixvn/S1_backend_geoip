using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using GeoIP.Core.Models;

namespace GeoIP.Core.Library
{
    public interface IMySqlClient
    {
        public DataBase DataBase { get; set; }
        Task<IEnumerable<T>> QueryAsync<T>(string name, SqlMapper.IDynamicParameters parameters);
        Task<T> QueryFirstAsync<T>(string name, SqlMapper.IDynamicParameters parameters);
    }
}