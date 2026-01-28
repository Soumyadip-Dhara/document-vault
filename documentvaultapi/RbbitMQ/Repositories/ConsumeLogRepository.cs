using AutoMapper;
using documentvaultapi.DAL;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories;
using documentvaultapi.Models.MQueue;
using documentvaultapi.RabbitMQ.IRepositories;
using NPOI.SS.Formula.Functions;

namespace documentvaultapi.RabbitMQ.Repositories
{
    public class ConsumeLogRepository : Repository<ConsumeLog, DocumentVaultDbContext>, IConsumeLogRepository
    {
        private readonly DocumentVaultDbContext _dbContext;
        private readonly IMapper _mapper;

        public ConsumeLogRepository(DocumentVaultDbContext context, IMapper mapper) : base(context)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public async Task InsertNewLog(NewConsumeLogModel newConsumeLog)
        {
            ConsumeLog consumeLog = _mapper.Map<ConsumeLog>(newConsumeLog);
            await _dbContext.ConsumeLogs.AddAsync(consumeLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
