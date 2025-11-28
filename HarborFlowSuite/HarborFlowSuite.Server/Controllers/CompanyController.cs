using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Application.Services;

namespace HarborFlowSuite.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
    {
        return Ok(await _companyService.GetCompanies());
    }

    [HttpPost]
    public async Task<ActionResult<Company>> PostCompany(CreateCompanyDto createCompanyDto)
    {
        var company = await _companyService.CreateCompany(createCompanyDto);
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetCompany(Guid id)
    {
        var company = await _companyService.GetCompany(id);

        if (company == null)
        {
            return NotFound();
        }

        return company;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompany(Guid id, Company company)
    {
        var result = await _companyService.UpdateCompany(id, company);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var result = await _companyService.DeleteCompany(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
