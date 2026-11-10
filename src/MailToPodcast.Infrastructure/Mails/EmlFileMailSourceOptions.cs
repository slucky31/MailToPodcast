using System.ComponentModel.DataAnnotations;

namespace MailToPodcast.Infrastructure.Mails;

/// <summary>
/// Settings of the .eml file source used by the CLI.
/// </summary>
public sealed class EmlFileMailSourceOptions
{
    public const string SectionName = "Input";

    /// <summary>An .eml file, or a folder whose .eml files are all read.</summary>
    [Required]
    public string Path { get; set; } = string.Empty;
}
