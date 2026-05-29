# Document Upload RabbitMQ ACK Verification Guide

## ✅ Implementation Summary

Your RabbitMQ setup is now configured to properly handle message consumption and acknowledgements for the document upload queue. Here's what has been set up:

### Key Components Configured

#### 1. **Message Consumer Chain**
- ✅ `DocumentUploadConsumer` - listens on `documentstorage_document_upload1` queue
- ✅ `DocumentUploadQueueService` - processes document uploads with validation and MinIO storage
- ✅ `RabbitMQConsumerBase` - handles ACK/NACK publishing for success/failure scenarios
- ✅ `RabbitMQAckConsumer` - **NOW REGISTERED** - listens on `documentstorage_document_upload1_ack` queue
- ✅ `MQueueAckService` - processes acknowledgement messages

#### 2. **Queue Configuration**
```
documentstorage_document_upload1        → Main queue for document uploads
documentstorage_document_upload1_ack    → ACK/NACK queue (now monitored)
```

#### 3. **Database Tracking**
- ✅ `ConsumeLog` - logs successful message consumption
- ✅ `ConsumeFailedLog` - logs failed message consumption with error details
- ✅ `PublishedAcknowledgementLog` - logs all ACK messages sent
- ✅ `ConsumedAcknowledgementLog` - logs all ACK messages received and processed
- ✅ `MessageQueueFailedLogs` - tracks failed messages from ACK queue

---

## Testing Checklist

### Pre-Flight Checks
- [ ] RabbitMQ server is running and accessible
- [ ] PostgreSQL database is running
- [ ] MinIO storage is configured and running
- [ ] Application is built and running without errors
- [ ] Connection strings in `appsettings.json` are correct

### Queue Setup Verification
```powershell
# In RabbitMQ Management UI (http://localhost:15672)
# 1. Check Queues tab
# [ ] Queue "documentstorage_document_upload1" exists (Durable)
# [ ] Queue "documentstorage_document_upload1_ack" exists (Durable)
# [ ] Both have 0 messages initially
```

### Success Path Test (✅ Success + ACK)

1. **Publish Test Message**
   - Send a valid document upload message to `documentstorage_document_upload1`
   - Include required fields: MessageId (GUID), FileName, ContentType, FileContent, FileSize, ApplicationId, CreatedBy
   - Example content type: `application/pdf`

2. **Monitor Consumption**
   - Wait 2-3 seconds for consumer to process
   - Check `ConsumeLog` table: Should have 1 row with Status='SUCCESS'
   - Verify `PublishedAcknowledgementLog` table: Should have 1 row (ACK message sent)

3. **Monitor ACK Reception**
   - Wait 2-3 seconds for ACK consumer to process
   - Check `ConsumedAcknowledgementLog` table: Should have 1 row with Status='SUCCESS'
   - MinIO bucket should contain the uploaded file

4. **Queue Status**
   - Both queues should have 0 messages (messages processed and ACK'd)
   - Application logs should show no errors

### Failure Path Test (❌ Failure + NACK)

1. **Publish Invalid Message**
   - Send document with invalid file extension (e.g., `.exe`)
   - OR send document with invalid content type
   - OR send message with invalid MessageId (not a GUID)

2. **Monitor Failure Handling**
   - Check `ConsumeFailedLog` table: Should have 1 row with FailedType and FailedMessage
   - Verify `PublishedAcknowledgementLog` table: Should have NACK entry (isSuccess=false)

3. **Monitor NACK Reception**
   - Check `ConsumedAcknowledgementLog` table: Should have 1 row with Status='ERROR'
   - Verify `MessageQueueFailedLogs` table: Should record the failed ACK

4. **Error Logging**
   - Check `./consumeFailedLog` directory: Should contain error logs with timestamp

### Redelivery Path Test (⚠️ Transient Failure)

1. **Simulate Transient Failure**
   - Temporarily stop MinIO or database
   - Send valid document message
   - Monitor ConsumeFailedLog: Should record PENDING status

2. **Recovery**
   - Restart MinIO/Database
   - Consumer should retry the message (Redelivered flag = true)
   - Eventually succeeds and sends ACK

---

## Key Configuration Files

### RabbitMQ Registration
**File**: `RbbitMQ/Extensions/RabbitMqRegiserExtensions.cs`

```csharp
// ✅ UPDATED: DOCUMENT_UPLOAD_ACK now included
var ackQueues = new[]
{
    MessageQueueConstants.UM_APPLICATION_MAP_ACK,
    MessageQueueConstants.DOCUMENT_UPLOAD_ACK  // ← NEW
};
```

### Message Constants
**File**: `Common/Constants/MessageQueueConstants.cs`

```csharp
public const string DOCUMENT_UPLOAD_QUEUE = "documentstorage_document_upload1";
public const string DOCUMENT_UPLOAD_ACK = "documentstorage_document_upload1_ack";
```

---

## Success Indicators

### In Logs
```
✅ Processing message. Redelivered: False
✅ Processing document upload message: [MessageId] for file: [FileName]
✅ Document uploaded successfully for message [MessageId]. Document ID: [DocumentId]
```

### In Database

**ConsumeLog (Success)**
| MessageId | Status | QueueName | ConsumedAt |
|-----------|--------|-----------|-----------|
| [GUID] | SUCCESS | documentstorage_document_upload1 | [Timestamp] |

**PublishedAcknowledgementLog**
| UniqueId | MessageId | QueueName | Status |
|----------|-----------|-----------|--------|
| [GUID] | [GUID] | documentstorage_document_upload1_ack | SUCCESS |

**ConsumedAcknowledgementLog**
| MessageId | PublishedMessageId | QueueName | Status |
|-----------|-------------------|-----------|--------|
| [GUID] | [GUID] | documentstorage_document_upload1_ack | SUCCESS |

---

## Troubleshooting

### Issue: ACK message not appearing in documentstorage_document_upload1_ack queue
- **Check**: Is `PublishedAcknowledgementLog` table being populated?
- **Fix**: Verify RabbitMQConsumerBase's `PublishAcknowledgementAsync()` is being called
- **Verify**: Check if RabbitMQ connection is working

### Issue: RabbitMQAckConsumer not processing messages
- **Check**: Is the hosted service registered?
- **Fix**: Verify `DOCUMENT_UPLOAD_ACK` is in `ackQueues` array ✅ (DONE)
- **Verify**: Check application startup logs for hosted service initialization

### Issue: Document upload succeeds but ACK fails
- **Check**: Is database transaction rolling back?
- **Verify**: Check `ConsumedAcknowledgementLog` table for error entries
- **Fix**: Check error logs in `./consumeFailedLog/ConsumeAck/` directory

### Issue: Duplicate detection not working
- **Check**: Verify SHA256 hash computation is correct
- **Fix**: Check if `DuplicateDocumentException` is being thrown properly

---

## Performance Monitoring

### Recommended Monitoring

1. **Queue Depth**
   - Monitor queue message count in RabbitMQ
   - Alert if backlog exceeds threshold

2. **Processing Time**
   - Track `ConsumedAt - PublishedAt` duration
   - Alert if average exceeds 5 seconds

3. **Failure Rate**
   - Monitor ConsumeFailedLog growth
   - Alert if failure percentage exceeds 5%

4. **ACK Reception**
   - Verify ConsumedAcknowledgementLog keeps up with PublishedAcknowledgementLog
   - Alert if lag exceeds 10 minutes

---

## Next Steps

1. ✅ Verify all components are running
2. ✅ Run success path test
3. ✅ Run failure path test
4. ✅ Monitor logs and database tables
5. ✅ Set up alerts for queue depth and failure rates
6. ✅ Document any custom error handling needed
