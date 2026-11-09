using System.Diagnostics;
using MailToPodcast.Core.Mails;
using Microsoft.Extensions.Logging;

namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Chains the steps of one mail: HTML to text, script, speech, storage.
/// </summary>
public sealed partial class EpisodePipeline(
    IHtmlToTextConverter htmlToTextConverter,
    IScriptWriter scriptWriter,
    ISpeechSynthesizer speechSynthesizer,
    IEpisodeStorage episodeStorage,
    ILogger<EpisodePipeline> logger) : IEpisodePipeline
{
    public async Task<Episode> ProcessAsync(IncomingMail mail, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mail);

        var text = ExtractText(mail);
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyMailException($"Mail '{mail.Id}' has no text to read.");
        }

        LogTextExtracted(mail.Id, text.Length);

        var stopwatch = Stopwatch.StartNew();
        var script = await scriptWriter.WriteAsync(mail.Subject, text, cancellationToken);
        LogScriptWritten(mail.Id, script.SourceLanguage, script.WasTranslated, script.Text.Length, stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        var audio = await speechSynthesizer.SynthesizeAsync(script, cancellationToken);
        LogAudioSynthesized(mail.Id, audio.Length, stopwatch.ElapsedMilliseconds);

        var location = await episodeStorage.SaveAsync(mail.Id, script, audio, cancellationToken);
        LogEpisodeStored(mail.Id, location);

        return new Episode(mail.Id, script, location, audio.Length);
    }

    private string ExtractText(IncomingMail mail)
    {
        if (!string.IsNullOrWhiteSpace(mail.HtmlBody))
        {
            return htmlToTextConverter.Convert(mail.HtmlBody);
        }

        return mail.TextBody?.Trim() ?? string.Empty;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Mail {MailId}: {Length} characters of text extracted")]
    private partial void LogTextExtracted(string mailId, int length);

    [LoggerMessage(Level = LogLevel.Information, Message = "Mail {MailId}: script written (language {Language}, translated {Translated}, {Length} characters) in {ElapsedMs} ms")]
    private partial void LogScriptWritten(string mailId, string language, bool translated, int length, long elapsedMs);

    [LoggerMessage(Level = LogLevel.Information, Message = "Mail {MailId}: {Bytes} bytes of audio synthesized in {ElapsedMs} ms")]
    private partial void LogAudioSynthesized(string mailId, int bytes, long elapsedMs);

    [LoggerMessage(Level = LogLevel.Information, Message = "Mail {MailId}: episode stored at {Location}")]
    private partial void LogEpisodeStored(string mailId, Uri location);
}
