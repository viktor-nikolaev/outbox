using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZionApps.Outbox.Models;

namespace ZionApps.Outbox
{
    public interface IEventTransport
    {
        Task<SendEventResult> SendAsync(TransportEvent ev, CancellationToken cancel = default);
    }

    public class TransportEvent
    {
        public TransportEvent(string eventKey, string topicName, string payload, Dictionary<string, string?> headers)
        {
            EventKey = eventKey;
            TopicName = topicName;
            Payload = payload;
            Headers = headers;
        }

        public string EventKey { get; set; }
        public string TopicName { get; set; }
        public string Payload { get; set; }
        public Dictionary<string, string?> Headers { get; set; }
    }
}