using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.Helper;
using documentvaultapi.Enum;
using Microsoft.AspNetCore.Mvc;
using documentvaultapi.Filters;
using documentvaultapi.RbbitMQ;
using documentvaultapi.RbbitMQ.Models.MQueue;
using documentvaultapi.Common.Constants;

namespace documentvaultapi.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ApplicationAuthFilter))]
    [Route("api/Documents")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentservice;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public DocumentController(
            IDocumentService documentservice,
            IRabbitMqService rabbitMqService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _documentservice = documentservice;
            _rabbitMqService = rabbitMqService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<APIResponseClass<DocumentUploadResponseDTO>> Upload(
            [FromForm] DocumentUploadRequestDTO request)
        {
            APIResponseClass<DocumentUploadResponseDTO> response = new();

            try
            {
                var result = await _documentservice.UploadAsync(
                    request.File,
                    request.CreatedBy
                );

                response.apiResponseStatus = APIResponseStatus.Success;
                response.message = "Document uploaded successfully";
                response.result = result;
            }
            catch (DuplicateDocumentException ex)
            {
                response.apiResponseStatus = APIResponseStatus.Warning;
                response.message = "Duplicate document detected";

                response.result = new DocumentUploadResponseDTO
                {
                    DocumentId = ex.ExistingDocumentId,
                    Status = "Duplicate"
                };
            }
            catch (Exception ex)
            {
                response.apiResponseStatus = APIResponseStatus.Error;
                response.message = ex.Message;
            }

            return response;
        }

        [HttpPost("upload-via-queue")]
        [Consumes("multipart/form-data")]
        public async Task<APIResponseClass<DocumentUploadResponseDTO>> UploadViaQueue(
            [FromForm] DocumentUploadRequestDTO request)
        {
            APIResponseClass<DocumentUploadResponseDTO> response = new();

            try
            {
                // Validate file
                if (request.File == null || request.File.Length == 0)
                {
                    response.apiResponseStatus = APIResponseStatus.Error;
                    response.message = "File is empty";
                    return response;
                }

                // Get app_id from header
                var appIdHeader = _httpContextAccessor.HttpContext?
                    .Request.Headers["app_id"]
                    .FirstOrDefault();

                if (!int.TryParse(appIdHeader, out int applicationId))
                {
                    response.apiResponseStatus = APIResponseStatus.Error;
                    response.message = "Invalid or missing app_id header";
                    return response;
                }

                // Read file content into memory
                byte[] fileContent;
                using (var memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream);
                    fileContent = memoryStream.ToArray();
                }

                // Create message DTO
                var uploadMessage = new DocumentUploadMessageDTO
                {
                    MessageId = Guid.NewGuid(),
                    FileContent = fileContent,
                    FileName = request.File.FileName,
                    ContentType = request.File.ContentType,
                    FileSize = request.File.Length,
                    CreatedBy = request.CreatedBy,
                    ApplicationId = applicationId,
                    CreatedAt = DateTime.UtcNow
                };

                // Publish to RabbitMQ queue
                await _rabbitMqService.PublishAsync(
                    MessageQueueConstants.DOCUMENT_UPLOAD_QUEUE,
                    uploadMessage,
                    uploadMessage.MessageId.ToString()
                );

                // Return response indicating queued status
                response.apiResponseStatus = APIResponseStatus.Success;
                response.message = "Document upload queued for processing";
                response.result = new DocumentUploadResponseDTO
                {
                    DocumentId = uploadMessage.MessageId,
                    FileName = request.File.FileName,
                    Status = "Queued",
                    Hash = ""
                };
            }
            catch (Exception ex)
            {
                response.apiResponseStatus = APIResponseStatus.Error;
                response.message = $"Error queuing document upload: {ex.Message}";
            }

            return response;
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var (stream, contentType, fileName) = await _documentservice.DownloadAsync(id);

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponseClass<string>
                {
                    apiResponseStatus = APIResponseStatus.Error,
                    message = ex.Message
                });
            }
        }
        //[HttpGet("download/{documentId}")]
        //public async Task<APIResponseClass<DocumentDownloadResponseDTO>> Download(Guid documentId)
        //{
        //    APIResponseClass<DocumentDownloadResponseDTO> response = new();

        //    try
        //    {
        //        var result = await _Documentservice.GetDownloadUrlAsync(documentId);

        //        response.result = result;
        //        response.apiResponseStatus = APIResponseStatus.Success;
        //        response.message = "Download link generated successfully";

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.apiResponseStatus = APIResponseStatus.Error;
        //        response.message = ex.Message;
        //        return response;
        //    }
        //}

        [HttpDelete("DeleteDocument")]
        public async Task<APIResponseClass<string>> DeleteDocument(
    //[FromHeader(Name = "client_secret")] Guid clientSecret,
    //[FromHeader(Name = "app_id")] long appId,
    [FromQuery(Name = "documentId")] Guid documentId)
        {
            APIResponseClass<string> response = new();

            try
            {
                await _documentservice.DeleteDocumentAsync(documentId);

                response.apiResponseStatus = APIResponseStatus.Success;
                response.message = "Document deleted successfully: " + documentId;
                response.result = "DONE";
            }
            catch (Exception ex)
            {
                response.apiResponseStatus = APIResponseStatus.Error;
                response.message = ex.Message;
            }

            return response;
        }
    }
}
