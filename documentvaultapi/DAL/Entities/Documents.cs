using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Entities;

public partial class documents
{
    [Key]
    public Guid id { get; set; }

    public string object_name { get; set; } = null!;

    public string bucket_name { get; set; } = null!;

    public string original_file_name { get; set; } = null!;

    public string? content_type { get; set; }

    public long file_size { get; set; }

    public long? created_by { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? created_at { get; set; }

    public bool is_active { get; set; }
}
