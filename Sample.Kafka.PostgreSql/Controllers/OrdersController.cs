using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Kafka.PostgreSql.Model;
using ZionApps.Outbox;
using ZionApps.Outbox.EF;

namespace Sample.Kafka.PostgreSql.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly OrdersDbContext _orders;
        private readonly IOutbox _outbox;

        public OrdersController(ILogger<OrdersController> logger, OrdersDbContext orders, IOutbox outbox)
        {
            _logger = logger;
            _orders = orders;
            _outbox = outbox;
        }

        [HttpPost("without-transaction")]
        public async Task CreateOrderWithoutTransaction(Order order)
        {
            _orders.Orders.Add(order);

            var headers = new Dictionary<string, string?>
            {
                ["orderId"] = order.Id.ToString()
            };

            await _outbox.PublishAsync("orders", order, order.Id.ToString(), headers);
            await _orders.SaveChangesAsync();
        }

        [HttpPost("with-transaction")]
        public async Task CreateOrderWithEfTransaction(Order order)
        {
            await using var transaction = await _orders.Database.BeginTransactionAsync(_outbox);
            _orders.Orders.Add(order);

            var headers = new Dictionary<string, string?>
            {
                ["orderId"] = order.Id.ToString()
            };

            await _outbox.PublishAsync("orders", order, order.Id.ToString(), headers);
            await _orders.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
    }
}