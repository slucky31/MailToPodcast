using MailToPodcast.Core.Mails;

namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Turns one mail into a stored podcast episode.
/// </summary>
public interface IEpisodePipeline
{
    /// <summary>Cleans, rewrites, reads aloud and stores the mail.</summary>
    Task<Episode> ProcessAsync(IncomingMail mail, CancellationToken cancellationToken);
}
