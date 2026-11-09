namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Thin wrapper over the Speech SDK, so chunking and assembly can be tested without calling Azure.
/// </summary>
internal interface ISpeechClient
{
    /// <summary>Synthesizes one SSML document and returns its MP3 audio.</summary>
    Task<byte[]> SynthesizeSsmlAsync(string ssml, CancellationToken cancellationToken);
}
