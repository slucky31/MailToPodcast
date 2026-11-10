using System.ComponentModel.DataAnnotations;

namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Settings of a processing run.
/// </summary>
public sealed class BatchOptions
{
    public const string SectionName = "Batch";

    /// <summary>Maximum number of mails processed in one run.</summary>
    [Range(1, 100)]
    public int MaxMailsPerRun { get; set; } = 3;
}
