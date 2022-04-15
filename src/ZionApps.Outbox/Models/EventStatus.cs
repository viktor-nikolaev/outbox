namespace ZionApps.Outbox.Models
{
    public enum EventStatus
    {
        Failed = -1,
        Scheduled = 1,
        Succeeded = 2
    }
}