namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Rewrites the text of a mail into a script for listening (language detection, translation into French, oral style).
/// </summary>
public interface IScriptWriter
{
    /// <summary>Writes the script of an episode from the subject and cleaned text of a mail.</summary>
    Task<PodcastScript> WriteAsync(string subject, string text, CancellationToken cancellationToken);
}
