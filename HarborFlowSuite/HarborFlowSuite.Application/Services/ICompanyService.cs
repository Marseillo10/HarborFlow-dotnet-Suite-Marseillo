using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Application.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetCompanies();
    Task<Company?> GetCompany(Guid id);
    Task<Company> CreateCompany(CreateCompanyDto createCompanyDto);
    Task<bool> UpdateCompany(Guid id, Company company);
    Task<bool> DeleteCompany(Guid id);
}
