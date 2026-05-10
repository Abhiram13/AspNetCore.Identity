namespace AspNetCore.Identity.Features.Company.Models;

public record InsertCompanyDto
{
    public required string Name { get; init; }
}