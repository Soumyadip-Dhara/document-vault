using AutoMapper;
using documentvaultapi.Enum;
using documentvaultapi.RabbitMQ.IRepositories;
using documentvaultapi.RbbitMQ;
using FluentValidation;
using FluentValidation.Results;
using RabbitMQ.Client;

namespace documentvaultapi.RbbitMQ.Services
{
    public class MQueueAckService : IMessageProcessor<AckPayloadModel>
    {
        private readonly ILogger<MQueueAckService> _logger;
        private readonly IValidator<AckPayloadModel> _validator;
        private readonly IMessageQueueFailedLogsRepository _messageQueueFailedLogsRepository;

        public MQueueAckService
        (
            ILogger<MQueueAckService> logger,
            IValidator<AckPayloadModel> validator,
            IMessageQueueFailedLogsRepository messageQueueFailedLogsRepository,
            IMapper mapper
        )
        {
            _logger = logger;
            _validator = validator;
            _messageQueueFailedLogsRepository = messageQueueFailedLogsRepository;
        }
        public async Task<ValidationResult> ValidateMessage(AckPayloadModel message)
        {
            return await _validator.ValidateAsync(message);
        }

        public async Task ProcessMessage(AckPayloadModel message, IReadOnlyBasicProperties mqBasicProperties)
        {
            if (message.Status == ConsumeStatusEnums.FAILED)
            {
                await _messageQueueFailedLogsRepository.InsertLogAsync(message);
            }
            await Task.CompletedTask;
        }
    }
}
