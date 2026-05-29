using AutoMapper;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RbbitMQ.Models.MQueue.FromUM;
using FluentValidation;
using FluentValidation.Results;
using RabbitMQ.Client;

namespace documentvaultapi.RbbitMQ.Services.FromUM
{
    public class ApplicationMapQueueService : IMessageProcessor<ConsumeApplicationMapDTO>
    {
        private readonly ILogger<ApplicationMapQueueService> _logger;
        private readonly IValidator<ConsumeApplicationMapDTO> _validator;
        private readonly IApplicationMapRepository _applicationMapRepository;

        private readonly IMapper _mapper;

        public ApplicationMapQueueService
        (
            ILogger<ApplicationMapQueueService> logger,
            IValidator<ConsumeApplicationMapDTO> validator,
            IApplicationMapRepository applicationMapRepository,

            IMapper mapper
        )
        {
            _logger = logger;
            _validator = validator;
            _applicationMapRepository = applicationMapRepository;

            _mapper = mapper;
        }
        public async Task<ValidationResult> ValidateMessage(ConsumeApplicationMapDTO message)
        {
            return await _validator.ValidateAsync(message);
        }

        public async Task ProcessMessage(ConsumeApplicationMapDTO message, IReadOnlyBasicProperties mqBasicProperties)
        {
            //_logger.LogInformation($"");
            ApplicationMap appMap = _mapper.Map<ApplicationMap>(message);
            await _applicationMapRepository.SaveChangesAManaged(appMap);
            await Task.CompletedTask;
        }


    }
}

