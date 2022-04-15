using System.Text;
using Confluent.Kafka;

namespace ZionApps.Outbox.Kafka
{
    /// <summary>
    ///     Параметры работы продьюсера сообщений
    /// </summary>
    public class KafkaOptions
    {
        /// <summary>
        ///     Конфигурация
        /// </summary>
        public ProducerConfig ProducerConfig { get; set; } = new()
        {
            BatchNumMessages = 100,
            LingerMs = 1000
        };

        /// <summary>
        ///     Кодировка сообщений
        /// </summary>
        public Encoding MessageEncoding { get; set; } = Encoding.UTF8;
    }
}