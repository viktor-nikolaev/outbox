using System.Collections.Generic;

namespace ZionApps.Outbox.Models
{
    public class OutboxOptions
    {
        /// <summary>
        /// EventName for which we need to guarantee ordering
        /// </summary>
        public HashSet<string> SequentialEventNames { get; set; } = new();

        public int NoRowsDelayMs { get; set; } = 10_000;

        public int MaxEventRetries { get; set; } = 10;

        public int DelayBetweenRetriesMs { get; set; } = 1000;

        public Dictionary<string, string> EventName2TopicName { get; set; } = new();

        public bool IsEventSequential(string eventName)
        {
            return SequentialEventNames.Contains(eventName);
        }

        public string GetTopicNameByEventName(string eventName)
        {
            return EventName2TopicName.GetValueOrDefault(eventName) ?? eventName;
        }
        
        public OutboxOptions MapTopicNameToEventName(string eventName, string topicName)
        {
            EventName2TopicName[eventName] = topicName;
            return this;
        }
    }
}