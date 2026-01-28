using AutoMapper;
using documentvaultapi.DAL.Entities;
using documentvaultapi.RabbitMQ.IRepositories;
using documentvaultapi.Enum;
using documentvaultapi.RbbitMQ;
using Microsoft.EntityFrameworkCore;
using documentvaultapi.DAL;
using documentvaultapi.DAL.Repositories;
namespace documentvaultapi.RabbitMQ.Repositories
{
    public class MessageQueueFailedLogsRepository : Repository<MessageQueueFailedLog, DocumentVaultDbContext>, IMessageQueueFailedLogsRepository
    {
        private readonly DocumentVaultDbContext _dbContext;
        private readonly IMapper _mapper;

        public MessageQueueFailedLogsRepository(DocumentVaultDbContext context, IMapper mapper) : base(context)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task InsertLogAsync(AckPayloadModel ackPayload)
        {
            //MessageQueueFailedLog queueFailedLog = _mapper.Map<MessageQueueFailedLog>(ackPayload);
            var queueFailedLog = new MessageQueueFailedLog
            {
                UniqueId = Guid.NewGuid(),
                MessageId = ackPayload.MessageId,
                ActionStatus = ConsumeStatusEnums.PENDING,
                FailedType = ackPayload.FailedType,
                FailedAt = ackPayload.Timestamp.ToUniversalTime(),
                FailedMessage = ackPayload.StatusMsg
            };

            await _dbContext.MessageQueueFailedLogs.AddAsync(queueFailedLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
