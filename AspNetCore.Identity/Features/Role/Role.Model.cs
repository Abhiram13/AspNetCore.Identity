using System.Text.Json.Serialization;

namespace AspNetCore.Identity.Features.Role.Models;

public record CreateRoleDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
};