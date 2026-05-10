using AspNetCore.Identity.Features.Companies.Repository;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Features.User.Services;

namespace AspNetCore.Identity.Features.Companies.Services;

public sealed class CompanyService
{
    private readonly CompanyRepository _companyRepository;
    private readonly UserService _userService;
    
    public CompanyService(CompanyRepository companyRepository, UserService userService)
    {
        _companyRepository = companyRepository;
        _userService = userService;
    }

    public async Task CreateCompanyAsync(string companyName)
    {
        await _companyRepository.CreateOneCompanyAsync(companyName);
    }

    public async Task<IReadOnlyList<Entities.Company>> GetAllCompaniesAsync()
    {
        return await _companyRepository.GetAllCompaniesAsync();
    }

    public async Task CreateOneCompanyUserAsync(int companyId, CreateUserDto payload)
    {
        CreateUserResultDto userResult = await _userService.CreateOneUserAsync(payload);
        await _companyRepository.CreateOneCompanyUserAsync(companyId, userResult.UserId, userResult.RoleId);
    }
}