using System.ComponentModel.DataAnnotations;

namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Settings of Azure AI Speech.
/// </summary>
public sealed class AzureSpeechOptions
{
    public const string SectionName = "Speech";

    /// <summary>Resource key (user secrets or Key Vault; replaced by a managed identity in phase 2).</summary>
    [Required]
    public string Key { get; set; } = string.Empty;

    /// <summary>Azure region of the Speech resource, e.g. "francecentral".</summary>
    [Required]
    public string Region { get; set; } = string.Empty;

    /// <summary>Neural voice, e.g. fr-FR-DeniseNeural, fr-FR-HenriNeural, fr-FR-VivienneMultilingualNeural.</summary>
    [Required]
    public string VoiceName { get; set; } = "fr-FR-DeniseNeural";

    /// <summary>Maximum characters sent in one request; longer scripts are cut at paragraph ends.</summary>
    [Range(500, 9000)]
    public int MaxCharactersPerRequest { get; set; } = 3000;

    /// <summary>Pause between paragraphs, in milliseconds.</summary>
    [Range(0, 5000)]
    public int ParagraphPauseMilliseconds { get; set; } = 600;
}
