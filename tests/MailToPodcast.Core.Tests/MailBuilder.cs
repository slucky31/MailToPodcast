using MailToPodcast.Core.Episodes;
using MailToPodcast.Core.Mails;

namespace MailToPodcast.Core.Tests;

internal static class MailBuilder
{
    public static IncomingMail Html(string id = "mail-1", string html = "<p>Bonjour</p>") =>
        new(id, "Sujet", "Auteur <auteur@example.com>", DateTimeOffset.UnixEpoch, html, null);

    public static IncomingMail Text(string id = "mail-1", string? text = "Bonjour") =>
        new(id, "Sujet", null, null, null, text);

    public static PodcastScript Script(string text = "Texte lu") => new("Titre", "en", true, text);
}
