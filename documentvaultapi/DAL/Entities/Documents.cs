//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

//namespace documentvaultapi.DAL.Entities;

//public partial class documents
//{
//    [Key]
//    public Guid id { get; set; }

//    public string object_name { get; set; } = null!;

//    public string bucket_name { get; set; } = null!;

//    public string original_file_name { get; set; } = null!;

//    public string? content_type { get; set; }

//    public long file_size { get; set; }

//    public int? application_id { get; set; }

//    public long? created_by { get; set; }

//    [Column(TypeName = "timestamp without time zone")]
//    public DateTime? created_at { get; set; }

//    public bool is_active { get; set; }

//    public string? file_hash { get; set; }
//}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Entities;

[Table("documents")]
public partial class Documents
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("object_name")]
    [StringLength(255)]
    public string ObjectName { get; set; } = null!;

    [Column("bucket_name")]
    [StringLength(255)]
    public string BucketName { get; set; } = null!;

    [Column("original_file_name")]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = null!;

    [Column("content_type")]
    [StringLength(100)]
    public string? ContentType { get; set; }

    [Column("file_size")]
    public long FileSize { get; set; }

    [Column("application_id")]
    public int? ApplicationId { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("file_hash")]
    [StringLength(128)]
    public string? FileHash { get; set; }
}
