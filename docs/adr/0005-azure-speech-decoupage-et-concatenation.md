# ADR-0005: Azure Speech, découpage en blocs et concaténation des MP3

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

La synthèse se fait avec une voix neuronale fr-FR d'Azure AI Speech. Une requête
de synthèse temps réel est limitée à environ 10 minutes d'audio, alors qu'une
lecture quasi intégrale ([ADR-0004](0004-lecture-quasi-integrale.md)) peut durer
davantage.

## Options considérées

- **Option A — API de synthèse par lots (batch synthesis)**
  - Pour : textes longs pris en charge par Azure.
  - Contre : asynchrone (soumission puis interrogation), plus de code et de délai.
- **Option B — Synthèse temps réel par blocs, puis concaténation (retenue)**
  - Pour : simple ; les trames MP3 d'un même format se concatènent telles quelles.
  - Contre : une requête par bloc.

## Décision

On retient **Option B** : le script est découpé en blocs d'au plus 3 000
caractères aux fins de paragraphe (puis de phrase si un paragraphe est trop
long), chaque bloc devient un document SSML avec une pause entre paragraphes, et
les MP3 (`Audio24Khz96KBitRateMonoMp3`) sont mis bout à bout. La voix est
réglable (`Speech:VoiceName`, option `--voice`) pour comparer Denise, Henri et
les voix multilingues.

## Conséquences

- **Positives** : pas de limite pratique de longueur ; découpage et SSML testés unitairement.
- **Négatives** : clé de ressource en phase 1 ; à remplacer par une identité
  managée en phase 2.
- **Projets impactés** : Infrastructure

## Liens

- `src/MailToPodcast.Infrastructure/Speech/`
