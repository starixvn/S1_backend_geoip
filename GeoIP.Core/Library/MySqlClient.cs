using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using GeoIP.Core.Models;

using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;
using Serilog.Context;

namespace GeoIP.Core.Library
{
    public class MySqlClient : IMySqlClient
    {
        public DataBase DataBase { get; set; }
        private readonly string _connectionString;

        private readonly ILogger<MySqlClient> _logger;

        public MySqlClient(string connectionString, ILogger<MySqlClient> logger, DataBase dataBase)
        {
            _connectionString = connectionString;
            _logger = logger;
            DataBase = dataBase;
        }

        public IEnumerable<T> Query<T>(string name, SqlMapper.IDynamicParameters parameters)
        {
            IEnumerable<T> result;
            var sw = Stopwatch.StartNew();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                result = connection.Query<T>(name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                _logger.LogError(e, name);
                throw;
            }

            _logger.LogTrace($"{name} {sw.Elapsed:c}");
            return result;
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string name, SqlMapper.IDynamicParameters parameters)
        {
            IEnumerable<T> result;
            var sw = Stopwatch.StartNew();
            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                result = await connection.QueryAsync<T>(name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                _logger.LogError(e, name);
                throw;
            }

            _logger.LogTrace($"{name} {sw.Elapsed:c}");
            return result;
        }

        public async Task<T> QueryFirstAsync<T>(string name, SqlMapper.IDynamicParameters parameters)
        {
            T result;
            var sw = Stopwatch.StartNew();
            try
            {
                await using var connection = new MySqlConnection(_connectionString);
                result = await connection.QueryFirstOrDefaultAsync<T>(name, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                _logger.LogError(e, name);
                throw;
            }

            _logger.LogTrace($"{name} {sw.Elapsed:c}");
            return result;
        }

        public IEnumerable<T> Query<T>(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            IEnumerable<T> result;
            var sw = Stopwatch.StartNew();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var command = new CommandDefinition(sql, parameters, // 使用 CommandDefinition 不讓 Dapper 用預設值建立
                    commandType: commandType,
                    flags: CommandFlags.NoCache);

                result = connection.Query<T>(command);
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError(e, sql);
                throw;
            }

            LogContext.PushProperty("ElapsedMilliseconds", sw.Elapsed.TotalMilliseconds);
            _logger.LogDebug($"{sql} {sw.Elapsed:c}");
            return result.ToList();
        }
    }
}