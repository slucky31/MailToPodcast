using MailToPodcast.Core.Mails;
using MimeKit;

namespace MailToPodcast.Infrastructure.Mails;

/// <summary>
/// Maps a MIME message to an <see cref="IncomingMail"/>.
/// </summary>
internal static class EmlMailMapper
{
    public static IncomingMail Map(MimeMessage message, string id)
    {
        ArgumentNullException.ThrowIfNull(message);

        return new IncomingMail(
            id,
            string.IsNullOrWhiteSpace(message.Subject) ? "(sans objet)" : message.Subject.Trim(),
            message.From.Count > 0 ? message.From.ToString() : null,
            message.Date == DateTimeOffset.MinValue ? null : message.Date,
            message.HtmlBody,
            message.TextBody);
    }
}
