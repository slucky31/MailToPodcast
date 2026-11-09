using AwesomeAssertions;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace MailToPodcast.Infrastructure.Tests.Storage;

public sealed class LocalEpisodeStorageTests : IDisposable
{
    private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory("mailtopodcast-");

    public void Dispose() => _directory.Delete(recursive: true);

    [Fact]
    public async Task SaveAsync_Should_WriteMp3AndScript_WhenEpisodeIsSaved()
    {
        var output = Path.Combine(_directory.FullName, "out");
        var storage = new LocalEpisodeStorage(Options.Create(new LocalEpisodeStorageOptions { Directory = output }));
        var script = new PodcastScript("Titre", "en", true, "Texte lu.");

        var location = await storage.SaveAsync("digest", script, [1, 2, 3], TestContext.Current.CancellationToken);

        location.LocalPath.Should().Be(Path.Combine(output, "digest.mp3"));
        (await File.ReadAllBytesAsync(location.LocalPath, TestContext.Current.CancellationToken)).Should().Equal(1, 2, 3);
        var text = await File.ReadAllTextAsync(Path.Combine(output, "digest.txt"), TestContext.Current.CancellationToken);
        text.Should().Contain("Titre").And.Contain("Langue d'origine : en (traduit en français)").And.Contain("Texte lu.");
    }

    [Fact]
    public void ToFileName_Should_ReplaceInvalidCharacters_WhenIdContainsSlashes()
    {
        LocalEpisodeStorage.ToFileName("a/b\0c").Should().Be("a_b_c");
    }
}
