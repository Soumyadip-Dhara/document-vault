using documentvaultapi.DAL;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.RabbitMQ.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace documentvaultapi.RabbitMQ.Repositories
{
    public class MessageQueueRepository :Repository<MessageQueue, DocumentVaultDbContext>,IMessageQueueRepository
    {
        private readonly DocumentVaultDbContext _dbContext;

        public MessageQueueRepository(DocumentVaultDbContext context) : base(context) 
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<MessageQueue>> GetRecordsForQueueAsync(string queueName)
        {
            return await _dbContext.MessageQueues
                .Where(r => r.QueueName == queueName)
                .ToListAsync();
        }

        public async Task InsertLogAsync(MessageQueue record, string queueName)
        {
            var log = new MessageQueueLog
            {
                UniqueId = record.UniqueId,
                ExchangeName = record.ExchangeName,
                QueueName = queueName,
                MessageBody = record.MessageBody,
                QueueOptions = record.QueueOptions,
                PublishAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local)
            };

            await _dbContext.MessageQueueLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveRecordAsync(Guid uniqueId)
        {
            var record = await _dbContext.MessageQueues.FindAsync(uniqueId);
            if (record != null)
            {
                _dbContext.MessageQueues.Remove(record);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
