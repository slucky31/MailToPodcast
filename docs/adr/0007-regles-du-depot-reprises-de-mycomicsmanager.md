# ADR-0007: Règles du dépôt et stack de tests reprises de MyComicsManager

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Le mainteneur travaille aussi sur MyComicsManager, dont les conventions sont
établies (ADR-0012, 0017 et 0018 de ce projet). Garder les mêmes réflexes d'un
dépôt à l'autre évite de réapprendre.

## Options considérées

- **Option A — Conventions propres à MailToPodcast**
  - Contre : deux jeux de règles à retenir sans gain identifié.
- **Option B — Reprendre les règles de MyComicsManager (retenue)**
  - Pour : mêmes outils et mêmes habitudes.

## Décision

On reprend :

- `.editorconfig` (CRLF, analyseurs `AnalysisMode=All`), Central Package
  Management via `Directory.Packages.props` ;
- xUnit v3, NSubstitute, AwesomeAssertions, Microsoft.Testing.Platform ;
  nommage `MethodName_Should_DoExpectation_WhenCondition` ;
- conventional commits, titres de PR limités à `feat`/`fix` (`.github/semantic.yml`),
  branches `<type>/<issue>-<résumé>`, pas de PR empilées ;
- logs via `ILogger<T>` et `[LoggerMessage]`, Serilog en sortie ;
- Renovate, labeler, CI GitHub Actions (format, build, tests).

Écart assumé : pas de SonarCloud pour l'instant (à ajouter avec un projet
Sonar et le secret `SONAR_TOKEN`).

## Conséquences

- **Positives** : conventions déjà connues ; `CLAUDE.md` les rend explicites
  pour les agents.
- **Négatives** : les surcharges d'analyseurs pour les tests doivent cibler
  `tests/**/*.cs` (le motif `*.Tests/**` de MyComicsManager ne correspond pas à
  un dossier `tests/`).
- **Projets impactés** : tous

## Liens

- `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props`, `.github/`
- `CLAUDE.md`
