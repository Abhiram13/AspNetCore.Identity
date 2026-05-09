using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Controllers;

#region Models

public record CreateRoleDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
};

#endregion

[ApiController]
[Route("api/roles")]
public class RoleController : BaseApiController
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleController(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleDto payload)
    {
        ApplicationRole role = new ApplicationRole
        {
            Name = payload.Name
        };
        
        await _roleManager.CreateAsync(role);

        return Ok("Role successfully created");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllRolesAsync()
    {

        List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();

        return Ok(roles);
    }
}