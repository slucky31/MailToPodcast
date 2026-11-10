using System.Text;

namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Cuts a script into pieces short enough for one synthesis request, at paragraph ends,
/// then at sentence ends for a paragraph that is too long on its own.
/// </summary>
internal static class ScriptChunker
{
    public static IReadOnlyList<IReadOnlyList<string>> Split(string text, int maxCharacters)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxCharacters, 1);

        var paragraphs = text
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .SelectMany(paragraph => SplitLongParagraph(paragraph, maxCharacters));

        var chunks = new List<IReadOnlyList<string>>();
        var current = new List<string>();
        var length = 0;
        foreach (var paragraph in paragraphs)
        {
            if (current.Count > 0 && length + paragraph.Length > maxCharacters)
            {
                chunks.Add(current);
                current = [];
                length = 0;
            }

            current.Add(paragraph);
            length += paragraph.Length;
        }

        if (current.Count > 0)
        {
            chunks.Add(current);
        }

        return chunks;
    }

    private static IEnumerable<string> SplitLongParagraph(string paragraph, int maxCharacters)
    {
        if (paragraph.Length <= maxCharacters)
        {
            yield return paragraph;
            yield break;
        }

        var piece = new StringBuilder();
        foreach (var sentence in SplitSentences(paragraph))
        {
            if (piece.Length > 0 && piece.Length + sentence.Length > maxCharacters)
            {
                yield return piece.ToString().Trim();
                piece.Clear();
            }

            piece.Append(sentence);
        }

        if (piece.Length > 0)
        {
            yield return piece.ToString().Trim();
        }
    }

    private static IEnumerable<string> SplitSentences(string paragraph)
    {
        var start = 0;
        for (var i = 0; i < paragraph.Length; i++)
        {
            var isEnd = paragraph[i] is '.' or '!' or '?' or '…';
            if (isEnd && (i + 1 == paragraph.Length || paragraph[i + 1] == ' '))
            {
                yield return paragraph[start..(i + 1)] + " ";
                start = i + 1;
            }
        }

        if (start < paragraph.Length)
        {
            yield return paragraph[start..];
        }
    }
}
