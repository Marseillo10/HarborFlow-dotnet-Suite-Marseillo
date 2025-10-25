using HarborFlow.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HarborFlow.Infrastructure;

public class HarborFlowDbContext : DbContext
{
    public HarborFlowDbContext(DbContextOptions<HarborFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Vessel> Vessels { get; set; }
    public DbSet<VesselPosition> VesselPositions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
    public DbSet<MapBookmark> MapBookmarks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vessel>(entity =>
        {
            entity.ToTable("vessels");

            entity.HasKey(e => e.IMO);
            entity.Property(e => e.IMO).HasMaxLength(7);

            entity.Property(e => e.Mmsi).HasMaxLength(9);
            entity.HasIndex(e => e.Mmsi).IsUnique();

            entity.Property(e => e.Metadata).HasColumnType("jsonb");

            entity.HasMany(e => e.Positions)
                .WithOne(p => p.Vessel)
                .HasForeignKey(p => p.VesselImo);
        });

        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.HasKey(e => e.ApprovalId);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId);
        });

        modelBuilder.Entity<MapBookmark>(entity =>
        {
            entity.ToTable("map_bookmarks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
        });
    }
}