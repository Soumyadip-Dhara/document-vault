using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.Models.MQueue;

namespace documentvaultapi.RabbitMQ.IRepositories
{
    public interface IConsumeFailedLogRepository : IRepository<ConsumeFailedLog>
    {
        Task InsertNewLog(NewConsumeLogModel newConsumeLog);
    }
}
