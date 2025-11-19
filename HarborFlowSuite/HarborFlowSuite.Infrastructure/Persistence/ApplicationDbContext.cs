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
    public DbSet<Company> Companies { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
    public DbSet<Port> Ports { get; set; }
    public virtual DbSet<GfwMetadataCache> GfwMetadataCache { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary keys for string Ids
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Vessel>().HasKey(v => v.Id);
        modelBuilder.Entity<ServiceRequest>().HasKey(sr => sr.Id);
        modelBuilder.Entity<Company>().HasKey(c => c.Id);
        modelBuilder.Entity<ApprovalHistory>().HasKey(ah => ah.Id);
        modelBuilder.Entity<Port>().HasKey(p => p.Id);

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
            .WithMany()
            .HasForeignKey(sr => sr.RequesterId)
            .IsRequired();

        modelBuilder.Entity<ServiceRequest>()
            .HasOne(sr => sr.Vessel)
            .WithMany()
            .HasForeignKey(sr => sr.VesselId)
            .IsRequired(false);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(ah => ah.ServiceRequest)
            .WithMany(sr => sr.ApprovalHistories)
            .HasForeignKey(ah => ah.ServiceRequestId)
            .IsRequired();

        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(ah => ah.Approver)
            .WithMany()
            .HasForeignKey(ah => ah.ApproverId)
            .IsRequired();

        // Configure enum conversions
        modelBuilder.Entity<ServiceRequest>()
            .Property(sr => sr.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ApprovalHistory>()
            .Property(ah => ah.Status)
            .HasConversion<string>();
    }
}
