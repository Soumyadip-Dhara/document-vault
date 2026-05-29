using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.RabbitMQ.Models.MQueue;


namespace documentvaultapi.RabbitMQ.IRepositories;

public interface IConsumedAcknowledgementLogRepository : IRepository<ConsumedAcknowledgementLog>
{
    Task<bool> InsertNewLog(ConsumedAcknowledgementLogModel consumedAcknowledgementLog);
}