namespace AspNetCore.Identity.Features.Companies.Models;

public record InsertCompanyDto
{
    public required string Name { get; init; }
}