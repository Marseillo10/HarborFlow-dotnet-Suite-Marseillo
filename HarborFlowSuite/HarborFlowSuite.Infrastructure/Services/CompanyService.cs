using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CompanyService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<Company>> GetCompanies()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<Company?> GetCompany(Guid id)
    {
        return await _context.Companies
            .Include(c => c.Histories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company> CreateCompany(CreateCompanyDto createCompanyDto)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = createCompanyDto.Name,
            LogoUrl = createCompanyDto.LogoUrl,
            Website = createCompanyDto.Website,
            PrimaryContactEmail = createCompanyDto.PrimaryContactEmail,
            BillingAddress = createCompanyDto.BillingAddress,
            SubscriptionTier = createCompanyDto.SubscriptionTier,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Companies.Add(company);

        // Add History
        var history = new CompanyHistory
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Action = "Created",
            ChangedBy = _currentUserService.UserId,
            ChangeDetails = $"Company {company.Name} created.",
            CreatedAt = DateTime.UtcNow
        };
        _context.CompanyHistories.Add(history);

        await _context.SaveChangesAsync();

        return company;
    }

    public async Task<bool> UpdateCompany(Guid id, Company company)
    {
        if (id != company.Id)
        {
            return false;
        }

        var existingCompany = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (existingCompany == null) return false;

        _context.Entry(company).State = EntityState.Modified;

        // Add History
        var changes = new List<string>();
        if (existingCompany.Name != company.Name) changes.Add($"Name changed from '{existingCompany.Name}' to '{company.Name}'");
        if (existingCompany.SubscriptionTier != company.SubscriptionTier) changes.Add($"Tier changed from '{existingCompany.SubscriptionTier}' to '{company.SubscriptionTier}'");
        if (existingCompany.PrimaryContactEmail != company.PrimaryContactEmail) changes.Add($"Email changed from '{existingCompany.PrimaryContactEmail}' to '{company.PrimaryContactEmail}'");
        if (existingCompany.LogoUrl != company.LogoUrl) changes.Add("Logo URL changed");
        if (existingCompany.Website != company.Website) changes.Add($"Website changed from '{existingCompany.Website}' to '{company.Website}'");
        if (existingCompany.BillingAddress != company.BillingAddress) changes.Add($"Billing Address changed from '{existingCompany.BillingAddress}' to '{company.BillingAddress}'");

        if (changes.Any())
        {
            var history = new CompanyHistory
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                Action = "Updated",
                ChangedBy = _currentUserService.UserId,
                ChangeDetails = string.Join(", ", changes),
                CreatedAt = DateTime.UtcNow
            };
            _context.CompanyHistories.Add(history);
        }

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CompanyExists(id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<bool> DeleteCompany(Guid id)
    {
        var company = await _context.Companies
            .Include(c => c.Vessels)
            .Include(c => c.Users)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
        {
            return false;
        }

        if (company.Vessels != null && company.Vessels.Any())
        {
            throw new InvalidOperationException("Cannot delete company because it has assigned vessels.");
        }

        if (company.Users != null && company.Users.Any())
        {
            throw new InvalidOperationException("Cannot delete company because it has assigned users.");
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool CompanyExists(Guid id)
    {
        return _context.Companies.Any(e => e.Id == id);
    }
}
