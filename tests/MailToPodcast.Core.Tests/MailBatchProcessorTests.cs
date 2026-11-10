using AwesomeAssertions;
using MailToPodcast.Core.Episodes;
using MailToPodcast.Core.Mails;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MailToPodcast.Core.Tests;

public sealed class MailBatchProcessorTests
{
    private readonly IMailSource _source = Substitute.For<IMailSource>();
    private readonly IEpisodePipeline _pipeline = Substitute.For<IEpisodePipeline>();

    private MailBatchProcessor CreateProcessor(int maxMailsPerRun = 3) => new(
        _source,
        _pipeline,
        Options.Create(new BatchOptions { MaxMailsPerRun = maxMailsPerRun }),
        NullLogger<MailBatchProcessor>.Instance);

    private static Episode EpisodeFor(IncomingMail mail) => new(mail.Id, MailBuilder.Script(), new Uri($"file:///{mail.Id}.mp3"), 1);

    [Fact]
    public async Task RunAsync_Should_MarkMailAsSent_WhenEpisodeIsStored()
    {
        var mail = MailBuilder.Html("a");
        _source.GetPendingAsync(Arg.Any<CancellationToken>()).Returns([mail]);
        _pipeline.ProcessAsync(mail, Arg.Any<CancellationToken>()).Returns(EpisodeFor(mail));

        var report = await CreateProcessor().RunAsync(TestContext.Current.CancellationToken);

        report.IsSuccess.Should().BeTrue();
        report.Episodes.Should().ContainSingle().Which.Id.Should().Be("a");
        Received.InOrder(() =>
        {
            _pipeline.ProcessAsync(mail, Arg.Any<CancellationToken>());
            _source.MarkAsSentAsync(mail, Arg.Any<CancellationToken>());
        });
        await _source.DidNotReceiveWithAnyArgs().MarkAsFailedAsync(default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task RunAsync_Should_MarkMailAsFailedAndContinue_WhenOneMailFails()
    {
        var failing = MailBuilder.Html("a");
        var working = MailBuilder.Html("b");
        _source.GetPendingAsync(Arg.Any<CancellationToken>()).Returns([failing, working]);
        _pipeline.ProcessAsync(failing, Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("boom"));
        _pipeline.ProcessAsync(working, Arg.Any<CancellationToken>()).Returns(EpisodeFor(working));

        var report = await CreateProcessor().RunAsync(TestContext.Current.CancellationToken);

        report.IsSuccess.Should().BeFalse();
        report.FailedMailIds.Should().Equal("a");
        report.Episodes.Should().ContainSingle().Which.Id.Should().Be("b");
        await _source.Received(1).MarkAsFailedAsync(failing, Arg.Any<CancellationToken>());
        await _source.DidNotReceive().MarkAsSentAsync(failing, Arg.Any<CancellationToken>());
        await _source.Received(1).MarkAsSentAsync(working, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_Should_ProcessAtMostMaxMails_WhenMoreArePending()
    {
        var mails = new[] { MailBuilder.Html("a"), MailBuilder.Html("b"), MailBuilder.Html("c") };
        _source.GetPendingAsync(Arg.Any<CancellationToken>()).Returns(mails);
        _pipeline.ProcessAsync(Arg.Any<IncomingMail>(), Arg.Any<CancellationToken>())
            .Returns(call => EpisodeFor(call.Arg<IncomingMail>()));

        var report = await CreateProcessor(maxMailsPerRun: 2).RunAsync(TestContext.Current.CancellationToken);

        report.Episodes.Select(e => e.Id).Should().Equal("a", "b");
        await _pipeline.DidNotReceive().ProcessAsync(mails[2], Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_Should_NotMarkMailAsFailed_WhenRunIsCancelled()
    {
        var mail = MailBuilder.Html("a");
        _source.GetPendingAsync(Arg.Any<CancellationToken>()).Returns([mail]);
        _pipeline.ProcessAsync(mail, Arg.Any<CancellationToken>()).ThrowsAsync(new OperationCanceledException());

        var act = () => CreateProcessor().RunAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<OperationCanceledException>();
        await _source.DidNotReceiveWithAnyArgs().MarkAsFailedAsync(default!, TestContext.Current.CancellationToken);
    }
}
