using AwesomeAssertions;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Infrastructure.Speech;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace MailToPodcast.Infrastructure.Tests.Speech;

public sealed class AzureSpeechSynthesizerTests
{
    private readonly ISpeechClient _client = Substitute.For<ISpeechClient>();

    [Fact]
    public async Task SynthesizeAsync_Should_ConcatenateAudioOfEachChunkInOrder_WhenScriptNeedsSeveralRequests()
    {
        var options = Options.Create(new AzureSpeechOptions { VoiceName = "fr-FR-DeniseNeural", MaxCharactersPerRequest = 500, ParagraphPauseMilliseconds = 0 });
        var first = new string('a', 400);
        var second = new string('b', 400);
        _client.SynthesizeSsmlAsync(Arg.Is<string>(s => s.Contains(first)), Arg.Any<CancellationToken>()).Returns([1, 2]);
        _client.SynthesizeSsmlAsync(Arg.Is<string>(s => s.Contains(second)), Arg.Any<CancellationToken>()).Returns([3]);
        var synthesizer = new AzureSpeechSynthesizer(_client, options);

        var audio = await synthesizer.SynthesizeAsync(new PodcastScript("T", "fr", false, $"{first}\n\n{second}"), TestContext.Current.CancellationToken);

        audio.Should().Equal(1, 2, 3);
        await _client.Received(2).SynthesizeSsmlAsync(Arg.Is<string>(s => s.Contains("fr-FR-DeniseNeural")), Arg.Any<CancellationToken>());
    }
}
