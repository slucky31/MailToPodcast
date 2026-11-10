# ADR-0009: Flux RSS privé servi par une Function HTTP avec clé

**Statut** : Proposée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Le flux `feed.xml` et les MP3 doivent être lisibles par une application de
podcast, mais pas publics. Les applications de podcast ne savent pas
s'authentifier autrement qu'avec une URL.

## Options considérées

- **Option A — Conteneur Blob public avec un chemin difficile à deviner**
  - Contre : impossible à révoquer sans tout renommer.
- **Option B — SAS de service longue durée dans toutes les URL**
  - Contre : nécessite la clé du compte de stockage.
- **Option C — Function HTTP `GET /api/feed/{clé}` (proposée)**
  - Pour : conteneur privé ; la clé est comparée à celle du Key Vault ; le flux
    renvoyé contient des URL de MP3 signées par SAS de délégation utilisateur
    (7 jours, via identité managée), régénérées à chaque lecture du flux ;
    changer la clé révoque l'accès.
  - Contre : une Function de plus à exposer.

## Décision

**Option C** proposée, à confirmer au début de la phase 2.

## Conséquences

- **Positives** : aucune clé de compte de stockage ; révocation simple.
- **Négatives** : un épisode téléchargé plus de 7 jours après la dernière
  lecture du flux nécessite de rafraîchir le flux (comportement normal des
  applications de podcast).
- **Projets impactés** : Functions, Infrastructure

## Liens

- ADR liées : [0006](0006-pas-de-base-de-donnees.md), [0008](0008-flex-consumption-et-bicep.md)
