# Architecture Decision Records

Ce dossier contient les Architecture Decision Records (ADR) de MailToPodcast :
un enregistrement court de chaque décision technique structurante, pourquoi elle
a été prise, et ce qu'elle implique. Le format est celui de MyComicsManager.

## Statuts

- **Proposée** — en discussion, pas encore actée
- **Acceptée** — décision prise, en vigueur
- **Rejetée** — envisagée puis écartée (à garder : évite de la reproposer sans contexte)
- **Dépréciée** — n'est plus d'actualité mais n'a pas de remplaçante formelle
- **Remplacée par ADR-XXXX** — une décision plus récente l'a supplantée

## Créer une nouvelle ADR

1. Copier `template.md` vers `NNNN-titre-en-kebab-case.md` (numéro suivant, 4 chiffres)
2. Remplir le template
3. Ajouter une ligne dans l'index ci-dessous
4. Committer (une ADR peut être son propre commit `docs(adr): ...`)

Ne jamais modifier une ADR **Acceptée** rétroactivement pour refléter un
changement d'avis : en écrire une nouvelle qui la remplace, et mettre à jour le
statut de l'ancienne.

Les ADR sont réservées aux décisions structurantes (choix d'un service, d'une
techno, d'une convention, d'un découpage). Un compromis local se note dans un
commentaire à côté du code.

## Index

| ADR | Titre | Statut | Date |
| --- | ----- | ------ | ---- |
| [0001](0001-livraison-en-deux-phases.md) | Livraison en deux phases : CLI `.eml` puis Azure Function | Acceptée | 2026-11-09 |
| [0002](0002-ports-dans-core-et-facades-sdk.md) | Ports dans Core, façades minces sur les SDK | Acceptée | 2026-11-09 |
| [0003](0003-claude-pour-la-reecriture-orale.md) | Claude pour détecter la langue, traduire et réécrire en un seul appel | Acceptée | 2026-11-09 |
| [0004](0004-lecture-quasi-integrale.md) | Lecture quasi intégrale plutôt que résumé | Acceptée | 2026-11-09 |
| [0005](0005-azure-speech-decoupage-et-concatenation.md) | Azure Speech, découpage en blocs et concaténation des MP3 | Acceptée | 2026-11-09 |
| [0006](0006-pas-de-base-de-donnees.md) | Pas de base de données : l'état vit dans Gmail et le stockage | Acceptée | 2026-11-09 |
| [0007](0007-regles-du-depot-reprises-de-mycomicsmanager.md) | Règles du dépôt et stack de tests reprises de MyComicsManager | Acceptée | 2026-11-09 |
| [0008](0008-flex-consumption-et-bicep.md) | Phase 2 : plan Flex Consumption et infrastructure en Bicep | Acceptée | 2026-11-09 |
| [0009](0009-flux-rss-prive-par-cle.md) | Flux RSS privé servi par une Function HTTP avec clé | Proposée | 2026-11-09 |
| [0010](0010-source-des-mails-en-phase-2.md) | Source des mails en phase 2 : API Gmail ou transfert vers un webhook | Proposée | 2026-11-09 |
| [0011](0011-version-majeure-alignee-sur-dotnet.md) | Version majeure alignée sur .NET, puis SemVer avec Versionize | Acceptée | 2026-11-09 |
