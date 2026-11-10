using MailToPodcast.Core;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Core.Mails;
using MailToPodcast.Infrastructure.Html;
using MailToPodcast.Infrastructure.Llm;
using MailToPodcast.Infrastructure.Mails;
using MailToPodcast.Infrastructure.Speech;
using MailToPodcast.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailToPodcast.Infrastructure;

/// <summary>
/// Registers the core and the adapters of phase 1 (.eml files in, local folder out).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddMailToPodcastLocal(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddMailToPodcastCore();
        AddValidatedOptions<BatchOptions>(services, configuration, BatchOptions.SectionName);
        AddValidatedOptions<EmlFileMailSourceOptions>(services, configuration, EmlFileMailSourceOptions.SectionName);
        AddValidatedOptions<ClaudeOptions>(services, configuration, ClaudeOptions.SectionName);
        AddValidatedOptions<AzureSpeechOptions>(services, configuration, AzureSpeechOptions.SectionName);
        AddValidatedOptions<LocalEpisodeStorageOptions>(services, configuration, LocalEpisodeStorageOptions.SectionName);

        services.AddSingleton<IMailSource, EmlFileMailSource>();
        services.AddSingleton<IHtmlToTextConverter, AngleSharpHtmlToTextConverter>();
        services.AddSingleton<IClaudeClient, AnthropicClaudeClient>();
        services.AddSingleton<IScriptWriter, ClaudeScriptWriter>();
        services.AddSingleton<ISpeechClient, AzureSpeechClient>();
        services.AddSingleton<ISpeechSynthesizer, AzureSpeechSynthesizer>();
        services.AddSingleton<IEpisodeStorage, LocalEpisodeStorage>();
        return services;
    }

    private static void AddValidatedOptions<TOptions>(IServiceCollection services, IConfiguration configuration, string section)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(section))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
