using System.Text;
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Beta;
using Anthropic.Models.Beta.Messages;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Infrastructure.Llm;

/// <summary>
/// Calls the Messages API in streaming (long scripts would hit the HTTP timeout otherwise), with structured
/// output and the server-side refusal fallback.
/// </summary>
internal sealed class AnthropicClaudeClient(IOptions<ClaudeOptions> options) : IClaudeClient, IDisposable
{
    private readonly AnthropicClient _client = new() { ApiKey = options.Value.ApiKey };

    public async Task<string> CompleteJsonAsync(
        string systemPrompt,
        string userMessage,
        IReadOnlyDictionary<string, JsonElement> jsonSchema,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var parameters = new MessageCreateParams
        {
            Model = settings.Model,
            MaxTokens = settings.MaxTokens,
            Betas = [AnthropicBeta.ServerSideFallback2026_07_01],
            Fallbacks = new Default(),
            System = systemPrompt,
            OutputConfig = new BetaOutputConfig
            {
                Effort = ToEffort(settings.Effort),
                Format = new BetaJsonOutputFormat { Schema = jsonSchema.ToDictionary() },
            },
            Messages = [new BetaUserMessageParam { Content = userMessage }],
        };

        var text = new StringBuilder();
        string? stopReason = null;
        await foreach (var streamEvent in _client.Beta.Messages.CreateStreaming(parameters, cancellationToken))
        {
            if (streamEvent.TryPickContentBlockDelta(out var delta) && delta.Delta.TryPickText(out var textDelta))
            {
                text.Append(textDelta.Text);
            }
            else if (streamEvent.TryPickDelta(out var messageDelta) && messageDelta.Delta.StopReason is { } reason)
            {
                stopReason = reason.Raw();
            }
        }

        if (stopReason is not null && stopReason != "end_turn")
        {
            throw new ClaudeResponseException($"Claude stopped before the end of the answer (stop reason: {stopReason}).");
        }

        return text.ToString();
    }

    public void Dispose() => _client.Dispose();

    private static Effort ToEffort(string effort) => effort switch
    {
        "medium" => Anthropic.Models.Beta.Messages.Effort.Medium,
        "high" => Anthropic.Models.Beta.Messages.Effort.High,
        "xhigh" => Anthropic.Models.Beta.Messages.Effort.Xhigh,
        "max" => Anthropic.Models.Beta.Messages.Effort.Max,
        _ => Anthropic.Models.Beta.Messages.Effort.Low,
    };
}
