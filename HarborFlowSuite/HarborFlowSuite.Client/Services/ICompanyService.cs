using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Client.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompanies();
        Task<Company> GetCompany(Guid id);
        Task<Company> CreateCompany(CreateCompanyDto createCompanyDto);
        Task<bool> UpdateCompany(Guid id, Company company);
        Task<bool> DeleteCompany(Guid id);
    }
}
