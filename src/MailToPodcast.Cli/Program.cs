using MailToPodcast.Core.Episodes;
using MailToPodcast.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

// Phase 1 test bench: turns one .eml file (or every .eml of a folder) into an MP3 and its script.
//   dotnet run --project src/MailToPodcast.Cli -- --input mail.eml [--voice fr-FR-HenriNeural] [--output episodes]
// --text-only true stops after the HTML cleanup and writes {id}.clean.txt: no API is called, nothing is billed.
var switchMappings = new Dictionary<string, string>(StringComparer.Ordinal)
{
    ["--input"] = "Input:Path",
    ["--output"] = "Output:Directory",
    ["--voice"] = "Speech:VoiceName",
    ["--model"] = "Claude:Model",
    ["--max"] = "Batch:MaxMailsPerRun",
    ["--text-only"] = "TextOnly",
};

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { Args = args, ContentRootPath = AppContext.BaseDirectory });
builder.Configuration.AddUserSecrets<Program>(optional: true);
builder.Configuration.AddCommandLine(args, switchMappings);

builder.Services.AddSerilog((services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));
builder.Services.AddMailToPodcastLocal(builder.Configuration);

using var host = builder.Build();

if (builder.Configuration.GetValue<bool>("TextOnly"))
{
    // Not started on purpose: starting validates the Claude and Speech settings, which this mode does not need.
    await TextOnly.RunAsync(host.Services, CancellationToken.None);
    await Log.CloseAndFlushAsync();
    return 0;
}

await host.StartAsync();

var processor = host.Services.GetRequiredService<MailBatchProcessor>();
var report = await processor.RunAsync(CancellationToken.None);

var logger = host.Services.GetRequiredService<ILogger<Program>>();
foreach (var episode in report.Episodes)
{
    logger.LogInformation("Episode {Title} ({Language}, translated: {Translated}) -> {Location}", episode.Script.Title, episode.Script.SourceLanguage, episode.Script.WasTranslated, episode.AudioLocation.LocalPath);
}

logger.LogInformation("{Succeeded} episode(s) produced, {Failed} mail(s) failed", report.Episodes.Count, report.FailedMailIds.Count);

await host.StopAsync();
await Log.CloseAndFlushAsync();
return report.IsSuccess ? 0 : 1;
