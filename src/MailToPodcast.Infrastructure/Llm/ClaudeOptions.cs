using System.ComponentModel.DataAnnotations;

namespace MailToPodcast.Infrastructure.Llm;

/// <summary>
/// Settings of the Claude API.
/// </summary>
public sealed class ClaudeOptions
{
    public const string SectionName = "Claude";

    /// <summary>API key (user secrets or Key Vault, never in appsettings.json).</summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Model id.</summary>
    [Required]
    public string Model { get; set; } = "claude-opus-5-5";

    /// <summary>Effort level: low, medium, high, xhigh or max.</summary>
    [Required]
    [RegularExpression("^(low|medium|high|xhigh|max)$")]
    public string Effort { get; set; } = "low";

    /// <summary>Maximum tokens of the answer: the script of a long newsletter is long.</summary>
    [Range(1024, 128000)]
    public int MaxTokens { get; set; } = 64000;
}
