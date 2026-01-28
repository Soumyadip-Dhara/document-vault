using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.RbbitMQ;

namespace documentvaultapi.RabbitMQ.IRepositories
{
    public interface IMessageQueueFailedLogsRepository : IRepository<MessageQueueFailedLog>
    {
        Task InsertLogAsync(AckPayloadModel record);
    }
}
