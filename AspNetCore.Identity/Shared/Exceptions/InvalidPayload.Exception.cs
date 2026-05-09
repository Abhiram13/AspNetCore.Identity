namespace AspNetCore.Identity.Shared.Exceptions;

public class InvalidPayloadException : Exception
{
    public InvalidPayloadException() { }
    
    public InvalidPayloadException(string message) : base(message) {}
}