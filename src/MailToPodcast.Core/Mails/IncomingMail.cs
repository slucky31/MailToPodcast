namespace MailToPodcast.Core.Mails;

/// <summary>
/// A mail waiting to be turned into a podcast episode.
/// </summary>
/// <param name="Id">Stable identifier (Gmail message id, or file name for the CLI); used as the episode id.</param>
/// <param name="Subject">Mail subject.</param>
/// <param name="From">Sender, as displayed.</param>
/// <param name="ReceivedAt">Date of the mail, when known.</param>
/// <param name="HtmlBody">HTML part of the mail, when present.</param>
/// <param name="TextBody">Plain-text part of the mail, when present.</param>
public sealed record IncomingMail(
    string Id,
    string Subject,
    string? From,
    DateTimeOffset? ReceivedAt,
    string? HtmlBody,
    string? TextBody);
