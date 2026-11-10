# ADR-0011: Préfixe .NET devant une version SemVer (10.x.y.z) avec Versionize

**Statut** : Acceptée
**Date** : 2026-11-10
**Décideurs** : @slucky31

## Contexte

Le projet a besoin d'un numéro de version, d'un tag et d'un changelog à chaque
mise en production, sans étape manuelle. Le numéro doit indiquer la version de
.NET ciblée (`net10.0` → `10`), sans renoncer au fonctionnement normal de
SemVer : un changement incompatible doit pouvoir monter la majeure.

## Options considérées

- **Option A — SemVer pur (1.0.0)**
  - Contre : la version ne dit plus rien de la plateforme.
- **Option B — Majeure = version de .NET, puis mineure et correctif (10.x.y)**
  - Pour : même schéma que MyComicsManager.
  - Contre : un changement incompatible ne peut pas monter la majeure ; il faut
    interdire `feat!` et `BREAKING CHANGE`.
- **Option C — Préfixe .NET suivi d'une version SemVer complète (10.x.y.z) (retenue)**
  - Pour : `x.y.z` suit SemVer sans exception (`feat!`/`BREAKING CHANGE` → `x`,
    `feat` → `y`, `fix` → `z`), calculé par Versionize depuis les titres de PR
    (squash-merge) validés par `.github/semantic.yml` ; le préfixe donne la
    version de .NET. Le format à quatre nombres est aussi celui de
    `AssemblyVersion`/`FileVersion`.
  - Contre : ce n'est plus une chaîne SemVer stricte (quatre nombres) ; le
    préfixe est ajouté par le build et par le modèle de tag, pas par Versionize.

## Décision

On retient **Option C** :

- `Directory.Build.props` porte `<DotNetMajor>10</DotNetMajor>` (qui donne aussi
  `TargetFramework` = `net$(DotNetMajor).0`) et `<Version>` = partie SemVer
  `x.y.z`, seule valeur lue et modifiée par Versionize. Les versions
  d'assembly, de fichier et informative valent `$(DotNetMajor).$(Version)` ;
- le workflow `CD` (push sur `main`) calcule la version suivante avec
  `versionize --dry-run --exit-insignificant-commits --tag-template "v10.{version}"`,
  build et teste, puis le job `release` lance Versionize sans commit ni tag
  (`--skip-commit --skip-tag`, il ne fait que monter `<Version>` et écrire
  `CHANGELOG.md`), remplace `x.y.z` par `10.x.y.z` dans le titre de la section
  ajoutée au changelog, committe `chore(release): 10.x.y.z`, pose le tag
  `v10.x.y.z` et crée la release GitHub `v10.x.y.z` ;
- un commit `chore`, `docs`, `ci` ne déclenche pas de release, sans faire
  échouer le run ;
- passage à une nouvelle version de .NET (ex. `net11.0`) : changer
  `DotNetMajor`. Le modèle de tag devient `v11.{version}` : aucun tag existant
  ne correspond, Versionize repart de la `<Version>` courante, publiée sous
  `v11.x.y.z`. Remettre `<Version>` à `1.0.0` dans la même PR si l'on veut
  repartir de `v11.1.0.0`.

Comme dans MyComicsManager, si `main` est protégée, le push du commit de
release demande un secret `RELEASE_TOKEN` (PAT classique d'un compte admin,
`enforce_admins: false`) : `GITHUB_TOKEN` est refusé sur une branche protégée.
Sans ce secret, le workflow utilise `GITHUB_TOKEN`, suffisant tant que `main`
n'est pas protégée.

## Conséquences

- **Positives** : version, tag, changelog et release GitHub sans geste manuel ;
  le préfixe dit sur quelle version de .NET tourne l'application et `x.y.z`
  garde le sens habituel de SemVer. Sans tag existant, Versionize publie la
  version courante : la première release après le merge de la phase 1 est
  `v10.1.0.0`, la suivante `v10.1.1.0` (`feat`), `v10.1.0.1` (`fix`) ou
  `v10.2.0.0` (changement incompatible).
- **Négatives** : le commit, le tag et le titre du changelog sont écrits par le
  workflow et non par Versionize (une montée de version de Versionize qui
  changerait le format du titre laisserait `x.y.z` dans le changelog, sans
  casser la release) ; `CHANGELOG.md` est committé et peut entrer en conflit si deux
  releases se croisent (non attendu en mono-mainteneur).
- **Projets impactés** : tous (version partagée), `.github/workflows/cd.yml`

## Liens

- `Directory.Build.props`, `.versionize`, `.github/workflows/cd.yml`, `.github/semantic.yml`
- ADR liées : [0007](0007-regles-du-depot-reprises-de-mycomicsmanager.md)
