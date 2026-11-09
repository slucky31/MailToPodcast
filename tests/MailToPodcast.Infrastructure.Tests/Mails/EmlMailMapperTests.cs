using AwesomeAssertions;
using MailToPodcast.Infrastructure.Mails;
using MimeKit;

namespace MailToPodcast.Infrastructure.Tests.Mails;

public sealed class EmlMailMapperTests
{
    [Fact]
    public void Map_Should_CopyHeadersAndBodies_WhenMessageIsMultipart()
    {
        using var message = new MimeMessage { Subject = " Weekly digest ", Date = new DateTimeOffset(2026, 11, 9, 8, 0, 0, TimeSpan.Zero) };
        message.From.Add(new MailboxAddress("Newsletter", "news@example.com"));
        message.Body = new BodyBuilder { HtmlBody = "<p>Hello</p>", TextBody = "Hello" }.ToMessageBody();

        var mail = EmlMailMapper.Map(message, "digest");

        mail.Id.Should().Be("digest");
        mail.Subject.Should().Be("Weekly digest");
        mail.From.Should().Contain("news@example.com");
        mail.ReceivedAt.Should().Be(new DateTimeOffset(2026, 11, 9, 8, 0, 0, TimeSpan.Zero));
        mail.HtmlBody.Should().Be("<p>Hello</p>");
        mail.TextBody.Should().Be("Hello");
    }

    [Fact]
    public void Map_Should_UseDefaultSubjectAndNoSender_WhenHeadersAreMissing()
    {
        using var message = new MimeMessage { Body = new TextPart("plain") { Text = "Texte" } };
        message.Headers.Remove(HeaderId.Date);

        var mail = EmlMailMapper.Map(message, "x");

        mail.Subject.Should().Be("(sans objet)");
        mail.From.Should().BeNull();
        mail.ReceivedAt.Should().BeNull();
        mail.HtmlBody.Should().BeNull();
        mail.TextBody.Should().Be("Texte");
    }
}
