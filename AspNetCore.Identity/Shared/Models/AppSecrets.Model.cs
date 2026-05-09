namespace AspNetCore.Identity.Shared.Models;

public sealed record PostgresConnection
{
    public required string DbConnection { get; init; }
}