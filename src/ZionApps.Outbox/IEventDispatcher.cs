using System.Threading;
using System.Threading.Tasks;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    /// <summary>
    /// Пушит сообщение в брокер
    /// </summary>
    public interface IEventDispatcher
    {
        Task DispatchAsync(Event ev, CancellationToken cancel = default);
    }
}