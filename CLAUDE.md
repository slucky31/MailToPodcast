# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

MailToPodcast turns newsletters received by mail into podcast episodes: HTML cleanup, rewrite for listening with the Claude API (language detection, translation into French, oral style, near-full reading), French neural voice with Azure AI Speech, MP3 output.

- **Phase 1 (current):** `MailToPodcast.Cli` reads `.eml` files and writes the MP3 and its script into a local folder.
- **Phase 2 (planned):** an Azure Function (isolated, Flex Consumption, timer every 5 min) reads Gmail mails labelled « À écouter », relabels them « Envoyé en podcast », uploads the MP3 to Blob Storage and updates a private RSS `feed.xml`. Secrets in Key Vault, infrastructure in Bicep.

## Architecture Decision Records

Structuring decisions live in `docs/adr/` (index in `docs/adr/README.md`, template in `docs/adr/template.md`). Read the relevant ADRs before changing an architectural choice; record a new decision as a new ADR (`docs(adr): ...`) and never rewrite an Accepted one: supersede it.

## Build & Development Commands

```bash
dotnet restore
dotnet build MailToPodcast.slnx
dotnet test --solution MailToPodcast.slnx
dotnet test --solution MailToPodcast.slnx --coverage --coverage-output-format cobertura

dotnet format --verify-no-changes            # Check compliance
dotnet format                                 # Auto-fix

# Try the transformation (see README for the secrets)
dotnet run --project src/MailToPodcast.Cli -- --input path/to/mail.eml
dotnet run --project src/MailToPodcast.Cli -- --input path/to/folder --text-only true   # cleanup only, no API call
```

## Architecture

```
src/MailToPodcast.Core/            → Models, ports (interfaces), EpisodePipeline, MailBatchProcessor (no SDK dependency)
src/MailToPodcast.Infrastructure/  → Adapters: Mails (MimeKit), Html (AngleSharp), Llm (Anthropic SDK), Speech (Azure Speech), Storage
src/MailToPodcast.Cli/             → Phase 1 composition root (generic host, Serilog, user secrets)
tests/                             → xUnit v3 + NSubstitute + AwesomeAssertions, one project per src project
```

**Layer rules:**
- Core references no external SDK (only `Microsoft.Extensions.*.Abstractions`/`Options`). Every external service is behind a Core port: `IMailSource`, `IHtmlToTextConverter`, `IScriptWriter`, `ISpeechSynthesizer`, `IEpisodeStorage`.
- SDKs that are hard to substitute sit behind a thin internal facade in Infrastructure (`IClaudeClient`, `ISpeechClient`), so the prompt, parsing, chunking and SSML stay unit-tested.
- A mail is marked as sent only after its episode is stored; a failed mail is marked as failed and the run goes on with the next one.
- The episode id is the mail id: storage and (phase 2) the RSS `guid` are deterministic, so a rerun overwrites instead of duplicating.

## Claude API

- Default model `claude-opus-5-5`, effort `low`, streaming, structured JSON output, server-side refusal fallback (`fallbacks: default`). All configurable in the `Claude` section.
- The mail content goes between `<newsletter>` tags and is treated as data, never as instructions.

## Testing

**Test naming:** `MethodName_Should_DoExpectation_WhenCondition` — start with the name of the method under test, not the class name.

- Do not add a test whose assertion is already fully covered by an existing test (no redundant tests).
- Never call the real Claude or Azure Speech APIs from tests.
- Never commit real newsletters as fixtures (third-party copyright, personal tracking and unsubscribe links): write minimal synthetic HTML instead.

## Code Style

Enforced via `.editorconfig`:
- 4 spaces for C#, 2 spaces for XML/props/JSON/YAML files
- CRLF endings, UTF-8
- Prefer `var` when type is clear
- Sorted `using` directives
- Central package management via `Directory.Packages.props`

## Commit & PR Guidelines

- Conventional commits: `type(scope): summary` (e.g., `feat(speech): split long scripts at paragraph ends`)
- PR titles must start with `feat` or `fix` (enforced by `.github/semantic.yml`)
- Branch names are explicit: `<type>/<issue>-<short-kebab-summary>`, one branch and one PR per issue
- Never stack PRs: branch from `main` only
- Versioning: `<Version>` in `Directory.Build.props` is bumped only by Versionize in the CD workflow. The product version is `<DotNetMajor>.x.y.z` (10.x.y.z for net10.0, tags `v10.x.y.z`): Versionize bumps only `x.y.z`, in full SemVer (`feat!`/`BREAKING CHANGE` bump `x`). On a .NET upgrade, change `DotNetMajor` (ADR-0011)
- Never commit secrets: use user secrets locally, Key Vault in Azure

## Code Quality Rules

### Cognitive Complexity
- Method cognitive complexity must not exceed 15 (SonarQube). Extract nested blocks into private methods when needed.

### Logging
- Every class logs through an injected `ILogger<T>` with `[LoggerMessage]` source-generated methods. The static `Serilog.Log` is reserved to `Program.cs`.
- Never log the content of a mail or of a script: log ids, subjects, sizes and durations.

### IDisposable — Replace-and-Dispose
- When replacing an `IDisposable` field, always call `.Dispose()` on the previous instance before assigning the new one.
