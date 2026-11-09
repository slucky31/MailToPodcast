using System.Text.Json;

namespace MailToPodcast.Infrastructure.Llm;

/// <summary>
/// Thin wrapper over the Anthropic SDK, so the prompt and the parsing can be tested without calling the API.
/// </summary>
internal interface IClaudeClient
{
    /// <summary>Sends one message and returns the JSON answer, constrained by the schema.</summary>
    Task<string> CompleteJsonAsync(
        string systemPrompt,
        string userMessage,
        IReadOnlyDictionary<string, JsonElement> jsonSchema,
        CancellationToken cancellationToken);
}
