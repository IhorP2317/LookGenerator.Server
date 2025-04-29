namespace LookGenerator.Application.Common.Exceptions;

public class ThirdPartyResponseException(string message, string? rawResponse = null, Exception? innerException = null)
    : Exception(message, innerException)
{
    public string? RawResponse { get; } = rawResponse;
}