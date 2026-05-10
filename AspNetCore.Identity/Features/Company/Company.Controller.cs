using System.Net;
using AspNetCore.Identity.Features.Company.Models;
using AspNetCore.Identity.Features.Company.Services;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Identity.Features.Company.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly CompanyService _companyService;
    
    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] InsertCompanyDto dto)
    {
        await  _companyService.CreateCompanyAsync(dto.Name);
        return Ok(new ApiResponse { Message =  "Successfully created company", StatusCode = HttpStatusCode.Created });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCompanyAsync()
    {
        // FIX: Why "Entities.Company"?
        IReadOnlyList<Entities.Company> companies = await _companyService.GetAllCompaniesAsync();
        return Ok(new ApiResponse<IReadOnlyList<Entities.Company>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = companies,
            Message = "Successfully retrieved all companies",
        });
    }

    [HttpPost("{companyId:int}")]
    public async Task<IActionResult> CreateOneCompanyUserAsync([FromRoute] int companyId, [FromBody] CreateUserDto payload)
    {
        await _companyService.CreateOneCompanyUserAsync(companyId, payload);
        
        return StatusCode(StatusCodes.Status201Created, new ApiResponse { Message = "Successfully created company user", StatusCode = HttpStatusCode.Created });
    }
}