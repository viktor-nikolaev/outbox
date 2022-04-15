using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ZionApps.Outbox.Helpers;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    internal class Outbox : IOutbox
    {
        private readonly IEventStorage _storage;
        private readonly OutboxOptions _options;
        private readonly AsyncLocal<DbTransaction> _transaction;

        public Outbox(IEventStorage storage, IOptions<OutboxOptions> options)
        {
            _storage = storage;
            _transaction = new AsyncLocal<DbTransaction>();
            _options = options.Value;
        }

        public async Task PublishAsync<T>(string eventName, T payload,
            string? eventKey, Dictionary<string, string?>? headers = null) where T : notnull
        {
            var key = eventKey ?? payload.GetIdentityValue()?.ToString();
            var isTopicSequential = _options.IsEventSequential(eventName);

            var ev = new Event
            (
                key,
                eventName,
                isTopicSequential,
                payload,
                headers
            );

            await _storage.StoreEventAsync(ev, _transaction.Value);
        }

        void IOutbox.SetCurrentTransaction(DbTransaction transaction)
        {
            _transaction.Value = transaction;
        }
    }
}