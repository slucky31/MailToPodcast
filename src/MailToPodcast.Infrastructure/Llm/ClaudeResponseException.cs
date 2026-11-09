namespace MailToPodcast.Infrastructure.Llm;

/// <summary>
/// Thrown when Claude gives no usable answer (refusal, answer cut off by the token limit, invalid JSON).
/// </summary>
public sealed class ClaudeResponseException : Exception
{
    public ClaudeResponseException()
    {
    }

    public ClaudeResponseException(string message)
        : base(message)
    {
    }

    public ClaudeResponseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
