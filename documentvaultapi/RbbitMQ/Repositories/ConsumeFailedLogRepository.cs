using AutoMapper;
using documentvaultapi.DAL;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.Models.MQueue;
using documentvaultapi.RabbitMQ.IRepositories;
using NPOI.SS.Formula.Functions;

namespace documentvaultapi.RabbitMQ.Repositories
{
    public class ConsumeFailedLogRepository : Repository<ConsumeFailedLog, DocumentVaultDbContext>, IConsumeFailedLogRepository
    {
        private readonly DocumentVaultDbContext _dbContext;
        private readonly IMapper _mapper;

        public ConsumeFailedLogRepository(DocumentVaultDbContext context, IMapper mapper) : base(context)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public async Task InsertNewLog(NewConsumeLogModel newConsumeLog)
        {
            ConsumeFailedLog consumeLog = _mapper.Map<ConsumeFailedLog>(newConsumeLog);
            //ConsumeFailedLog consumeFailedLog = new ConsumeFailedLog
            //{
            //    MessageId = Guid.Parse(newConsumeLog.MessageId),
            //    QueueName = newConsumeLog.QueueName,
            //    ActionStatus = newConsumeLog.ActionStatus,
            //    ConsumedAt = newConsumeLog.ConsumedAt,
            //    FailedAt = newConsumeLog.FailedAt,
            //    ExchangeName = newConsumeLog.ExchangeName,
            //    FailedMessage = newConsumeLog.FailedMessage,
            //    FailedType = newConsumeLog.FailedType,
            //    RaoutingKey = newConsumeLog.RaoutingKey,
            //    MessageBody = newConsumeLog.MessageBody,
            //};
            await _dbContext.ConsumeFailedLogs.AddAsync(consumeLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
