using System.Text.Json.Serialization;

namespace AspNetCore.Identity.Features.User.Models;

public record CreateUserDto(string Email, string Password, string RoleName);

public record LoginDto
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}

public record CreateUserResultDto
{
    public int UserId { get; init; }
    public int RoleId { get; init; }
    
}