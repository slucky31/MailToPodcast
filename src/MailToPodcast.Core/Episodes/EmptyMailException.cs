namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Thrown when a mail has no text left to read once cleaned.
/// </summary>
public sealed class EmptyMailException : Exception
{
    public EmptyMailException()
    {
    }

    public EmptyMailException(string message)
        : base(message)
    {
    }

    public EmptyMailException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
