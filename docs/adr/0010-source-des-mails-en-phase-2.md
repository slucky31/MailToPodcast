# ADR-0010: Source des mails en phase 2 : API Gmail ou transfert vers un webhook

**Statut** : Proposée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Le besoin initial est de lire, toutes les 5 minutes, les mails Gmail libellés
« À écouter ». Un compte Gmail personnel impose OAuth utilisateur : application
Google à passer « En production » (sinon le refresh token expire au bout de
7 jours) et scope `gmail.modify`. Le projet email-to-podcast-rss montre une
autre approche : transférer les mails à une adresse dont le fournisseur appelle
un webhook.

## Options considérées

- **Option A — API Gmail, timer toutes les 5 minutes**
  - Pour : le geste reste « poser un libellé » ; le libellé « Envoyé en
    podcast » trace l'état dans Gmail.
  - Contre : OAuth Google à configurer et à surveiller ; scope sensible.
- **Option B — Transfert vers une adresse entrante (Postmark, SendGrid Inbound Parse) qui appelle une Function HTTP**
  - Pour : pas d'OAuth Google ; traitement immédiat, sans timer.
  - Contre : service tiers de plus ; le geste devient « transférer le mail » ;
    l'état ne vit plus dans Gmail.

## Décision

À trancher au début de la phase 2. Le port `IMailSource` permet l'une ou l'autre
sans toucher au pipeline.

## Conséquences

- **Projets impactés** : Functions, Infrastructure

## Liens

- ADR liées : [0006](0006-pas-de-base-de-donnees.md)
- https://github.com/tcanfarotta/email-to-podcast-rss (MIT)
