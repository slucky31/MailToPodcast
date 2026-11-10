namespace MailToPodcast.Core.Mails;

/// <summary>
/// Turns the HTML body of a mail into readable plain text.
/// </summary>
public interface IHtmlToTextConverter
{
    /// <summary>Converts HTML into plain text, paragraphs separated by blank lines.</summary>
    string Convert(string html);
}
