using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    /// <summary>
    /// Хранит события в базе
    /// </summary>
    public interface IEventStorage
    {
        Task InitializeAsync(CancellationToken cancel = default);
        Task StoreEventAsync(Event ev, DbTransaction? transaction = null, CancellationToken cancel = default);
        Task UpdateEventStatusAsync(long eventId, EventStatus newStatus, CancellationToken cancel = default);
        Task<IReadOnlyCollection<Event>> GetNonSequentialEventsToDispatchAsync(CancellationToken cancel = default);
        Task<IReadOnlyCollection<Event>> GetSequentialEventsToDispatchAsync(string eventName, CancellationToken cancel = default);
    }
}