using AwesomeAssertions;
using MailToPodcast.Infrastructure.Speech;

namespace MailToPodcast.Infrastructure.Tests.Speech;

public sealed class SsmlBuilderTests
{
    [Fact]
    public void Build_Should_WrapParagraphsWithVoiceAndPauses_WhenGivenSeveralParagraphs()
    {
        var ssml = SsmlBuilder.Build(["Un", "Deux"], "fr-FR-HenriNeural", 600);

        ssml.Should().Be(
            "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"fr-FR\">"
            + "<voice name=\"fr-FR-HenriNeural\"><p>Un</p><break time=\"600ms\"/><p>Deux</p></voice></speak>");
    }

    [Fact]
    public void Build_Should_EscapeXml_WhenTextHasSpecialCharacters()
    {
        var ssml = SsmlBuilder.Build(["Tom & Jerry <3"], "fr-FR-DeniseNeural", 0);

        ssml.Should().Contain("<p>Tom &amp; Jerry &lt;3</p>").And.NotContain("<break");
    }
}
