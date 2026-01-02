using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using documentvaultapi.DAL.Entities;

namespace documentvaultapi.DAL;

public partial class DocumentVaultDbContext : DbContext
{
    public DocumentVaultDbContext(DbContextOptions<DocumentVaultDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<documents> documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<documents>(entity =>
        {
            entity.HasKey(e => e.id).HasName("Documents_pkey");

            entity.Property(e => e.id).ValueGeneratedNever();
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
