using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;

    public CompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetCompanies()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<Company?> GetCompany(Guid id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<Company> CreateCompany(CreateCompanyDto createCompanyDto)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = createCompanyDto.Name
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return company;
    }

    public async Task<bool> UpdateCompany(Guid id, Company company)
    {
        if (id != company.Id)
        {
            return false;
        }

        _context.Entry(company).State = EntityState.Modified;

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
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return false;
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
