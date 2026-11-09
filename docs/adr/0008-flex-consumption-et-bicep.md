# ADR-0008: Phase 2 : plan Flex Consumption et infrastructure en Bicep

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

La phase 2 exécute le pipeline dans une Azure Function .NET 10 isolated,
déclenchée toutes les 5 minutes. Un mail peut demander 1 à 3 minutes entre
Claude et la synthèse vocale.

## Options considérées

- **Option A — Plan Consumption classique**
  - Contre : modèle en fin de vie pour les nouvelles applications Linux ;
    durée d'exécution limitée.
- **Option B — Plan Flex Consumption (retenu)**
  - Pour : facturation à l'usage, identité managée, intégration réseau, durée
    d'exécution suffisante pour quelques mails par passage.
- **Option C — Plan Premium ou App Service**
  - Contre : coût fixe disproportionné pour un usage personnel.

Pour les ressources : création à la main dans le portail, ou description en
Bicep dans le dépôt.

## Décision

On retient **Flex Consumption** et une infrastructure décrite en **Bicep** dans
`infra/` (Function App, stockage, Key Vault, ressource Speech, Application
Insights, identités et rôles).

## Conséquences

- **Positives** : environnement reproductible et versionné ; Speech et Blob
  accessibles par identité managée, sans clé.
- **Négatives** : fichiers Bicep à maintenir.
- **Projets impactés** : Functions (à créer), `infra/`

## Liens

- ADR liées : [0001](0001-livraison-en-deux-phases.md)
