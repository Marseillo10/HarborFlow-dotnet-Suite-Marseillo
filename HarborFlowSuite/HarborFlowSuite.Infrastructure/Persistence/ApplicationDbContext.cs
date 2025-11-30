using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly Services.ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, Services.ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Vessel> Vessels { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
    public DbSet<Port> Ports { get; set; }
    public DbSet<CompanyHistory> CompanyHistories { get; set; }
    public virtual DbSet<GfwMetadataCache> GfwMetadataCache { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Global Query Filters for Company Isolation
        // Allow seeing SystemAdmins to support ApprovalHistory visibility
        modelBuilder.Entity<User>().HasQueryFilter(e => _currentUserService.IsSystemAdmin || e.CompanyId == _currentUserService.CompanyId || e.Role.Name == Shared.Constants.UserRole.SystemAdmin);
        modelBuilder.Entity<Vessel>().HasQueryFilter(e => _currentUserService.IsSystemAdmin || e.CompanyId == _currentUserService.CompanyId);
        modelBuilder.Entity<ServiceRequest>().HasQueryFilter(e => _currentUserService.IsSystemAdmin || e.CompanyId == _currentUserService.CompanyId);
        // Apply filter to ApprovalHistory to avoid issues with required User relationship
        modelBuilder.Entity<ApprovalHistory>().HasQueryFilter(e => _currentUserService.IsSystemAdmin || e.Approver.CompanyId == _currentUserService.CompanyId || e.Approver.Role.Name == Shared.Constants.UserRole.SystemAdmin);
        // Apply filter to VesselPosition to match Vessel filter
        modelBuilder.Entity<VesselPosition>().HasQueryFilter(e => _currentUserService.IsSystemAdmin || e.Vessel.CompanyId == _currentUserService.CompanyId);

        // Configure primary keys for string Ids
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Vessel>().HasKey(v => v.Id);
        modelBuilder.Entity<ServiceRequest>().HasKey(sr => sr.Id);
        modelBuilder.Entity<Company>().HasKey(c => c.Id);
        modelBuilder.Entity<ApprovalHistory>().HasKey(ah => ah.Id);
        modelBuilder.Entity<Port>().HasKey(p => p.Id);

        // Configure Indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.FirebaseUid)
            .IsUnique();

        modelBuilder.Entity<Vessel>()
            .HasIndex(v => v.MMSI)
            .IsUnique();

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
