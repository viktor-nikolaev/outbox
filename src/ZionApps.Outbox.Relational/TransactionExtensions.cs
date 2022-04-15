using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ZionApps.Outbox.Relational
{
    public static class TransactionExtensions
    {
        /// <summary>
        ///     Start the outbox transaction
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" />.</param>
        /// <param name="outbox">The <see cref="IOutbox" />.</param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel" />.</param>
        /// <returns>The <see cref="IDbTransaction" /> object.</returns>
        public static async Task<DbTransaction> BeginTransactionAsync(this DbConnection connection, IOutbox outbox,
            IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var transaction = await connection.BeginTransactionAsync(isolationLevel);
            outbox.SetCurrentTransaction(transaction);

            return transaction;
        }

        public static DbTransaction WithOutbox(this DbTransaction transaction, IOutbox outbox)
        {
            outbox.SetCurrentTransaction(transaction);
            return transaction;
        }
    }
}