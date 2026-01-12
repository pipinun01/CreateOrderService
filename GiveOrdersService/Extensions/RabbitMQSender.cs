using RabbitMQ.Client;
using Shared;
using System.Buffers.Binary;
using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace GiveOrdersService.Extensions
{
   
    public interface IRabbitMQSender
    {
        Task PublishMessageAsync(string queueName, Orders orders);
    }
    public class RabbitMQSender: IRabbitMQSender
    {
        private ConnectionFactory factory = new ConnectionFactory();
        IConfiguration _configuration;
        private Lazy<Task<IConnection>> _connection;
        public RabbitMQSender(IConfiguration configuration)
        {
            _configuration = configuration;
            var config = _configuration.GetSection("MessageBroker");
            factory.HostName = config["Host"];
            factory.UserName = config["Username"];
            factory.Password = config["Password"];

            //using var connection = await factory.CreateConnectionAsync();
            _connection = new Lazy<Task<IConnection>>(() => factory.CreateConnectionAsync());
        }
        public async Task PublishMessageAsync(string queueName, Orders orders)
        {
            //var factory = await Initialize();
            var connection = await _connection.Value;
            var channelOPtions = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);
            using var channel = await connection.CreateChannelAsync(channelOPtions);
            await channel.ExchangeDeclareAsync(exchange: "order_created", type: ExchangeType.Topic);

            ///QueueDeclareAsync - не нужен так как очредь создается в consumere при старте приложения
            //await channel.QueueDeclareAsync(queue: queueName,
            //                             durable: true,
            //                             exclusive: false,
            //                             autoDelete: false,
            //                             arguments: null);

            #region Notification about confirmed messages by RabbitMQ
            //var sequenceNumber = await channel.GetNextPublishSequenceNumberAsync();
            //channel.BasicReturnAsync += (sender, ea) =>
            //{
            //    ulong sequenceNumber = 0;

            //    IReadOnlyBasicProperties props = ea.BasicProperties;
            //    if (props.Headers is not null)
            //    {
            //        object? maybeSeqNum = props.Headers[Constants.PublishSequenceNumberHeader];
            //        if (maybeSeqNum is not null)
            //        {
            //            sequenceNumber = BinaryPrimitives.ReadUInt64BigEndian((byte[])maybeSeqNum);
            //        }
            //    }

            //    Console.WriteLine($"{DateTime.Now} [WARNING] message sequence number {sequenceNumber} has been basic.return-ed");
            //    return Task.CompletedTask;
            //};
            //channel.BasicAcksAsync += (sender, ea) =>
            //{
            //    Console.WriteLine($"{DateTime.Now} [INFO] message sequence number {ea.DeliveryTag} has been ACK-ed by broker. sequenceNumber: {sequenceNumber}");
            //    return Task.CompletedTask;
            //};
            //channel.BasicNacksAsync += (sender, ea) =>
            //{
            //    Console.WriteLine($"{DateTime.Now} [ERROR] message sequence number {ea.DeliveryTag} has been NACK-ed by broker");
            //    return Task.CompletedTask;
            //};
            #endregion
            string routingKey = "order.create";
            var message = JsonSerializer.Serialize(orders);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "order_created",
                                         routingKey: routingKey,
                                         body: body);
        }
    }
}
