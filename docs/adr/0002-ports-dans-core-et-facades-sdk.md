# ADR-0002: Ports dans Core, façades minces sur les SDK

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Chaque étape appelle un service externe (Gmail, Claude, Azure Speech, Blob).
Ces étapes doivent être testées avec xUnit et NSubstitute sans appeler les vrais
services, et le pipeline doit tourner aussi bien dans la console que dans une
Azure Function.

## Options considérées

- **Option A — Appeler les SDK directement depuis le pipeline**
  - Pour : moins de types.
  - Contre : pipeline intestable sans réseau ; les SDK exposent des classes
    concrètes difficiles à simuler.
- **Option B — Ports dans Core, adaptateurs dans Infrastructure, façades sur les SDK (retenue)**
  - Pour : `EpisodePipeline` et `MailBatchProcessor` sont testés avec des
    substituts ; le prompt, le parsing, le découpage SSML restent testés grâce
    à une façade d'une méthode (`IClaudeClient`, `ISpeechClient`).
  - Contre : une couche d'indirection de plus par SDK.

## Décision

On retient **Option B** :

- `MailToPodcast.Core` ne référence aucun SDK, seulement `Microsoft.Extensions.*`
  (abstractions de log, options, DI). Il définit `IMailSource`,
  `IHtmlToTextConverter`, `IScriptWriter`, `ISpeechSynthesizer`, `IEpisodeStorage`.
- `MailToPodcast.Infrastructure` implémente ces ports. Les SDK difficiles à
  simuler passent par une façade interne d'une méthode.
- Le point d'entrée (CLI, puis Function) compose le tout.

## Conséquences

- **Positives** : tout le comportement métier est testé unitairement ; passer
  de la phase 1 à la phase 2 revient à ajouter des adaptateurs.
- **Négatives** : la façade elle-même (`AnthropicClaudeClient`,
  `AzureSpeechClient`) n'est couverte que par un essai manuel.
- **Projets impactés** : Core, Infrastructure, tests

## Liens

- ADR liées : [0001](0001-livraison-en-deux-phases.md), [0007](0007-regles-du-depot-reprises-de-mycomicsmanager.md)
