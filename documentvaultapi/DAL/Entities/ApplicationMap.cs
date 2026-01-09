using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Entities;

[Table("application_map")]
public partial class ApplicationMap
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("app_id")]
    public long AppId { get; set; }

    [Column("app_name")]
    [StringLength(100)]
    public string? AppName { get; set; }

    [Column("client_secret")]
    public Guid ClientSecret { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("base_url")]
    public string? BaseUrl { get; set; }
}
