using System.ComponentModel.DataAnnotations;

namespace MailToPodcast.Infrastructure.Storage;

/// <summary>
/// Settings of the local folder where the CLI writes episodes.
/// </summary>
public sealed class LocalEpisodeStorageOptions
{
    public const string SectionName = "Output";

    /// <summary>Folder where the MP3 and script files are written.</summary>
    [Required]
    public string Directory { get; set; } = "episodes";
}
