using documentvaultapi.DAL;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.RabbitMQ.IRepositories;
using documentvaultapi.RabbitMQ.Models.MQueue;

namespace documentvaultapi.RabbitMQ.Repositories;

public class ConsumedAcknowledgementLogRepository : Repository<ConsumedAcknowledgementLog, DocumentVaultDbContext>, IConsumedAcknowledgementLogRepository
{
    readonly private ILogger _logger;
    readonly private DocumentVaultDbContext _dbContext;
    public ConsumedAcknowledgementLogRepository(DocumentVaultDbContext context, ILogger<ConsumedAcknowledgementLogRepository> logger) : base(context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<bool> InsertNewLog(ConsumedAcknowledgementLogModel consumedAcknowledgementLog)
    {
        try
        {
            var log = new ConsumedAcknowledgementLog()
            {
                UniqueId = Guid.NewGuid(),
                MessageId = consumedAcknowledgementLog.MessageId,
                PublishedMessageId = consumedAcknowledgementLog.PublishedMessageId,
                QueueName = consumedAcknowledgementLog.QueueName,
                ExchangeName = consumedAcknowledgementLog.ExchangeName,
                MessageBody = consumedAcknowledgementLog.MessageBody,
                QueueOptions = consumedAcknowledgementLog.QueueOptions,
                ConsumedAt = consumedAcknowledgementLog.ConsumedAt,
                Status = consumedAcknowledgementLog.Status,
                ErrorMessages = consumedAcknowledgementLog.ErrorMessages,
                ErrorType = consumedAcknowledgementLog.ErrorType,
            };
            await _dbContext.AddAsync(log);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting consumed acknowledgement log");
            throw;
        }
    }
}