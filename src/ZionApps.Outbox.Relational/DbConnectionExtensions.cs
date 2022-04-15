using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ZionApps.Outbox.Relational
{
    public static class DbConnectionExtensions
    {
        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string sql,
            DbTransaction? transaction = null,
            params object[] sqlParams)
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            foreach (var param in sqlParams)
            {
                command.Parameters.Add(param);
            }

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            return await command.ExecuteNonQueryAsync();
        }

        public static async IAsyncEnumerable<DbDataReader> ExecuteReader(this DbConnection connection, string sql,
            params object[] sqlParams)
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            foreach (var param in sqlParams)
            {
                command.Parameters.Add(param);
            }

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return reader;
            }
        }
    }
}