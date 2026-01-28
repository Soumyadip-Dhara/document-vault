using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace documentvaultapi.DAL.Entities;

[Table("queues_master", Schema = "message_queue")]
public partial class QueuesMaster
{
    [Key]
    [Column("id")]
    public short Id { get; set; }

    [Column("queue_name")]
    [StringLength(100)]
    public string QueueName { get; set; } = null!;

    [Column("identifier")]
    [StringLength(50)]
    public string Identifier { get; set; } = null!;

    [Column("status")]
    public short Status { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [Column("exchange_name", TypeName = "character varying")]
    public string? ExchangeName { get; set; }

    [Column("producer")]
    [StringLength(20)]
    public string? Producer { get; set; }

    [Column("consumer")]
    [StringLength(20)]
    public string? Consumer { get; set; }
}
