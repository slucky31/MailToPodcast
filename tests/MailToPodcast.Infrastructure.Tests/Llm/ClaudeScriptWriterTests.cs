using System.Text.Json;
using AwesomeAssertions;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Infrastructure.Llm;
using NSubstitute;

namespace MailToPodcast.Infrastructure.Tests.Llm;

public sealed class ClaudeScriptWriterTests
{
    private readonly IClaudeClient _client = Substitute.For<IClaudeClient>();
    private readonly ClaudeScriptWriter _writer;

    public ClaudeScriptWriterTests()
    {
        _writer = new ClaudeScriptWriter(_client);
    }

    private void AnswerWith(string json) => _client
        .CompleteJsonAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, JsonElement>>(), Arg.Any<CancellationToken>())
        .Returns(json);

    [Fact]
    public async Task WriteAsync_Should_ReturnParsedScript_WhenClaudeAnswersValidJson()
    {
        AnswerWith("""{"title":" L'IA cette semaine ","source_language":"EN","was_translated":true,"script":" Bonjour.\n\nSuite. "}""");

        var script = await _writer.WriteAsync("AI this week", "Hello.", TestContext.Current.CancellationToken);

        script.Should().Be(new PodcastScript("L'IA cette semaine", "en", true, "Bonjour.\n\nSuite."));
    }

    [Fact]
    public async Task WriteAsync_Should_WrapMailInNewsletterTagsWithSubject_WhenCallingClaude()
    {
        AnswerWith("""{"title":"T","source_language":"fr","was_translated":false,"script":"S"}""");

        await _writer.WriteAsync("Mon objet", "Le texte du mail", TestContext.Current.CancellationToken);

        await _client.Received(1).CompleteJsonAsync(
            ClaudeScriptWriter.SystemPrompt,
            Arg.Is<string>(m => m.Contains("Objet du mail : Mon objet") && m.Contains("<newsletter>\nLe texte du mail\n</newsletter>")),
            Arg.Is<IReadOnlyDictionary<string, JsonElement>>(schema => schema["required"].GetArrayLength() == 4),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"title":"T","source_language":"fr","was_translated":false,"script":"  "}""")]
    [InlineData("""{"title":"","source_language":"fr","was_translated":false,"script":"S"}""")]
    [InlineData("null")]
    public async Task WriteAsync_Should_ThrowClaudeResponseException_WhenAnswerIsUnusable(string json)
    {
        AnswerWith(json);

        var act = () => _writer.WriteAsync("Objet", "Texte", TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ClaudeResponseException>();
    }

    [Fact]
    public async Task WriteAsync_Should_UseUndeterminedLanguage_WhenClaudeGivesNone()
    {
        AnswerWith("""{"title":"T","source_language":"","was_translated":false,"script":"S"}""");

        var script = await _writer.WriteAsync("Objet", "Texte", TestContext.Current.CancellationToken);

        script.SourceLanguage.Should().Be("und");
    }
}
