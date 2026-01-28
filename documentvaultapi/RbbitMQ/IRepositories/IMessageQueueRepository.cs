using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;

namespace documentvaultapi.RabbitMQ.IRepositories
{
    public interface IMessageQueueRepository:IRepository<MessageQueue>
    {
        Task<IEnumerable<MessageQueue>> GetRecordsForQueueAsync(string queueName);
        Task InsertLogAsync(MessageQueue record, string queueName);
        Task RemoveRecordAsync(Guid uniqueId);
    }
}
