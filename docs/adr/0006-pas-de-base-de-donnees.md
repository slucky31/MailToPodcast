# ADR-0006: Pas de base de données : l'état vit dans Gmail et le stockage

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Il faut savoir quels mails ont déjà été traités et lister les épisodes du flux.
La stack habituelle serait SQL Server avec Dapper.

## Options considérées

- **Option A — SQL Server + Dapper**
  - Pour : historique interrogeable.
  - Contre : une base à payer et à maintenir pour un usage personnel de
    quelques mails par jour.
- **Option B — État dans les libellés Gmail et le stockage (retenue)**
  - Pour : le libellé « Envoyé en podcast » dit qu'un mail est traité ; le
    `feed.xml` dans Blob liste les épisodes ; l'id d'épisode est l'id du mail,
    donc un nouveau passage écrase au lieu de dupliquer.
  - Contre : pas de requêtes sur l'historique.

## Décision

On retient **Option B**. Si un historique interrogeable devient utile, un
`IEpisodeRepository` Dapper + SQL Server se branchera derrière un port de Core.

## Conséquences

- **Positives** : aucun coût de base de données ; idempotence par construction.
- **Négatives** : statistiques et recherche d'épisodes non disponibles.
- **Projets impactés** : Core, Infrastructure

## Liens

- ADR liées : [0002](0002-ports-dans-core-et-facades-sdk.md)
