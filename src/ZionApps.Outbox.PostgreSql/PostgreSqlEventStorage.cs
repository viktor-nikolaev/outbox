using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using ZionApps.Outbox.Models;
using ZionApps.Outbox.PostgreSql.Queries;
using ZionApps.Outbox.Relational;

namespace ZionApps.Outbox.PostgreSql
{
    internal class PostgreSqlEventStorage : IEventStorage
    {
        private readonly PostgreSqlOptions _options;
        private readonly string _schema;
        private readonly string _table;

        public PostgreSqlEventStorage(IOptions<PostgreSqlOptions> options)
        {
            _options = options.Value;
            _schema = _options.Schema;
            _table = "events";
        }

        public async Task InitializeAsync(CancellationToken cancel = default)
        {
            var sql = string.Format(SqlSource.CreateEventsTable, _schema, _table);

            await using var connection = CreateConnection();
            await connection.ExecuteNonQueryAsync(sql);
        }

        public async Task StoreEventAsync(Event ev, DbTransaction? transaction, CancellationToken cancel = default)
        {
            var sql = string.Format(SqlSource.InsertEvent, _schema, _table);

            object[] sqlParams =
            {
                new NpgsqlParameter("@EventKey", ev.EventKey),
                new NpgsqlParameter("@EventName", ev.EventName),
                new NpgsqlParameter("@IsSequential", ev.IsSequential),
                new NpgsqlParameter("@Payload", NpgsqlDbType.Jsonb) { Value = ev.Payload },
                new NpgsqlParameter("@AddedAtUtc", ev.AddedAtUtc),
                new NpgsqlParameter("@Headers", NpgsqlDbType.Jsonb) { Value = ev.Headers },
                new NpgsqlParameter("@Status", ev.Status.ToString("G")),
            };

            if (transaction == null)
            {
                await using var connection = CreateConnection();
                await connection.ExecuteNonQueryAsync(sql, sqlParams: sqlParams);
            }
            else
            {
                var connection = transaction.Connection;
                await connection.ExecuteNonQueryAsync(sql, transaction, sqlParams);
            }
        }

        public async Task UpdateEventStatusAsync(long eventId, EventStatus newStatus,
            CancellationToken cancel = default)
        {
            var sql = string.Format(SqlSource.UpdateEventStatus, _schema, _table);

            object[] sqlParams =
            {
                new NpgsqlParameter("@EventId", eventId),
                new NpgsqlParameter("@NewStatus", newStatus.ToString("G")),
            };

            await using var connection = CreateConnection();
            await connection.ExecuteNonQueryAsync(sql, sqlParams: sqlParams);
        }

        public Task<IReadOnlyCollection<Event>> GetNonSequentialEventsToDispatchAsync(CancellationToken cancel)
        {
            object[] sqlParams =
            {
                new NpgsqlParameter("@Limit", _options.FetchRowsLimit)
            };
            
            return GetEventsAsync(SqlSource.GetNonSequentialEventsToDispatch, sqlParams, cancel);
        }

        public Task<IReadOnlyCollection<Event>> GetSequentialEventsToDispatchAsync(string eventName,
            CancellationToken cancel)
        {
            object[] sqlParams =
            {
                new NpgsqlParameter("@EventName", eventName),
                new NpgsqlParameter("@Limit", _options.FetchRowsLimit),
            };

            return GetEventsAsync(SqlSource.GetSequentialEventsToDispatch, sqlParams, cancel);
        }

        private async Task<IReadOnlyCollection<Event>> GetEventsAsync(string query, object[] sqlParams,
            CancellationToken cancel)
        {
            // TODO Use Vladimir's sql command generator
            var sql = string.Format(query, _schema, _table);
            await using var connection = CreateConnection();

            return await connection
                .ExecuteReader(sql, sqlParams)
                .Select(reader => new Event
                (
                    id: reader.GetInt64(0),
                    eventKey: reader.GetString(1),
                    eventName: reader.GetString(2),
                    isSequential: reader.GetBoolean(3),
                    payload: reader.GetFieldValue<object>(4),
                    addedAtUtc: reader.GetDateTime(5),
                    headers: reader.GetFieldValue<Dictionary<string, string?>>(6),
                    status: Enum.Parse<EventStatus>(reader.GetString(7))
                ))
                .ToListAsync(cancel);
        }

        private NpgsqlConnection CreateConnection()
        {
            // TODO Connection factory?
            return new NpgsqlConnection(_options.ConnectionString);
        }
    }
}