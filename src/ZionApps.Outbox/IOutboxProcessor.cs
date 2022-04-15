using System.Threading;
using System.Threading.Tasks;

namespace ZionApps.Outbox
{
    public interface IOutboxProcessor
    {
        Task StartProcessingAsync(CancellationToken cancel);
    }
}