using System.Net;
using System.Text.Json.Serialization;

namespace AspNetCore.Identity.Shared.Models;

public sealed record ApiResponse<T> where T : class
{
    public required HttpStatusCode StatusCode { get; init; }
    public string? Message { get; init; }
    public T? Result { get; init; }
}

public sealed record ApiResponse
{
    public required HttpStatusCode StatusCode { get; init; }
    public required string Message { get; init; }
}