using documentvaultapi.Common.Constants;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RbbitMQ.Models.MQueue;
using documentvaultapi.RbbitMQ.Services;

namespace documentvaultapi.RbbitMQ.Consumers
{
    public class DocumentUploadConsumer : RabbitMQConsumerBase<DocumentUploadMessageDTO>
    {
        public DocumentUploadConsumer(
            ILogger<DocumentUploadQueueService> logger,
            IRabbitMQConnectionFactory connectionFactory,
            IServiceScopeFactory serviceScopeFactory)
            : base(
                logger,
                connectionFactory,
                serviceScopeFactory,
                MessageQueueConstants.DOCUMENT_UPLOAD_QUEUE)
        {
        }
    }
}
