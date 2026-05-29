using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace documentvaultapi.RbbitMQ.Models.MQueue
{
    /// <summary>
    /// Message DTO for document upload processing via RabbitMQ queue
    /// </summary>
    public class DocumentUploadMessageDTO
    {
        public Guid MessageId { get; set; }
        
        public byte[] FileContent { get; set; }
        
        public string FileName { get; set; }
        
        public string ContentType { get; set; }
        
        public long FileSize { get; set; }
        
        public long? CreatedBy { get; set; }
        
        public int ApplicationId { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
