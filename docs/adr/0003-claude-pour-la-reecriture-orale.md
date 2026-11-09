# ADR-0003: Claude pour détecter la langue, traduire et réécrire en un seul appel

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Une newsletter peut être en anglais ou en français, et son texte est écrit pour
être lu à l'écran (listes, liens, sigles, nombres). Il faut détecter la langue,
traduire en français si besoin et réécrire pour l'oreille.

## Options considérées

- **Option A — Trois appels séparés (détection, traduction, réécriture)**
  - Pour : chaque étape isolée.
  - Contre : trois fois la latence et le texte envoyé en entrée ; la
    traduction puis la réécriture perdent en naturel.
- **Option B — Un seul appel Claude avec sortie JSON structurée (retenue)**
  - Pour : un aller-retour ; la réponse `{ title, source_language,
    was_translated, script }` est garantie conforme au schéma.
  - Contre : un texte long produit une longue réponse, d'où le streaming.

## Décision

On retient **Option B**, avec :

- modèle `claude-opus-5-5` et effort `low` par défaut, configurables
  (`Claude:Model`, `Claude:Effort`) ; `claude-sonnet-5-5` est l'alternative
  moins chère ;
- streaming et `MaxTokens` à 64 000, pour ne pas couper les longues newsletters ;
- repli côté serveur en cas de refus (`fallbacks: default`) ;
- le contenu du mail entre balises `<newsletter>`, traité comme une donnée et
  jamais comme des instructions.

## Conséquences

- **Positives** : coût Claude d'environ 0,05 $ (Sonnet 5.5) à 0,14 $ (Opus 5.5)
  pour une newsletter de 15 000 caractères.
- **Négatives** : dépendance à un service externe payant ; une réponse
  interrompue ou invalide lève `ClaudeResponseException` et le mail passe en
  erreur.
- **Projets impactés** : Infrastructure

## Liens

- ADR liées : [0004](0004-lecture-quasi-integrale.md)
- `src/MailToPodcast.Infrastructure/Llm/`
