using RabbitMQ.Client;

namespace documentvaultapi.RbbitMQ
{
    public interface IRabbitMQConnectionFactory
    {
        Task<IConnection> CreateConnectionAsync(string hostKey, CancellationToken cancellationToken = default);
        Task<IChannel> CreateChannelAsync(string hostKey, CancellationToken cancellationToken = default);
    }
}
