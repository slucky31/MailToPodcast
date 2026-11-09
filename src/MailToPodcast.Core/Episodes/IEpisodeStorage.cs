namespace MailToPodcast.Core.Episodes;

/// <summary>
/// Stores the audio and the script of an episode (local folder for the CLI, Blob Storage in production).
/// </summary>
public interface IEpisodeStorage
{
    /// <summary>Saves the episode under its id, overwriting a previous save, and returns where the audio lives.</summary>
    Task<Uri> SaveAsync(string episodeId, PodcastScript script, byte[] audio, CancellationToken cancellationToken);
}
