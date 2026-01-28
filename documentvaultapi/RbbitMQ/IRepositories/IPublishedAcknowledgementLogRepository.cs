using documentvaultapi.Models.MQueue;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;

namespace documentvaultapi.RabbitMQ.IRepositories;

public interface IPublishedAcknowledgementLogRepository : IRepository<PublishedAcknowledgementLog>
{
    Task<bool> InsertNewLog(PublishedAcknowledgementLogModel publishedAcknowledgementLog);
}