namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Outcome of a processing run.
/// </summary>
/// <param name="Episodes">Episodes produced.</param>
/// <param name="FailedMailIds">Ids of the mails that failed.</param>
public sealed record BatchReport(IReadOnlyList<Episode> Episodes, IReadOnlyList<string> FailedMailIds)
{
    /// <summary>True when no mail failed.</summary>
    public bool IsSuccess => FailedMailIds.Count == 0;
}
