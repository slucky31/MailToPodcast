# MailToPodcast

Transforme les newsletters reçues par mail en épisodes de podcast en français : nettoyage du HTML, réécriture pour l'oral par Claude (détection de la langue, traduction, lecture quasi intégrale), voix neuronale Azure Speech, MP3.

## Phase 1 : tester la transformation en local

### Prérequis

- SDK .NET 10
- Une clé API Claude ([console Anthropic](https://platform.claude.com/))
- Une ressource Azure AI Speech (le palier gratuit F0 couvre 500 000 caractères par mois)

### Secrets

Ils restent hors du dépôt, dans les user secrets du CLI :

```bash
cd src/MailToPodcast.Cli
dotnet user-secrets set "Claude:ApiKey" "sk-ant-..."
dotnet user-secrets set "Speech:Key" "<clé de la ressource Speech>"
dotnet user-secrets set "Speech:Region" "francecentral"
```

### Utilisation

Exporte un mail en `.eml` depuis Gmail (menu ⋮ du message → « Télécharger le message »), puis :

```bash
# Un mail
dotnet run --project src/MailToPodcast.Cli -- --input ~/Downloads/newsletter.eml

# Tous les .eml d'un dossier, avec une autre voix
dotnet run --project src/MailToPodcast.Cli -- --input ~/Downloads/mails --voice fr-FR-HenriNeural

# Vérifier le nettoyage du HTML seulement (aucun appel d'API, rien de facturé)
dotnet run --project src/MailToPodcast.Cli -- --input ~/Downloads/mails --text-only true
```

Les fichiers sont écrits dans `episodes/` (option `--output`) :

- `{nom}.mp3` : l'épisode ;
- `{nom}.txt` : le texte lu, pour relire la réécriture ;
- `{nom}.clean.txt` : le texte nettoyé, en mode `--text-only`.

| Option | Clé de configuration | Défaut |
| --- | --- | --- |
| `--input` | `Input:Path` | (obligatoire) |
| `--output` | `Output:Directory` | `episodes` |
| `--voice` | `Speech:VoiceName` | `fr-FR-DeniseNeural` |
| `--model` | `Claude:Model` | `claude-opus-5-5` |
| `--max` | `Batch:MaxMailsPerRun` | `10` |
| `--text-only` | `TextOnly` | `false` |

Voix à essayer : `fr-FR-DeniseNeural`, `fr-FR-HenriNeural`, `fr-FR-VivienneMultilingualNeural`, `fr-FR-RemyMultilingualNeural`.

## Phase 2 (à venir)

Azure Function (plan Flex Consumption, timer toutes les 5 minutes) qui lit les mails Gmail libellés « À écouter », les passe en « Envoyé en podcast », dépose le MP3 dans Blob Storage et met à jour un flux RSS privé. Secrets dans Key Vault, infrastructure en Bicep.
