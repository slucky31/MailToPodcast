using AwesomeAssertions;
using MailToPodcast.Infrastructure.Speech;

namespace MailToPodcast.Infrastructure.Tests.Speech;

public sealed class ScriptChunkerTests
{
    [Fact]
    public void Split_Should_GroupParagraphsUpToLimit_WhenScriptIsLong()
    {
        var chunks = ScriptChunker.Split("aaaa\n\nbbbb\n\ncccc", maxCharacters: 9);

        chunks.Should().HaveCount(2);
        chunks[0].Should().Equal("aaaa", "bbbb");
        chunks[1].Should().Equal("cccc");
    }

    [Fact]
    public void Split_Should_CutAtSentenceEnds_WhenOneParagraphExceedsLimit()
    {
        var chunks = ScriptChunker.Split("Une phrase. Deux phrases ! Trois ?", maxCharacters: 15);

        chunks.SelectMany(c => c).Should().Equal("Une phrase.", "Deux phrases !", "Trois ?");
    }

    [Fact]
    public void Split_Should_ReturnNoChunk_WhenScriptIsBlank()
    {
        ScriptChunker.Split(" \r\n\r\n ", maxCharacters: 100).Should().BeEmpty();
    }
}
