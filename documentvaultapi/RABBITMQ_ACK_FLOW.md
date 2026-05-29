# RabbitMQ Document Upload ACK Flow Documentation

## Queue Setup

### Main Queues
- **Source Queue**: `documentstorage_document_upload1`
- **ACK Queue**: `documentstorage_document_upload1_ack`

## Message Flow

### Success Flow

```
1. Message published to: documentstorage_document_upload1
   ↓
2. DocumentUploadConsumer listens and receives message
   ↓
3. DocumentUploadQueueService processes:
   - Validates file extension (pdf, jpg, jpeg, png, txt, doc, docx, xls, xlsx)
   - Validates content type
   - Computes SHA256 hash
   - Checks for duplicates
   - Uploads to MinIO
   - Saves document entity to database
   ↓
4. On SUCCESS:
   - Log written to ConsumeLog table
   - PublishAckAsync() sends ACK message to: documentstorage_document_upload1_ack
   - Message contains: Status=SUCCESS, MessageId, Timestamp
   - RabbitMQ BasicAck sent (message removed from queue)
   ↓
5. RabbitMQAckConsumer listens to documentstorage_document_upload1_ack
   ↓
6. ACK message processed:
   - Log written to ConsumedAcknowledgementLog table
   - Marks message processing as complete
```

### Failure Flow

```
1. Message published to: documentstorage_document_upload1
   ↓
2. DocumentUploadConsumer listens and receives message
   ↓
3. DocumentUploadQueueService processing FAILS due to:
   - Invalid file extension/content type
   - Duplicate document detected
   - MinIO upload failure
   - Database error
   - Deserialization error
   - Validation failure
   ↓
4. On FAILURE:
   - Log written to ConsumeFailedLog table with:
     * FailedType (specific error category)
     * FailedMessage (error details)
     * FailedAt (timestamp)
   - PublishNackAsync() sends NACK message to: documentstorage_document_upload1_ack
   - Message contains: Status=FAILED, MessageId, FailedType, ErrorDetails
   - RabbitMQ BasicNack sent with requeue flag based on redelivery status
   ↓
5. RabbitMQAckConsumer listens to documentstorage_document_upload1_ack
   ↓
6. NACK message processed:
   - Log written to ConsumedAcknowledgementLog table with error details
   - If Status=FAILED, also logged to MessageQueueFailedLogs table
   - Marks message processing as failed with error tracking
```

## Database Tables Involved

### Message Consumption Logging
- **ConsumeLog**: Successful message consumption logs
- **ConsumeFailedLog**: Failed message consumption logs
- **ConsumeAcknowledgementLog**: Messages consumed from ACK queue (success/fail from publisher's perspective)

### Queue Management
- **MessageQueue**: Published message metadata
- **MessageQueueFailedLogs**: Failed messages tracked from ACK queue
- **PublishedAcknowledgementLog**: Acknowledgements sent by consumer

## Configuration

### In appsettings.json
```json
{
  "RabbitMQConnection": {
    "Host": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  },
  "Logging": {
    "MessageQueueLog": {
      "ErrorLogPath": "./consumeFailedLog"
    }
  },
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Bucket": "documents"
  }
}
```

## Key Code Flow

### Document Upload Consumer
- **File**: `RbbitMQ/Consumers/DocumentUploadConsumer.cs`
- Inherits from `RabbitMQConsumerBase<DocumentUploadMessageDTO>`
- Service: `DocumentUploadQueueService`
- Queue: `documentstorage_document_upload1`

### ACK Processing
- **Consumer**: `RabbitMQAckConsumer`
- **Hosted Service**: `RabbitMQAckConsumerHostedService`
- **Service**: `MQueueAckService`
- **Queues**: Registered in `RabbitMqRegiserExtensions.cs` `ackQueues` array

### Error Handling
- Invalid messages logged to local file system at: `./consumeFailedLog/{QueueName}_{MessageId}`
- Database transaction rollback on ACK processing failure
- Redelivery support for transient failures

## Verification Checklist

- [ ] Both `documentstorage_document_upload1` and `documentstorage_document_upload1_ack` queues exist in RabbitMQ
- [ ] DocumentUploadConsumer service is running
- [ ] RabbitMQAckConsumer is registered for `documentstorage_document_upload1_ack` queue
- [ ] Database tables exist: ConsumeLog, ConsumeFailedLog, ConsumedAcknowledgementLog, MessageQueueFailedLogs, PublishedAcknowledgementLog
- [ ] MinIO bucket is configured and accessible
- [ ] Message ID is provided in BasicProperties when publishing
- [ ] ACK/NACK messages arrive in documentstorage_document_upload1_ack queue
- [ ] ConsumedAcknowledgementLog table records all ACK messages
- [ ] On failure, MessageQueueFailedLogs table records failed status
- [ ] Local error logs are created in `./consumeFailedLog` for critical failures

## Testing Command

```powershell
# Publish test message to documentstorage_document_upload1 queue
# Verify:
# 1. Message is consumed by DocumentUploadConsumer
# 2. ACK message appears in documentstorage_document_upload1_ack queue
# 3. Logs appear in ConsumeLog or ConsumeFailedLog tables
# 4. ACK is processed by RabbitMQAckConsumer
# 5. Final log appears in ConsumedAcknowledgementLog table
```
