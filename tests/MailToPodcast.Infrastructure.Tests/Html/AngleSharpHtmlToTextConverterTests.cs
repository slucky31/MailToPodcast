using AwesomeAssertions;
using MailToPodcast.Infrastructure.Html;

namespace MailToPodcast.Infrastructure.Tests.Html;

public sealed class AngleSharpHtmlToTextConverterTests
{
    private readonly AngleSharpHtmlToTextConverter _converter = new();

    [Fact]
    public void Convert_Should_SeparateBlocksWithBlankLines_WhenHtmlHasParagraphsAndHeadings()
    {
        var text = _converter.Convert("<html><body><h1>Titre</h1><p>Premier   paragraphe.</p><div>Second<br>bloc</div></body></html>");

        text.Should().Be("Titre\n\nPremier paragraphe.\n\nSecond\nbloc");
    }

    [Fact]
    public void Convert_Should_DropScriptsStylesImagesAndHiddenBlocks_WhenPresent()
    {
        const string html = """
            <html><head><title>Head</title><style>p { color: red; }</style></head>
            <body>
              <div style="display:none">Preheader caché</div>
              <span hidden>Caché aussi</span>
              <script>alert('x')</script>
              <img src="logo.png" alt="Logo">
              <p>Visible</p>
            </body></html>
            """;

        var text = _converter.Convert(html);

        text.Should().Be("Visible");
    }

    [Fact]
    public void Convert_Should_PrefixListItems_WhenHtmlHasLists()
    {
        var text = _converter.Convert("<ul><li>Un</li><li>Deux</li></ul>");

        text.Should().Be("- Un\n\n- Deux");
    }

    [Fact]
    public void Convert_Should_KeepLinkTextAndDropUnsubscribeLines_WhenNewsletterHasFooter()
    {
        const string html = """
            <p>Lisez <a href="https://example.com/article">cet article</a> passionnant.</p>
            <p>View this email in your browser</p>
            <p><a href="https://example.com/u">Se désinscrire</a> de cette liste.</p>
            """;

        var text = _converter.Convert(html);

        text.Should().Be("Lisez cet article passionnant.");
    }

    [Fact]
    public void Convert_Should_RemoveZeroWidthCharacters_WhenNewsletterUsesThemAsSpacers()
    {
        var text = _converter.Convert("<p>\u200B</p><p>Mi\u00ADlan\u200C\uFEFF</p>");

        text.Should().Be("Milan");
    }

    [Fact]
    public void Convert_Should_ReturnEmptyText_WhenHtmlHasNoReadableContent()
    {
        var text = _converter.Convert("<html><body><img src='x.png'><table><tr><td>&nbsp;</td></tr></table></body></html>");

        text.Should().BeEmpty();
    }
}
