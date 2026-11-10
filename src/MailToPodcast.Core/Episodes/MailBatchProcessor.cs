using MailToPodcast.Core.Mails;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Processes the pending mails one at a time: a mail is marked as sent only once its episode is stored,
/// and a failed mail is marked as failed without stopping the others.
/// </summary>
public sealed partial class MailBatchProcessor(
    IMailSource mailSource,
    IEpisodePipeline pipeline,
    IOptions<BatchOptions> options,
    ILogger<MailBatchProcessor> logger)
{
    public async Task<BatchReport> RunAsync(CancellationToken cancellationToken)
    {
        var pending = await mailSource.GetPendingAsync(cancellationToken);
        var batch = pending.Take(options.Value.MaxMailsPerRun).ToList();
        LogBatchStarted(pending.Count, batch.Count);

        var episodes = new List<Episode>();
        var failed = new List<string>();
        foreach (var mail in batch)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var episode = await ProcessOneAsync(mail, cancellationToken);
            if (episode is null)
            {
                failed.Add(mail.Id);
            }
            else
            {
                episodes.Add(episode);
            }
        }

        return new BatchReport(episodes, failed);
    }

    private async Task<Episode?> ProcessOneAsync(IncomingMail mail, CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope(new Dictionary<string, object> { ["MailId"] = mail.Id, ["Subject"] = mail.Subject });
        Episode episode;
        try
        {
            episode = await pipeline.ProcessAsync(mail, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogMailFailed(exception, mail.Id);
            await mailSource.MarkAsFailedAsync(mail, cancellationToken);
            return null;
        }

        await mailSource.MarkAsSentAsync(mail, cancellationToken);
        return episode;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "{Pending} pending mail(s), {Processed} processed in this run")]
    private partial void LogBatchStarted(int pending, int processed);

    [LoggerMessage(Level = LogLevel.Error, Message = "Mail {MailId} could not be turned into an episode")]
    private partial void LogMailFailed(Exception exception, string mailId);
}
