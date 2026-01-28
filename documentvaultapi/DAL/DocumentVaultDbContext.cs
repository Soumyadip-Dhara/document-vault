using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using documentvaultapi.DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace documentvaultapi.DAL;

public partial class DocumentVaultDbContext : DbContext
{
    public DocumentVaultDbContext(DbContextOptions<DocumentVaultDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Documents> Documents { get; set; }
    public virtual DbSet<ApplicationMap> ApplicationMaps { get; set; }
    public virtual DbSet<ConsumeFailedLog> ConsumeFailedLogs { get; set; }
    public virtual DbSet<ConsumeLog> ConsumeLogs { get; set; }
    public virtual DbSet<ConsumedAcknowledgementLog> ConsumedAcknowledgementLogs { get; set; }
    public virtual DbSet<MessageQueue> MessageQueues { get; set; }

    public virtual DbSet<MessageQueueFailedLog> MessageQueueFailedLogs { get; set; }

    public virtual DbSet<MessageQueueLog> MessageQueueLogs { get; set; }
    public virtual DbSet<PublishedAcknowledgementLog> PublishedAcknowledgementLogs { get; set; }
    public virtual DbSet<QueuesMaster> QueuesMasters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Documents>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Documents_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
        modelBuilder.Entity<ApplicationMap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("application_map_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });
        modelBuilder.Entity<ConsumeFailedLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("consume_failed_logs_pkey");

            entity.Property(e => e.ActionStatus)
                .HasDefaultValueSql("'PENDING'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.FailedType).IsFixedLength();
            entity.Property(e => e.IsRedelivered).HasDefaultValue(false);
        });

        modelBuilder.Entity<ConsumeLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("consume_logs_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ErrorType).IsFixedLength();
            entity.Property(e => e.Status).IsFixedLength();
        });
        modelBuilder.Entity<ConsumedAcknowledgementLog>(entity =>
        {
            entity.HasKey(e => e.UniqueId).HasName("consumed_acknowledgement_logs_pkey");

            entity.Property(e => e.UniqueId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.ErrorType).IsFixedLength();
            entity.Property(e => e.Status).IsFixedLength();
        });

        modelBuilder.Entity<MessageQueue>(entity =>
        {
            entity.HasKey(e => e.UniqueId).HasName("message_queues_pkey");

            entity.Property(e => e.UniqueId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<MessageQueueFailedLog>(entity =>
        {
            entity.HasKey(e => e.UniqueId).HasName("message_queue_failed_logs_pkey");

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.Property(e => e.UniqueId).ValueGeneratedNever();
            entity.Property(e => e.ActionStatus)
                .HasDefaultValueSql("'PENDING'::bpchar")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasConversion(dateTimeConverter);
            entity.Property(e => e.FailedAt).HasConversion(dateTimeConverter);
            entity.Property(e => e.ResolvedAt).HasConversion(dateTimeConverter);
            entity.Property(e => e.UpdatedAt).HasConversion(dateTimeConverter);
            entity.Property(e => e.FailedType).IsFixedLength();
        });

        modelBuilder.Entity<MessageQueueLog>(entity =>
        {
            entity.HasKey(e => e.UniqueId).HasName("message_queue_logs_pkey");

            entity.Property(e => e.UniqueId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<PublishedAcknowledgementLog>(entity =>
        {
            entity.HasKey(e => e.UniqueId).HasName("published_acknowledgement_log_pkey");

            entity.Property(e => e.UniqueId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<QueuesMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("queues_master_pkey");

            entity.Property(e => e.Consumer).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Identifier).IsFixedLength();
            entity.Property(e => e.Producer).IsFixedLength();
            entity.Property(e => e.Status).HasDefaultValue((short)1);
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
