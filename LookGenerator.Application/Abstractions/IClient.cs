using LookGenerator.Application.Common.Constants;

namespace LookGenerator.Application.Abstractions;

public interface IClient
{
    Task<string> SendAsync(string prompt, OutputResultJsonScheme outputResultJsonScheme, CancellationToken cancellationToken);

    Task<string> UploadJsonFileAsync(string jsonContent, string instructions, string initialMessage, OutputResultJsonScheme outputResultJsonScheme,
        CancellationToken cancellationToken = default);
}