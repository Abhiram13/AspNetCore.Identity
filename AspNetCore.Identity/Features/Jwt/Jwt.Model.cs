using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.Identity.Features.Jwt.Models;

public record JwtConfiguration
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public required bool ValidateIssuerSigningKey { get; init; }
}