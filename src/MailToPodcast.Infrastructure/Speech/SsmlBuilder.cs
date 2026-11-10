using System.Globalization;
using System.Security;
using System.Text;

namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Builds the SSML document of one synthesis request.
/// </summary>
internal static class SsmlBuilder
{
    public static string Build(IReadOnlyList<string> paragraphs, string voiceName, int pauseMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(paragraphs);
        ArgumentException.ThrowIfNullOrWhiteSpace(voiceName);

        var builder = new StringBuilder()
            .Append("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"fr-FR\">")
            .Append(CultureInfo.InvariantCulture, $"<voice name=\"{SecurityElement.Escape(voiceName)}\">");

        for (var i = 0; i < paragraphs.Count; i++)
        {
            if (i > 0 && pauseMilliseconds > 0)
            {
                builder.Append(CultureInfo.InvariantCulture, $"<break time=\"{pauseMilliseconds}ms\"/>");
            }

            builder.Append("<p>").Append(SecurityElement.Escape(paragraphs[i])).Append("</p>");
        }

        return builder.Append("</voice></speak>").ToString();
    }
}
