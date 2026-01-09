using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.Helper;
using documentvaultapi.Enum;
using Microsoft.AspNetCore.Mvc;
using documentvaultapi.Filters;

namespace documentvaultapi.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ApplicationAuthFilter))]
    [Route("api/Documents")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentservice;

        public DocumentController(IDocumentService documentservice)
        {
            _documentservice = documentservice;
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
