using System.Security.Claims;
using System.Text.Json.Serialization;
using AspNetCore.Identity.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AspNetCore.Identity.Controllers;

#region Models

public record CreateUserDto(string Email, string Password, string RoleName);

public record LoginDto
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}

#endregion

[ApiController]
[Route("api/users")]
public class UserController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JwtConfiguration _jwtConfiguration;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<JwtConfiguration> jwtConfiguration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtConfiguration = jwtConfiguration.Value;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUserAccountAsync([FromBody] CreateUserDto request)
    {
        // 1. Ensure the role exists
        bool roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        
        if (!roleExists)
        {
            return BadRequest("Role does not exist");
        }

        // 2. Create the User
        ApplicationUser user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
        };

        IdentityResult userResult = await _userManager.CreateAsync(user, request.Password);

        if (!userResult.Succeeded) return BadRequest(userResult.Errors);

        // 3. Assign the User to the Role
        IdentityResult assignResult = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!assignResult.Succeeded) return BadRequest(assignResult.Errors);

        return Ok(new { Message = $"User {request.Email} created and assigned to {request.RoleName}" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        List<ApplicationUser> users = await _userManager.Users.ToListAsync();
        
        return Ok(users);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync([FromBody] LoginDto request)
    {
        // 1. Find user by email
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Invalid credentials.");

        // 2. Check password
        bool result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) return Unauthorized("Invalid credentials.");
        
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add Roles to claims
        IList<string> roles = await _userManager.GetRolesAsync(user);
        
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        
        string token = JwtFactory.CreateToken(_jwtConfiguration, claims);
        
        return Ok(token);
    }
}