using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;
using ZionApps.Outbox.EF;
using ZionApps.Outbox.Relational;

namespace ZionApps.Outbox.Tests
{
    public class OutboxTests
    {
        [Fact]
        public async Task Publish_NoTransaction_WithoutErrors()
        {
            IOutbox sut = new Outbox(null!, null!);

            var message = new SampleEvent();
            await sut.PublishAsync("topic-name", message);
        }

        [Fact]
        public async Task Publish_WithTransaction_WithoutErrors()
        {
            IOutbox sut = new Outbox(null!, null!);
            var message = new SampleEvent();

            await using var connection = new NpgsqlConnection();
            await using var transaction = await connection.BeginTransactionAsync(sut);

            await sut.PublishAsync("topic-name", message);

            await transaction.CommitAsync();
        }

        [Fact]
        public async Task Publish_WithEfCoreTransaction_WithoutErrors()
        {
            // ARRANGE
            IOutbox sut = new Outbox(null!, null!);
            AppDbContext dbContext = new();
            var message = new SampleEvent();

            // ACT
            await using var transaction = await dbContext.Database.BeginTransactionAsync(sut);
            dbContext.Persons.Add(new Person { Name = "ef.transaction" });

            await sut.PublishAsync("topic-name", message);

            await transaction.CommitAsync();
        }
    }

    public class SampleEvent
    {
        public int Id { get; set; }
    }
}