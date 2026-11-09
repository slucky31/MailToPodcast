using MailToPodcast.Core.Episodes;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Reads a script with an Azure neural voice: the script is cut into requests of a few thousand characters,
/// read one after the other, and the MP3 pieces are joined (MP3 frames of the same format can be concatenated).
/// </summary>
internal sealed class AzureSpeechSynthesizer(ISpeechClient speechClient, IOptions<AzureSpeechOptions> options) : ISpeechSynthesizer
{
    public async Task<byte[]> SynthesizeAsync(PodcastScript script, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(script);
        var settings = options.Value;

        using var audio = new MemoryStream();
        foreach (var chunk in ScriptChunker.Split(script.Text, settings.MaxCharactersPerRequest))
        {
            var ssml = SsmlBuilder.Build(chunk, settings.VoiceName, settings.ParagraphPauseMilliseconds);
            var piece = await speechClient.SynthesizeSsmlAsync(ssml, cancellationToken);
            await audio.WriteAsync(piece, cancellationToken);
        }

        return audio.ToArray();
    }
}
