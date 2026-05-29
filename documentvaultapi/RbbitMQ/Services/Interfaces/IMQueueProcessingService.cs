namespace documentvaultapi.RbbitMQ.Services.Interfaces
{
    public interface IMQueueProcessingService
    {
        Task ProcessQueueAsync(string queueName, string? correlationId = "");
    }
}
