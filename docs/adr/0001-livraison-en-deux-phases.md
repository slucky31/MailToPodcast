# ADR-0001: Livraison en deux phases : CLI `.eml` puis Azure Function

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

La cible est une Azure Function qui lit Gmail toutes les 5 minutes, produit un
MP3 et met à jour un flux RSS. Avant d'investir dans Gmail OAuth, Blob Storage,
Key Vault et le déploiement, il faut savoir si la transformation elle-même
(nettoyage, réécriture par Claude, voix Azure) donne un podcast agréable à
écouter.

## Options considérées

- **Option A — Tout livrer d'un coup**
  - Pour : une seule mise en place.
  - Contre : on paie le coût de l'intégration Gmail/Azure avant de savoir si le
    rendu audio convient ; boucle de réglage du prompt et de la voix lente.
- **Option B — Phase 1 en console sur des fichiers `.eml`, phase 2 en Function (retenue)**
  - Pour : réglage du prompt et des voix en local, sans compte Google ni
    déploiement ; les mêmes `Core` et `Infrastructure` servent ensuite à la
    Function, rien n'est jeté.
  - Contre : deux points d'entrée à maintenir (CLI et Function).

## Décision

On retient **Option B**. `MailToPodcast.Cli` lit des `.eml` exportés de Gmail et
écrit le MP3 et le script dans un dossier local. Un mode `--text-only` vérifie
le nettoyage sans appel d'API.

## Conséquences

- **Positives** : essais rapides et peu coûteux ; les ports de la phase 2
  (`IMailSource`, `IEpisodeStorage`) existent déjà avec une implémentation locale.
- **Négatives** : la phase 2 ajoutera un projet `MailToPodcast.Functions` et
  les adaptateurs Gmail, Blob et RSS.
- **Projets impactés** : Cli, Infrastructure

## Liens

- ADR liées : [0002](0002-ports-dans-core-et-facades-sdk.md), [0008](0008-flex-consumption-et-bicep.md)
- PR #2 (phase 1)
