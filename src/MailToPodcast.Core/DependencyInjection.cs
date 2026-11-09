using MailToPodcast.Core.Episodes;
using Microsoft.Extensions.DependencyInjection;

namespace MailToPodcast.Core;

/// <summary>
/// Registers the services of the core.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddMailToPodcastCore(this IServiceCollection services)
    {
        services.AddTransient<IEpisodePipeline, EpisodePipeline>();
        services.AddTransient<MailBatchProcessor>();
        return services;
    }
}
