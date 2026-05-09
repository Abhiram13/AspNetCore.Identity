using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using AspNetCore.Identity.Features.User.Models;
using AspNetCore.Identity.Features.User.Services;
using AspNetCore.Identity.Shared.Models;

namespace AspNetCore.Identity.Features.User.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUserAccountAsync([FromBody] CreateUserDto request)
    {
        await _userService.CreateOneUserAsync(request);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse
        {
            StatusCode = HttpStatusCode.Created,
            Message = "User created successfully"
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        IReadOnlyList<ApplicationUser> users = await _userService.GetAllUsersAsync();
        return Ok(new ApiResponse<IReadOnlyList<ApplicationUser>>
        {
            StatusCode = HttpStatusCode.OK,
            Result = users
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync([FromBody] LoginDto request)
    {
        string token = await _userService.LoginAsync(request);
        return Ok(new ApiResponse<string>
        {
            StatusCode = HttpStatusCode.OK,
            Result = token
        });
    }
}