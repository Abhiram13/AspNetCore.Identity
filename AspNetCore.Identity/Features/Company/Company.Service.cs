using AspNetCore.Identity.Features.Companies.Models;
using AspNetCore.Identity.Features.Companies.Repository;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Features.User.Services;
using AspNetCore.Identity.Shared.Exceptions;

namespace AspNetCore.Identity.Features.Companies.Services;

public sealed class CompanyService
{
    private readonly CompanyRepository _companyRepository;
    private readonly UserService _userService;
    private readonly RoleService _roleService;
    
    public CompanyService(CompanyRepository companyRepository, UserService userService, RoleService roleService)
    {
        _companyRepository = companyRepository;
        _userService = userService;
        _roleService = roleService;
    }

    public async Task CreateCompanyAsync(string companyName)
    {
        await _companyRepository.CreateOneCompanyAsync(companyName);
    }

    public async Task<IReadOnlyList<Entities.Company>> GetAllCompaniesAsync()
    {
        return await _companyRepository.GetAllCompaniesAsync();
    }

    public async Task CreateOneCompanyUserAsync(int companyId, CreateCompanyUserDto payload)
    {
        CreateUserResultDto userResult = await _userService.CreateOneUserAsync(new CreateUserDto(payload.Email, payload.Password, "User"));
        await _companyRepository.CreateOneCompanyUserAsync(companyId, userResult.UserId, userResult.RoleId); // BUG: This one failed due to Foreign key constraint, but above user was created.
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersByCompanyAsync(int companyId)
    {
        IReadOnlyList<int> userIds = await _companyRepository.GetAllCompanyUsersAsync(companyId);
        IReadOnlyList<ApplicationUser> users = await _userService.GetAllUsersByIdAsync(userIds);
        
        return users;
    }

    private async Task<bool> IsAdminRoleExistsInCompanyAsync(int companyId)
    {
        int? roleId = await _roleService.GetRoleIdByNameAsync("Admin");
        
        if (roleId is not null && roleId.Value > 0)
        {
            bool isAdminRoleExistsInCompany = await _companyRepository.IsAdminRoleExistsInCompanyAsync(companyId, (int) roleId);
            return isAdminRoleExistsInCompany;
        }
        
        return false;
    }
}