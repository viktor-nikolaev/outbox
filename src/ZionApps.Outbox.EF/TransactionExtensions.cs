using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ZionApps.Outbox.EF
{
    public static class TransactionExtensions
    {
        /// <summary>
        ///     Start the CAP transaction
        /// </summary>
        /// <param name="database">The <see cref="DatabaseFacade" />.</param>
        /// <param name="outbox">The <see cref="IOutbox" />.</param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel" />.</param>
        /// <returns>The <see cref="IDbContextTransaction" /> of EF dbcontext transaction object.</returns>
        public static async Task<IDbContextTransaction> BeginTransactionAsync(this DatabaseFacade database,
            IOutbox outbox,
            IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var dbContextTransaction = await database.BeginTransactionAsync(isolationLevel);
            var transaction = dbContextTransaction.GetDbTransaction();
            
            outbox.SetCurrentTransaction(transaction);

            return dbContextTransaction;
        }
    }
}