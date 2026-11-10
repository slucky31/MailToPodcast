using MailToPodcast.Core.Mails;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailToPodcast.Infrastructure.Mails;

/// <summary>
/// Reads mails from .eml files. Used by the CLI to try the transformation without Gmail:
/// files are never moved, so marking a mail only logs it.
/// </summary>
internal sealed partial class EmlFileMailSource(
    IOptions<EmlFileMailSourceOptions> options,
    ILogger<EmlFileMailSource> logger) : IMailSource
{
    public async Task<IReadOnlyList<IncomingMail>> GetPendingAsync(CancellationToken cancellationToken)
    {
        var mails = new List<IncomingMail>();
        foreach (var file in ListFiles(options.Value.Path))
        {
            var message = await MimeMessage.LoadAsync(file, cancellationToken);
            mails.Add(EmlMailMapper.Map(message, Path.GetFileNameWithoutExtension(file)));
        }

        return mails;
    }

    public Task MarkAsSentAsync(IncomingMail mail, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mail);
        LogMarked(mail.Id, "sent");
        return Task.CompletedTask;
    }

    public Task MarkAsFailedAsync(IncomingMail mail, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mail);
        LogMarked(mail.Id, "failed");
        return Task.CompletedTask;
    }

    internal static IReadOnlyList<string> ListFiles(string path)
    {
        if (File.Exists(path))
        {
            return [path];
        }

        if (Directory.Exists(path))
        {
            return [.. Directory.EnumerateFiles(path, "*.eml").Order(StringComparer.Ordinal)];
        }

        throw new FileNotFoundException($"No .eml file or folder at '{path}'.", path);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Mail {MailId} marked as {State} (the .eml file is left in place)")]
    private partial void LogMarked(string mailId, string state);
}
