using RabbitMQ.Client;

namespace documentvaultapi.RbbitMQ
{
    public interface IRabbitMQConnectionFactory
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
        Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    }
}
