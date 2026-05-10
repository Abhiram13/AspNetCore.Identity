namespace AspNetCore.Identity.Features.Companies.Models;

public record InsertCompanyDto
{
    public required string Name { get; init; }
}

public record CreateCompanyUserDto
{
    public required string Email { get; init; }
    public required  string Password { get; init; }
}