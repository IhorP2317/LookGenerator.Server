using System.ClientModel;
using LookGenerator.Application.Abstractions;
using LookGenerator.Application.Common.Constants;
using LookGenerator.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Files;

namespace LookGenerator.Infrastructure.Services;

public class OpenAiClient(OpenAIClient client, IOptions<OpenAiSettings> options) : IClient
{
    
    public async Task<string> SendAsync(string prompt,OutputResultJsonScheme outputResultJsonScheme, CancellationToken cancellationToken)
    {
        var chatClient = client.GetChatClient(options.Value.ModelName);
        var messages = new List<ChatMessage> { new UserChatMessage(prompt) };

        var chatCompletionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: outputResultJsonScheme.Name,
                jsonSchema: BinaryData.FromString(outputResultJsonScheme.Format),
                jsonSchemaFormatDescription: outputResultJsonScheme.Description,
                jsonSchemaIsStrict: true
            )
        };


        var chatCompletion = await chatClient.CompleteChatAsync(messages, chatCompletionOptions, cancellationToken);

        return chatCompletion.Value.Content[0].Text;
    }
#pragma warning disable OPENAI001
    public async Task<string> UploadJsonFileAsync(string jsonContent, string instructions, string initialMessage, OutputResultJsonScheme outputResultJsonScheme,
        CancellationToken cancellationToken = default)
    {
        var fileClient = client.GetOpenAIFileClient();
        var assistantClient = client.GetAssistantClient();
        var fileName = $"looks-{Guid.NewGuid()}.json";


        await using var stream = BinaryData.FromString(jsonContent).ToStream();


        var looksFile = await fileClient.UploadFileAsync(
            stream,
            fileName,
            FileUploadPurpose.Assistants,
            cancellationToken
        );

        var assistantOptions = new AssistantCreationOptions
        {
            Name = "Look Generator",
            Instructions = instructions,
            Tools =
            {
                new FileSearchToolDefinition(),
                new CodeInterpreterToolDefinition()
            },
            ToolResources = new ToolResources
            {
                FileSearch = new FileSearchToolResources
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([looksFile.Value.Id]),
                    }
                }
            },
            ResponseFormat = AssistantResponseFormat.CreateJsonSchemaFormat(
                name:  outputResultJsonScheme.Name,
                jsonSchema: BinaryData.FromString(outputResultJsonScheme.Format),
                description: outputResultJsonScheme.Description,
                strictSchemaEnabled: true
            )
        };
        var assistant =
            await assistantClient.CreateAssistantAsync(options.Value.ModelName, assistantOptions, cancellationToken);
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { $"Analyze the uploaded JSON file id {looksFile.Value.Id} and {initialMessage}" }
        };
        ThreadRun threadRun = await assistantClient.CreateThreadAndRunAsync(assistant.Value.Id, threadOptions,
            cancellationToken: cancellationToken);
        do
        {
            Thread.Sleep(1000);
            threadRun = await assistantClient.GetRunAsync(threadRun.ThreadId, threadRun.Id, cancellationToken);
        } while (!threadRun.Status.IsTerminal);

        AsyncCollectionResult<ThreadMessage> messages
            = assistantClient.GetMessagesAsync(threadRun.ThreadId,
                new MessageCollectionOptions { Order = MessageCollectionOrder.Ascending }, cancellationToken);
        var result = string.Empty;
        await foreach (var message in messages)
        {
            if (message.Role != MessageRole.Assistant) continue;
            foreach (var contentItem in message.Content)
            {
                if (string.IsNullOrEmpty(contentItem.Text)) continue;
                result = contentItem.Text;
                break;
            }
        }
        _ = await assistantClient.DeleteThreadAsync(threadRun.ThreadId, cancellationToken);
        _ = await assistantClient.DeleteAssistantAsync(assistant.Value.Id, cancellationToken);
        _ = await fileClient.DeleteFileAsync(looksFile.Value.Id, cancellationToken);
        return result;
    }
}