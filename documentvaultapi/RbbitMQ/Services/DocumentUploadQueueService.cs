using AutoMapper;
using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.Entities;
using documentvaultapi.DAL.Repositories.Interfaces;
using documentvaultapi.Helper;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RbbitMQ.Models.MQueue;
using FluentValidation;
using FluentValidation.Results;
using Minio;
using Minio.DataModel.Args;
using RabbitMQ.Client;
using System.Security.Cryptography;

namespace documentvaultapi.RbbitMQ.Services
{
    public class DocumentUploadQueueService : IMessageProcessor<DocumentUploadMessageDTO>
    {
        private readonly ILogger<DocumentUploadQueueService> _logger;
        private readonly IValidator<DocumentUploadMessageDTO> _validator;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMinioClient _minioClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public DocumentUploadQueueService(
            ILogger<DocumentUploadQueueService> logger,
            IValidator<DocumentUploadMessageDTO> validator,
            IDocumentRepository documentRepository,
            IMinioClient minioClient,
            IConfiguration configuration,
            IMapper mapper)
        {
            _logger = logger;
            _validator = validator;
            _documentRepository = documentRepository;
            _minioClient = minioClient;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ValidationResult> ValidateMessage(DocumentUploadMessageDTO message)
        {
            return await _validator.ValidateAsync(message);
        }

        public async Task ProcessMessage(DocumentUploadMessageDTO message, IReadOnlyBasicProperties mqBasicProperties)
        {
            try
            {
                _logger.LogInformation($"Processing document upload message: {message.MessageId} for file: {message.FileName}");

                // Validate file extension
                var extension = Path.GetExtension(message.FileName);
                var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".pdf", ".jpg", ".jpeg", ".png", ".txt", ".doc", ".docx", ".xls", ".xlsx"
                };

                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception($"Invalid file type: {extension}");
                }

                // Validate content type
                var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "application/pdf",
                    "image/jpeg", "image/png",
                    "text/plain",
                    "application/msword",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "application/vnd.ms-excel",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };

                if (!allowedContentTypes.Contains(message.ContentType))
                {
                    throw new Exception($"Invalid content type: {message.ContentType}");
                }

                // Compute file hash
                string fileHash;
                using (var hashStream = new MemoryStream(message.FileContent))
                {
                    fileHash = ComputeSha256Hash(hashStream);
                }

                // Check for duplicates
                var existing = await _documentRepository.GetSingleAsync(d => d.FileHash == fileHash && d.IsActive);
                if (existing != null)
                {
                    _logger.LogWarning($"Duplicate document detected for message {message.MessageId}. Existing document ID: {existing.Id}");
                    throw new DuplicateDocumentException(existing.Id);
                }

                // Upload to MinIO
                var bucketName = _configuration["Minio:Bucket"]!;
                var objectName = $"{Guid.NewGuid()}_{message.FileName}";

                if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                }

                using (var stream = new MemoryStream(message.FileContent))
                {
                    await _minioClient.PutObjectAsync(
                        new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectName)
                            .WithStreamData(stream)
                            .WithObjectSize(message.FileSize)
                            .WithContentType(message.ContentType)
                    );
                }

                // Save document entity
                var entity = new Documents
                {
                    Id = Guid.NewGuid(),
                    BucketName = bucketName,
                    ObjectName = objectName,
                    OriginalFileName = message.FileName,
                    ContentType = message.ContentType,
                    FileSize = message.FileSize,
                    ApplicationId = message.ApplicationId,
                    CreatedBy = message.CreatedBy,
                    FileHash = fileHash,
                    IsActive = true
                };

                await _documentRepository.InsertAsync(entity);
                _logger.LogInformation($"Document uploaded successfully for message {message.MessageId}. Document ID: {entity.Id}");
            }
            catch (DuplicateDocumentException ex)
            {
                _logger.LogWarning($"Duplicate document error for message {message.MessageId}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing document upload message {message.MessageId}");
                throw;
            }
        }

        private static string ComputeSha256Hash(Stream stream)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
