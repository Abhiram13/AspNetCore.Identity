using System.Net;
using AspNetCore.Identity.Features.Companies.Entities;
using AspNetCore.Identity.Features.Companies.Models;
using AspNetCore.Identity.Features.Companies.Services;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Identity.Features.Companies.Controllers;

[ApiController]
[Route("api/companies")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CompanyController : ControllerBase
{
    private readonly CompanyService _companyService;
    
    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpPost]
    [Authorize(Policy = Shared.Constants.Policies.IS_SUPER_ADMIN)]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] InsertCompanyDto dto)
    {
        await  _companyService.CreateCompanyAsync(dto.Name);
        return Ok(new ApiResponse { Message =  "Successfully created company", StatusCode = HttpStatusCode.Created });
    }
    
    [HttpGet]
    [Authorize(Policy = Shared.Constants.Policies.IS_SUPER_ADMIN)]
    public async Task<IActionResult> GetAllCompanyAsync()
    {
        IReadOnlyList<Company> companies = await _companyService.GetAllCompaniesAsync();
        return Ok(new ApiResponse<IReadOnlyList<Company>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = companies,
            Message = "Successfully retrieved all companies",
        });
    }

    [HttpPost("{companyId:int}/users")]
    [Authorize(Policy = Shared.Constants.Policies.IS_COMPANY_ADMIN)]
    public async Task<IActionResult> CreateOneCompanyUserAsync([FromRoute] int companyId, [FromBody] CreateCompanyUserDto payload)
    {
        await _companyService.CreateOneCompanyUserAsync(companyId, payload);
        
        return StatusCode(StatusCodes.Status201Created, new ApiResponse { Message = "Successfully created company user", StatusCode = HttpStatusCode.Created });
    }

    [HttpGet("{companyId:int}/users")]
    [Authorize(Policy = Shared.Constants.Policies.IS_COMPANY_ADMIN)]
    public async Task<IActionResult> GetAllUsersByCompanyAsync([FromRoute] int companyId)
    {
        IReadOnlyList<ApplicationUser> users = await _companyService.GetAllUsersByCompanyAsync(companyId);

        return Ok(new ApiResponse<IReadOnlyList<ApplicationUser>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = users,
        });
    }
}