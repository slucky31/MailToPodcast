using MailToPodcast.Core.Mails;
using MailToPodcast.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Writes the cleaned text of each mail next to the episodes, to check the HTML cleanup for free.
/// </summary>
internal static partial class TextOnly
{
    public static async Task RunAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var source = services.GetRequiredService<IMailSource>();
        var converter = services.GetRequiredService<IHtmlToTextConverter>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(TextOnly));
        var directory = Directory.CreateDirectory(services.GetRequiredService<IOptions<LocalEpisodeStorageOptions>>().Value.Directory);

        foreach (var mail in await source.GetPendingAsync(cancellationToken))
        {
            var text = string.IsNullOrWhiteSpace(mail.HtmlBody) ? mail.TextBody ?? string.Empty : converter.Convert(mail.HtmlBody);
            var path = Path.Combine(directory.FullName, mail.Id + ".clean.txt");
            await File.WriteAllTextAsync(path, $"{mail.Subject}\n\n{text}\n", cancellationToken);
            LogWritten(logger, mail.Subject, text.Length, path);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "{Subject}: {Length} characters -> {Path}")]
    private static partial void LogWritten(ILogger logger, string subject, int length, string path);
}
