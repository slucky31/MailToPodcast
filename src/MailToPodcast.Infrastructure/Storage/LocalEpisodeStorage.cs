using System.Globalization;
using System.Text;
using MailToPodcast.Core.Episodes;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Infrastructure.Storage;

/// <summary>
/// Writes {id}.mp3 and {id}.txt (the script, to proofread the rewrite) into a local folder.
/// </summary>
internal sealed class LocalEpisodeStorage(IOptions<LocalEpisodeStorageOptions> options) : IEpisodeStorage
{
    public async Task<Uri> SaveAsync(string episodeId, PodcastScript script, byte[] audio, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(episodeId);
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(audio);

        var directory = Directory.CreateDirectory(options.Value.Directory);
        var name = ToFileName(episodeId);
        var audioPath = Path.Combine(directory.FullName, name + ".mp3");

        await File.WriteAllBytesAsync(audioPath, audio, cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(directory.FullName, name + ".txt"), FormatScript(script), Encoding.UTF8, cancellationToken);

        return new Uri(audioPath);
    }

    internal static string ToFileName(string episodeId)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(episodeId.Select(c => invalid.Contains(c) ? '_' : c));
    }

    private static string FormatScript(PodcastScript script) => string.Create(
        CultureInfo.InvariantCulture,
        $"""
        {script.Title}
        Langue d'origine : {script.SourceLanguage}{(script.WasTranslated ? " (traduit en français)" : string.Empty)}

        {script.Text}

        """);
}
