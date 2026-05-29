using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace documentvaultapi.RbbitMQ.Models.MQueue.FromUM
{
    public class ConsumeApplicationMapDTO
    {
        //public long Id { get; set; }

        public long? AppId { get; set; } //REQ

        public string? AppName { get; set; } //REQ

        public Guid? ClientSecret { get; set; } //REQ

        public DateTime? CreatedAt { get; set; } //REQ

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; } //REQ

        public string? BaseUrl { get; set; }
    }
}