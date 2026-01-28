using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace documentvaultapi.RbbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;

        public RabbitMqService(IConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQConnection:Host"],
                Port = int.Parse(_configuration["RabbitMQConnection:Port"]),
                UserName = _configuration["RabbitMQConnection:UserName"],
                Password = _configuration["RabbitMQConnection:Password"],
            };
        }
        public async Task PublishAsync<T>(string routingKey, T message, string exchange = "") where T : class
        {
            await Task.Run(async () =>
            {
                using (var connection = await _factory.CreateConnectionAsync())
                using (var channel = await connection.CreateChannelAsync())
                {
                    await channel.QueueDeclareAsync(queue: routingKey,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey, body: messageBody);
                }
            });
        }

        public async Task<string> GetPayloadRabbitMQAsync<T>(string queueName)
        {
            using (var connection = await _factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Declare the queue (ensure it exists)
                await channel.QueueDeclareAsync(queue: queueName,
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                // Use BasicGet to get a message from the queue (non-blocking)
                var result = await channel.BasicGetAsync(queue: queueName, autoAck: true);

                if (result == null)
                {
                    return null; // No message in the queue
                }

                var body = result.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                return message; // Return the message
            }
        }

        public async Task PublishAsync<T>(string routingKey, T message, string queueId, string exchange = "") where T : class
        {
            await Task.Run(async () =>
            {
                using (var connection = await _factory.CreateConnectionAsync())
                using (var channel = await connection.CreateChannelAsync())
                {
                    var properties = new BasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("queue-id", queueId);
                    await channel.QueueDeclareAsync(queue: routingKey,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey, mandatory: true, basicProperties: properties, body: messageBody);
                }
            });
        }
        public async Task<string> PushMessageAsync(string queueName, string message, string queueId, string exchange = "")
        {

            using (IConnection connection = await _factory.CreateConnectionAsync())
            {
                using IChannel channel = await connection.CreateChannelAsync();
                var properties = new BasicProperties();
                properties.MessageId = queueId;
                await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync(exchange: exchange, routingKey: queueName, mandatory: true, basicProperties: properties, body: messageBytes);
                //Console.WriteLine(" [x] Sent '" + message + "'");
            }

            return "Message published to the queue successfully.";
        }
    }
}