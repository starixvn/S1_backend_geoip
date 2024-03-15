using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Dapper;

using GeoIP.Core.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

using Serilog.Context;

namespace GeoIP.Core.Library
{
    public class MySqlClient
    {
        public DataBase DataBase { get; set; }

        private readonly string _connectionString;

        private readonly ILogger<MySqlClient> _logger;

        public MySqlClient(IServiceProvider provider, DataBase dataBase, string connectionString)
        {
            DataBase = dataBase;
            _logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            _connectionString = connectionString;
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            IEnumerable<T> result;
            var sw = Stopwatch.StartNew();

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var command = new CommandDefinition(sql, parameters, // 使用 CommandDefinition 不讓 Dapper 用預設值建立
                    commandType: CommandType.StoredProcedure,
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