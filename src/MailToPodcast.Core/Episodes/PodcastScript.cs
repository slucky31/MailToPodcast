namespace MailToPodcast.Core.Episodes;

/// <summary>
/// The text of an episode, rewritten in French for listening.
/// </summary>
/// <param name="Title">Episode title, in French.</param>
/// <param name="SourceLanguage">ISO 639-1 code of the mail's original language (e.g. "en").</param>
/// <param name="WasTranslated">True when the mail was translated into French.</param>
/// <param name="Text">Text to read aloud, paragraphs separated by blank lines.</param>
public sealed record PodcastScript(string Title, string SourceLanguage, bool WasTranslated, string Text);
