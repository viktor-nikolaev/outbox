using System;
using System.Collections.Generic;

namespace ZionApps.Outbox.Models
{
    public class Event
    {
        public Event(string? eventKey, string eventName, bool isSequential, object payload,
            Dictionary<string, string?>? headers = null)
        {
            EventKey = eventKey;
            EventName = eventName;
            IsSequential = isSequential;
            Payload = payload;
            AddedAtUtc = DateTime.UtcNow;
            Headers = headers ?? new Dictionary<string, string?>();
            Status = EventStatus.Scheduled;
        }

        public Event(long id, string? eventKey, string eventName, bool isSequential, object payload,
            DateTime addedAtUtc, Dictionary<string, string?> headers, EventStatus status)
        {
            Id = id;
            EventKey = eventKey;
            EventName = eventName;
            IsSequential = isSequential;
            Payload = payload;
            AddedAtUtc = addedAtUtc;
            Headers = headers;
            Status = status;
        }

        public long Id { get; set; }
        public string? EventKey { get; set; }
        public string EventName { get; set; }
        public bool IsSequential { get; set; }
        public object Payload { get; set; }
        public DateTime AddedAtUtc { get; set; }
        public Dictionary<string, string?> Headers { get; set; }
        public EventStatus Status { get; set; }
    }
}