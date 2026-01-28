using documentvaultapi.DAL;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.Models.MQueue;
using documentvaultapi.RabbitMQ.IRepositories;

namespace documentvaultapi.RabbitMQ.Repositories;

public class PublishedAcknowledgementLogRepository : Repository<PublishedAcknowledgementLog, DocumentVaultDbContext>, IPublishedAcknowledgementLogRepository
{
    private readonly DocumentVaultDbContext _dbContext;
    private readonly ILogger _logger;
    public PublishedAcknowledgementLogRepository(DocumentVaultDbContext context, ILogger<PublishedAcknowledgementLogRepository> logger) : base(context)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<bool> InsertNewLog(PublishedAcknowledgementLogModel publishedAcknowledgementLog)
    {
        try
        {
            var log = new PublishedAcknowledgementLog()
            {
                UniqueId = publishedAcknowledgementLog.UniqueId,
                ConsumeMessageId = publishedAcknowledgementLog.MessageId,
                QueueName = publishedAcknowledgementLog.QueueName,
                ExchangeName = publishedAcknowledgementLog.ExchangeName,
                MessageBody = publishedAcknowledgementLog.MessageBody,
                QueueOptions = publishedAcknowledgementLog.QueueOptions,
                PublishAt = publishedAcknowledgementLog.PublishAt,
            };
            await _dbContext.AddAsync(log);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting published acknowledgement log");
            throw;
        }
    }
}