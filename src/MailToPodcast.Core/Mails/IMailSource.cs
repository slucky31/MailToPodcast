namespace MailToPodcast.Core.Mails;

/// <summary>
/// Source of the mails to turn into podcasts (Gmail label in production, .eml files for the CLI).
/// </summary>
public interface IMailSource
{
    /// <summary>Returns the mails waiting to be processed.</summary>
    Task<IReadOnlyList<IncomingMail>> GetPendingAsync(CancellationToken cancellationToken);

    /// <summary>Marks a mail as processed, so it is not picked up again.</summary>
    Task MarkAsSentAsync(IncomingMail mail, CancellationToken cancellationToken);

    /// <summary>Marks a mail as failed, so it is not retried until a person asks for it.</summary>
    Task MarkAsFailedAsync(IncomingMail mail, CancellationToken cancellationToken);
}
