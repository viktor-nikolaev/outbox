using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox.Kafka
{
    internal class KafkaTransport : IEventTransport, IDisposable
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly Lazy<IProducer<string, byte[]>> _lazyKafka;
        private readonly ILogger<KafkaTransport> _logger;

        public KafkaTransport(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaTransport> logger)
        {
            _logger = logger;
            _kafkaOptions = kafkaOptions.Value;

            var builder = new ProducerBuilder<string, byte[]>(kafkaOptions.Value.ProducerConfig);

            // TODO Handle errors if we cannot build an IProducer
            _lazyKafka = new Lazy<IProducer<string, byte[]>>(() => builder.Build());
        }

        private IProducer<string, byte[]> Kafka => _lazyKafka.Value;

        public void Dispose()
        {
            Kafka.Flush();
            Kafka.Dispose();
        }

        public async Task<SendEventResult> SendAsync(TransportEvent ev, CancellationToken cancel = default)
        {
            var headers = new Headers();
            foreach (var (key, value) in ev.Headers)
            {
                headers.Add(value != null
                    ? new Header(key, _kafkaOptions.MessageEncoding.GetBytes(value))
                    : new Header(key, null));
            }

            var message = new Message<string, byte[]>
            {
                Key = ev.EventKey,
                // TODO Move serialization to the upper level
                Value = _kafkaOptions.MessageEncoding.GetBytes(ev.Payload),
                Headers = headers,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            var logger = _logger.WithProperty("event", ev);

            try
            {
                var result = await Kafka.ProduceAsync(ev.TopicName, message, cancel);

                if (result.Status is PersistenceStatus.NotPersisted)
                {
                    return new SendEventResult("Kafka did not persist the message");
                }

                return SendEventResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error sending an event to kafka");
                return new SendEventResult(e);
            }
        }
    }
}