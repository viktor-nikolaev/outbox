using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ZionApps.Outbox
{
    public interface IOutbox
    {
        Task PublishAsync<T>(string eventName, T payload,
            string? eventKey = null, Dictionary<string, string?>? headers = null) where T : notnull;

        /// <summary>
        /// Not for external use
        /// </summary>
        /// <param name="transaction"></param>
        public void SetCurrentTransaction(DbTransaction transaction);
    }
}