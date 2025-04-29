namespace LookGenerator.Infrastructure.Settings;

public class OpenAiSettings
{
    public string ApiKey { get; set; }  = string.Empty;

    public string ModelName { get; set; } = "o4-mini";
}