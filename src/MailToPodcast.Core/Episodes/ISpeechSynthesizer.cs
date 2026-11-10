namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Reads a script aloud.
/// </summary>
public interface ISpeechSynthesizer
{
    /// <summary>Returns the MP3 audio of the script.</summary>
    Task<byte[]> SynthesizeAsync(PodcastScript script, CancellationToken cancellationToken);
}
