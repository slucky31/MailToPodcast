# ADR-0004: Lecture quasi intégrale plutôt que résumé

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Le podcast peut soit résumer la newsletter en quelques minutes, soit la lire
presque en entier en l'adaptant à l'oral. Un format dialogue à deux voix existe
aussi dans d'autres projets (gmail-to-podcast).

## Options considérées

- **Option A — Résumé condensé (5 à 8 minutes)**
  - Pour : épisodes courts, coût Speech réduit.
  - Contre : perte des exemples et du raisonnement de l'auteur.
- **Option B — Lecture quasi intégrale adaptée à l'oral (retenue)**
  - Pour : rien d'important n'est perdu ; le ton de l'auteur est conservé.
  - Contre : épisodes de 10 à 20 minutes ; coût Speech proportionnel à la longueur.
- **Option C — Dialogue à deux voix**
  - Pour : plus vivant.
  - Contre : réinterprète le contenu ; s'éloigne de la lecture fidèle voulue.

## Décision

On retient **Option B**. Le prompt garde toutes les informations, retire ce qui
n'a pas de sens à l'écoute (liens, boutons, mentions légales, désinscription)
ainsi que les encarts sponsorisés et l'autopromotion de l'auteur.

## Conséquences

- **Positives** : écoute fidèle à la lecture.
- **Négatives** : Azure Speech devient le premier poste de coût (environ 0,23 $
  pour 15 000 caractères hors palier gratuit).
- **Projets impactés** : Infrastructure (prompt)

## Liens

- ADR liées : [0003](0003-claude-pour-la-reecriture-orale.md), [0005](0005-azure-speech-decoupage-et-concatenation.md)
