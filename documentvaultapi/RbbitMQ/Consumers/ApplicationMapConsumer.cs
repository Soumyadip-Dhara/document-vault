//namespace documentvaultapi.RbbitMQ.Consumers
//{
//    public class ApplicationMapConsumer
//    {
//    }
//}


using documentvaultapi.Common.Constants;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RbbitMQ.Models.MQueue.FromUM;
using documentvaultapi.RbbitMQ.Services.FromUM;

namespace documentvaultapi.RbbitMQ.Consumers
{
    public class ApplicationMapConsumer : RabbitMQConsumerBase<ConsumeApplicationMapDTO>
    {
        public ApplicationMapConsumer
        (
            ILogger<ApplicationMapQueueService> logger,
            IRabbitMQConnectionFactory connectionFactory,
            IServiceScopeFactory serviceScopeFactory
        // IConfiguration configuration,
        // IMessageErrorLogger errorLogger
        )
        : base
        (
           logger,
           connectionFactory,
           serviceScopeFactory,
            // configuration, 
            // errorLogger, 
            MessageQueueConstants.UM_APPLICATION_MAP
        )
        {
        }
    }
}

