using System.Security.Claims;
using AspNetCore.Identity.Features.Jwt.Services;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AspNetCore.Identity.Features.User.Services;

public sealed class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleService _roleService;
    private readonly JwtService _jwtService;

    public UserService(UserManager<ApplicationUser> userManager, RoleService roleService, JwtService jwtService)
    {
        _userManager = userManager;
        _roleService = roleService;
        _jwtService = jwtService;
    }

    public async Task<CreateUserResultDto> CreateOneUserAsync(CreateUserDto payload)
    {
        bool isRoleExists = await _roleService.IsRoleExistsAsync(payload.RoleName);

        if (!isRoleExists)
        {
            throw new InvalidPayloadException("Role does not exist");
        }

        ApplicationUser user = new ApplicationUser
        {
            UserName = payload.Email,
            Email = payload.Email,
            EmailConfirmed = false,
        };
        
        IdentityResult createUserResult = await _userManager.CreateAsync(user, payload.Password);

        if (!createUserResult.Succeeded)
        {
            throw new InvalidPayloadException($"Failed to create user - {createUserResult.Errors.First().Description}"); // FIX: Try to get rightful error message
        }
        
        IdentityResult assignResult = await _userManager.AddToRoleAsync(user, payload.RoleName);
        
        if (!assignResult.Succeeded)
        {
            throw new InvalidPayloadException($"Failed to assign role - {assignResult.Errors}");
        }
        
        int? roleId = await _roleService.GetRoleIdByNameAsync(payload.RoleName);
        
        return new CreateUserResultDto { UserId = user.Id, RoleId = roleId ?? 0 }; // FIX: Role id will not be null here and should not pass o as default
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<string> LoginAsync(LoginDto payload)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            throw new  InvalidPayloadException("Invalid Credentials");
        }
        
        bool result = await _userManager.CheckPasswordAsync(user, payload.Password);
        if (!result)
        {
            throw new  InvalidPayloadException("Invalid Credentials");
        }
        
        IList<string> roles = await _userManager.GetRolesAsync(user);
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        string token = _jwtService.CreateToken(claims);

        return token;
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersByIdAsync(IReadOnlyList<int> userIds)
    {
        List<ApplicationUser> users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        
        return users;
    }
}