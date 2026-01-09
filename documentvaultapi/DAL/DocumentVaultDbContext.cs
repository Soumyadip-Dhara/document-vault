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

    public virtual DbSet<Documents> Documents { get; set; }
    public virtual DbSet<ApplicationMap> ApplicationMaps { get; set; }

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
