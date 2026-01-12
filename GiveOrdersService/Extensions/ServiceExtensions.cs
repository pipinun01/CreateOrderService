using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace GiveOrdersService.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureRabbitMQ (this IServiceCollection services)=> services.AddSingleton<IRabbitMQSender, RabbitMQSender>();
    }
}
