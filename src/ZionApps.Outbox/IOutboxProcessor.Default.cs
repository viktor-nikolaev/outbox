using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    internal class OutboxProcessor : IOutboxProcessor
    {
        private readonly IEventStorage _storage;
        private readonly IEventDispatcher _dispatcher;
        private readonly OutboxOptions _options;

        public OutboxProcessor(IEventStorage storage, IEventDispatcher dispatcher, IOptions<OutboxOptions> options)
        {
            _storage = storage;
            _dispatcher = dispatcher;
            _options = options.Value;
        }

        public async Task StartProcessingAsync(CancellationToken cancel)
        {
            await _storage.InitializeAsync(cancel);

            var processOrderedEventsTasks = _options
                .SequentialEventNames
                .Select(eventName =>
                {
                    if (eventName == null)
                    {
                        throw new ArgumentNullException(nameof(eventName));
                    }

                    return ProcessOrderedEvents(eventName, cancel);
                });

            var processUnorderedEventsTask = ProcessUnorderedEventsAsync(cancel);

            var allTasks = processOrderedEventsTasks.Concat(new[] { processUnorderedEventsTask });
            await Task.WhenAll(allTasks);
        }

        private async Task ProcessOrderedEvents(string eventName, CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                // TODO Handle exceptions
                var events = await _storage.GetSequentialEventsToDispatchAsync(eventName, cancel);

                if (events.Any())
                {
                    // Partitioning by EventKey
                    var tasks = events
                        .GroupBy(e => e.EventKey)
                        .Select(async partition =>
                        {
                            foreach (var ev in partition)
                            {
                                await _dispatcher.DispatchAsync(ev, cancel);
                            }
                        });

                    await Task.WhenAll(tasks);
                }
                else
                {
                    await Task.Delay(_options.NoRowsDelayMs, cancel);
                }
            }
        }

        private async Task ProcessUnorderedEventsAsync(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                var events = await _storage.GetNonSequentialEventsToDispatchAsync(cancel);

                if (events.Any())
                {
                    var tasks = events.Select(e => _dispatcher.DispatchAsync(e, cancel));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    await Task.Delay(_options.NoRowsDelayMs, cancel);
                }
            }
        }
    }
}