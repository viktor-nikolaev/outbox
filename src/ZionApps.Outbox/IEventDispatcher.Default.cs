using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly OutboxOptions _options;
        private readonly IEventStorage _storage;
        private readonly IEventTransport _transport;

        public EventDispatcher(IEventTransport transport, IEventStorage storage, IOptions<OutboxOptions> options,
            ILogger<EventDispatcher> logger)
        {
            _transport = transport;
            _storage = storage;
            _logger = logger;
            _options = options.Value;
        }

        public async Task DispatchAsync(Event ev, CancellationToken cancel = default)
        {
            await SendEvent(ev, cancel);
            await SaveEventStatus(ev);
        }

        private async Task SaveEventStatus(Event ev)
        {
            // TODO Retry logic
            await _storage.UpdateEventStatusAsync(ev.Id, ev.Status);
        }

        private async Task SendEvent(Event ev, CancellationToken cancel)
        {
            var logger = _logger.WithProperty("event", ev);
            var transportEvent = new TransportEvent
            (
                ev.EventKey ?? ev.Id.ToString(),
                _options.GetTopicNameByEventName(ev.EventName),
                // TODO abstract away from serialization logic
                JsonSerializer.Serialize(ev.Payload),
                ev.Headers
            );

            // TODO Implement more clever retry logic
            var retry = 1;
            while (!cancel.IsCancellationRequested)
            {
                
                var result = await _transport.SendAsync(transportEvent, cancel);
                if (result.IsSuccess)
                {
                    ev.Status = EventStatus.Succeeded;
                    return;
                }

                var exceededRetries = retry++ >= _options.MaxEventRetries;
                if (exceededRetries)
                {
                    logger.LogError("Error dispatching event to the broker, stop retrying");
                    ev.Status = EventStatus.Failed;
                    return;
                }

                logger.WithProperty("retry", retry)
                    .LogWarning("Error dispatching event to the broker, retrying");

                await Task.Delay(_options.DelayBetweenRetriesMs, cancel);
            }

            logger.LogWarning("Sending canceled, the event will not be dispatched");
        }
    }
}