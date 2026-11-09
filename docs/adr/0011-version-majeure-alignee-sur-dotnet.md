# ADR-0011: Version majeure alignée sur .NET, puis SemVer avec Versionize

**Statut** : Acceptée
**Date** : 2026-11-09
**Décideurs** : @slucky31

## Contexte

Le projet a besoin d'un numéro de version, d'un tag et d'un changelog à chaque
mise en production, sans étape manuelle. Comme MyComicsManager (10.x.y), le
numéro de version majeure doit indiquer la version de .NET ciblée : la solution
est en `net10.0`, donc les versions commencent par `10`.

## Options considérées

- **Option A — SemVer pur (1.0.0, puis majeure à chaque changement incompatible)**
  - Contre : la majeure ne dit plus rien de la plateforme ; diverge de MyComicsManager.
- **Option B — Majeure = version de .NET, mineure et correctif en SemVer, calculés par Versionize (retenue)**
  - Pour : même mécanisme que MyComicsManager (ADR-0018 de ce projet-là) ;
    `feat` → mineure, `fix` → correctif, calculés depuis les titres de PR
    (squash-merge) déjà validés par `.github/semantic.yml`.
  - Contre : un changement incompatible ne peut pas monter la majeure ; il
    passe en mineure.

## Décision

On retient **Option B** :

- source unique : `<Version>` dans `Directory.Build.props`, point de départ `10.0.0` ;
- le workflow `CD` (push sur `main`) calcule la version suivante avec
  `versionize --dry-run --exit-insignificant-commits`, build et teste, puis le
  job `release` committe `chore(release): x.y.z` (version et `CHANGELOG.md`),
  pose le tag `vx.y.z` et crée la release GitHub ;
- un commit `chore`, `docs`, `ci` ne déclenche pas de release, sans faire
  échouer le run ;
- garde-fou : le run échoue si la version calculée ne commence pas par la
  majeure de `TargetFramework` (un `feat!` ou un `BREAKING CHANGE` monterait
  la majeure). Ne pas utiliser ces marqueurs ;
- passage à une nouvelle version de .NET (ex. `net11.0`) : changer
  `TargetFramework`, puis publier à la main la nouvelle majeure avec
  `versionize --release-as 11.0.0`.

Comme dans MyComicsManager, si `main` est protégée, le push du commit de
release demande un secret `RELEASE_TOKEN` (PAT classique d'un compte admin,
`enforce_admins: false`) : `GITHUB_TOKEN` est refusé sur une branche protégée.
Sans ce secret, le workflow utilise `GITHUB_TOKEN`, suffisant tant que `main`
n'est pas protégée.

## Conséquences

- **Positives** : version, tag, changelog et release GitHub sans geste manuel ;
  la majeure dit sur quelle version de .NET tourne l'application.
- **Négatives** : la première release après le merge de la phase 1 sera
  `10.1.0` (un `feat` depuis `10.0.0`) ; `CHANGELOG.md` est committé et peut
  entrer en conflit si deux releases se croisent (non attendu en
  mono-mainteneur).
- **Projets impactés** : tous (version partagée), `.github/workflows/cd.yml`

## Liens

- `Directory.Build.props`, `.versionize`, `.github/workflows/cd.yml`, `.github/semantic.yml`
- ADR liées : [0007](0007-regles-du-depot-reprises-de-mycomicsmanager.md)
