using GiveOrdersService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace GiveOrdersService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController
    {
        private readonly IRabbitMQSender _rabbitMQSender;
        public OrdersController(IRabbitMQSender rabbitMQSender)
        {
            _rabbitMQSender = rabbitMQSender;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Orders orders)
        {
            await _rabbitMQSender.PublishMessageAsync("order_created", orders);
            return new OkResult();

        }
    }
}
