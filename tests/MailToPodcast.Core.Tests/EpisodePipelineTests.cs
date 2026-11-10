using AwesomeAssertions;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Core.Mails;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MailToPodcast.Core.Tests;

public sealed class EpisodePipelineTests
{
    private readonly IHtmlToTextConverter _converter = Substitute.For<IHtmlToTextConverter>();
    private readonly IScriptWriter _scriptWriter = Substitute.For<IScriptWriter>();
    private readonly ISpeechSynthesizer _synthesizer = Substitute.For<ISpeechSynthesizer>();
    private readonly IEpisodeStorage _storage = Substitute.For<IEpisodeStorage>();
    private readonly EpisodePipeline _pipeline;

    public EpisodePipelineTests()
    {
        _pipeline = new EpisodePipeline(_converter, _scriptWriter, _synthesizer, _storage, NullLogger<EpisodePipeline>.Instance);
    }

    [Fact]
    public async Task ProcessAsync_Should_ChainEveryStep_WhenMailHasHtml()
    {
        var mail = MailBuilder.Html(html: "<p>Hello</p>");
        var script = MailBuilder.Script();
        var audio = new byte[] { 1, 2, 3 };
        var location = new Uri("file:///episodes/mail-1.mp3");
        _converter.Convert("<p>Hello</p>").Returns("Hello");
        _scriptWriter.WriteAsync("Sujet", "Hello", Arg.Any<CancellationToken>()).Returns(script);
        _synthesizer.SynthesizeAsync(script, Arg.Any<CancellationToken>()).Returns(audio);
        _storage.SaveAsync("mail-1", script, audio, Arg.Any<CancellationToken>()).Returns(location);

        var episode = await _pipeline.ProcessAsync(mail, TestContext.Current.CancellationToken);

        episode.Should().Be(new Episode("mail-1", script, location, 3));
    }

    [Fact]
    public async Task ProcessAsync_Should_UseTextBody_WhenMailHasNoHtml()
    {
        var mail = MailBuilder.Text(text: "  Texte brut  ");
        _scriptWriter.WriteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(MailBuilder.Script());
        _synthesizer.SynthesizeAsync(Arg.Any<PodcastScript>(), Arg.Any<CancellationToken>()).Returns([]);
        _storage.SaveAsync(default!, default!, default!, TestContext.Current.CancellationToken).ReturnsForAnyArgs(new Uri("file:///x.mp3"));

        await _pipeline.ProcessAsync(mail, TestContext.Current.CancellationToken);

        _converter.DidNotReceiveWithAnyArgs().Convert(default!);
        await _scriptWriter.Received(1).WriteAsync("Sujet", "Texte brut", Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task ProcessAsync_Should_ThrowEmptyMailException_WhenMailHasNoText(string? text)
    {
        var mail = MailBuilder.Text(text: text);

        var act = () => _pipeline.ProcessAsync(mail, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<EmptyMailException>();
        await _scriptWriter.DidNotReceiveWithAnyArgs().WriteAsync(default!, default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ProcessAsync_Should_NotStoreAnything_WhenSynthesisFails()
    {
        var mail = MailBuilder.Html();
        _converter.Convert(Arg.Any<string>()).Returns("Bonjour");
        _scriptWriter.WriteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(MailBuilder.Script());
        _synthesizer.SynthesizeAsync(Arg.Any<PodcastScript>(), Arg.Any<CancellationToken>())
            .Returns<byte[]>(_ => throw new InvalidOperationException("speech down"));

        var act = () => _pipeline.ProcessAsync(mail, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _storage.DidNotReceiveWithAnyArgs().SaveAsync(default!, default!, default!, TestContext.Current.CancellationToken);
    }
}
