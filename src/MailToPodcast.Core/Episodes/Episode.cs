namespace MailToPodcast.Core.Episodes;

/// <summary>
/// A podcast episode produced from a mail.
/// </summary>
/// <param name="Id">Episode id, equal to the mail id.</param>
/// <param name="Script">Script read in the episode.</param>
/// <param name="AudioLocation">Where the MP3 is stored.</param>
/// <param name="AudioSizeInBytes">Size of the MP3.</param>
public sealed record Episode(string Id, PodcastScript Script, Uri AudioLocation, long AudioSizeInBytes);
