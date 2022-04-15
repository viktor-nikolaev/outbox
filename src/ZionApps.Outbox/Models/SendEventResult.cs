using System;

namespace ZionApps.Outbox.Models
{
    public class SendEventResult
    {
        public static readonly SendEventResult Success = new();

        public SendEventResult(string? errorMessage = null, Exception? exception = null)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public SendEventResult(Exception? exception) : this(null, exception)
        {
        }

        public string? ErrorMessage { get; }
        public Exception? Exception { get; }
        public bool IsSuccess => ErrorMessage is null && Exception is null;
    }
}