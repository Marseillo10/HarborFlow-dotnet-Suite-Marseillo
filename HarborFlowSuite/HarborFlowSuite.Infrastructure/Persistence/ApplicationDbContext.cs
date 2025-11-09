using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Vessel> Vessels { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<MapBookmark> MapBookmarks { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary keys for string Ids
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Vessel>().HasKey(v => v.Id);
        modelBuilder.Entity<ServiceRequest>().HasKey(sr => sr.Id);
        modelBuilder.Entity<MapBookmark>().HasKey(mb => mb.Id);
        modelBuilder.Entity<Company>().HasKey(c => c.Id);

        // Configure relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .IsRequired(false); // CompanyId is optional for some roles

        modelBuilder.Entity<Vessel>()
            .HasOne(v => v.Company)
            .WithMany(c => c.Vessels)
            .HasForeignKey(v => v.CompanyId)
            .IsRequired(false); // CompanyId is optional

        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.Requester)
            .WithMany(u => u.ServiceRequests)
            .HasForeignKey(sr => sr.RequestorUserId)
            .IsRequired();

        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.AssignedOfficer)
            .WithMany()
            .HasForeignKey(sr => sr.AssignedOfficerUserId)
            .IsRequired(false);

        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.Vessel)
            .WithMany()
            .HasForeignKey(sr => sr.VesselId)
            .IsRequired(false);

        modelBuilder.Entity<MapBookmark>()
            .HasOne(mb => mb.User)
            .WithMany(u => u.MapBookmarks)
            .HasForeignKey(mb => mb.UserId)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);
            
        // Configure enum conversions
        modelBuilder.Entity<ServiceRequest>()
            .Property(sr => sr.Status)
            .HasConversion<string>();
    }
}
