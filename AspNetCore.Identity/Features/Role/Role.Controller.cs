using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Identity.Features.Role.Models;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Shared.Models;

namespace AspNetCore.Identity.Features.Role.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public RoleController(RoleService roleService)
    {
        _roleService = roleService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateRoleAsync([FromBody] CreateRoleDto payload)
    {
        await _roleService.CreateOneRoleAsync(payload);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse
        {
            StatusCode = HttpStatusCode.Created,
            Message = "Role was created"
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        IReadOnlyList<ApplicationRole> roles = await _roleService.GetAllRolesAsync();
        return Ok(new ApiResponse<IReadOnlyList<ApplicationRole>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = roles
        });
    }
}