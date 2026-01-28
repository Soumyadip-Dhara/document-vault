using documentvaultapi.Models.MQueue;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;

namespace documentvaultapi.RabbitMQ.IRepositories
{
    public interface IConsumeLogRepository: IRepository<ConsumeLog>
    {
        Task InsertNewLog(NewConsumeLogModel newConsumeLog);
    }
}
