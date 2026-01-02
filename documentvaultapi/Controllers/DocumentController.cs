using documentvaultapi.BAL.Services.Interfaces;
using documentvaultapi.DAL.DTOs;
using documentvaultapi.Helper;
using documentvaultapi.Enum;
using Microsoft.AspNetCore.Mvc;

namespace documentvaultapi.Controllers
{
    [ApiController]
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
    }
}
