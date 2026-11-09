using AwesomeAssertions;
using MailToPodcast.Infrastructure.Mails;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailToPodcast.Infrastructure.Tests.Mails;

public sealed class EmlFileMailSourceTests : IDisposable
{
    private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory("mailtopodcast-");

    public void Dispose() => _directory.Delete(recursive: true);

    private string WriteEml(string name, string subject)
    {
        using var message = new MimeMessage { Subject = subject, Body = new TextPart("plain") { Text = "Corps" } };
        var path = Path.Combine(_directory.FullName, name);
        message.WriteTo(path);
        return path;
    }

    private static EmlFileMailSource CreateSource(string path) =>
        new(Options.Create(new EmlFileMailSourceOptions { Path = path }), NullLogger<EmlFileMailSource>.Instance);

    [Fact]
    public async Task GetPendingAsync_Should_ReadEveryEmlInOrder_WhenPathIsAFolder()
    {
        WriteEml("b.eml", "B");
        WriteEml("a.eml", "A");
        await File.WriteAllTextAsync(Path.Combine(_directory.FullName, "notes.txt"), "ignored", TestContext.Current.CancellationToken);

        var mails = await CreateSource(_directory.FullName).GetPendingAsync(TestContext.Current.CancellationToken);

        mails.Select(m => (m.Id, m.Subject)).Should().Equal(("a", "A"), ("b", "B"));
    }

    [Fact]
    public async Task GetPendingAsync_Should_ReadOneMail_WhenPathIsAFile()
    {
        var path = WriteEml("single.eml", "Seul");

        var mails = await CreateSource(path).GetPendingAsync(TestContext.Current.CancellationToken);

        mails.Should().ContainSingle().Which.Id.Should().Be("single");
    }

    [Fact]
    public async Task GetPendingAsync_Should_ThrowFileNotFound_WhenPathDoesNotExist()
    {
        var act = () => CreateSource(Path.Combine(_directory.FullName, "missing.eml")).GetPendingAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task MarkAsSentAsync_Should_LeaveFileInPlace_WhenCalled()
    {
        var path = WriteEml("keep.eml", "Garder");
        var source = CreateSource(path);
        var mail = (await source.GetPendingAsync(TestContext.Current.CancellationToken))[0];

        await source.MarkAsSentAsync(mail, TestContext.Current.CancellationToken);
        await source.MarkAsFailedAsync(mail, TestContext.Current.CancellationToken);

        File.Exists(path).Should().BeTrue();
    }
}
