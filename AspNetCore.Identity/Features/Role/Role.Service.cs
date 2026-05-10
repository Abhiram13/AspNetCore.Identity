using AspNetCore.Identity.Features.Role.Models;
using AspNetCore.Identity.Features.User.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Features.Role.Services;

public sealed class RoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<RoleService> _logger;
    
    public RoleService(RoleManager<ApplicationRole> roleManager, ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task CreateOneRoleAsync(CreateRoleDto payload)
    {
        ApplicationRole role = new ApplicationRole { Name = payload.Name };
        await _roleManager.CreateAsync(role);
    }

    public async Task<IReadOnlyList<ApplicationRole>> GetAllRolesAsync()
    {
         return await _roleManager.Roles.ToListAsync();
    }
    
    public async Task<bool> IsRoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<int?> GetRoleIdByNameAsync(string roleName)
    {
        ApplicationRole? role = await _roleManager.FindByNameAsync(roleName);
        return role?.Id;
    }
}