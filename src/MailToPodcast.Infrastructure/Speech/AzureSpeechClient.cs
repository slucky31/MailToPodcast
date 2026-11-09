using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Calls Azure AI Speech and returns MP3 audio (24 kHz, 96 kbit/s, mono).
/// </summary>
internal sealed class AzureSpeechClient(IOptions<AzureSpeechOptions> options) : ISpeechClient
{
    public async Task<byte[]> SynthesizeSsmlAsync(string ssml, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var config = SpeechConfig.FromSubscription(settings.Key, settings.Region);
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz96KBitRateMonoMp3);

        using var synthesizer = new SpeechSynthesizer(config, null);
        using var registration = cancellationToken.Register(() => _ = synthesizer.StopSpeakingAsync());
        using var result = await synthesizer.SpeakSsmlAsync(ssml);
        cancellationToken.ThrowIfCancellationRequested();

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            return result.AudioData;
        }

        var details = SpeechSynthesisCancellationDetails.FromResult(result);
        throw new SpeechSynthesisException($"Azure Speech failed ({details.Reason}, {details.ErrorCode}): {details.ErrorDetails}");
    }
}
