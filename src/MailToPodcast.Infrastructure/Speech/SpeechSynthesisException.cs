namespace MailToPodcast.Infrastructure.Speech;

/// <summary>
/// Thrown when Azure Speech does not return audio.
/// </summary>
public sealed class SpeechSynthesisException : Exception
{
    public SpeechSynthesisException()
    {
    }

    public SpeechSynthesisException(string message)
        : base(message)
    {
    }

    public SpeechSynthesisException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
