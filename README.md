# AnimeThemes for Jellyfin

Plugin for [Jellyfin](https://jellyfin.org/) that fetches anime themes (OPs and EDs)
from [animethemes.moe](https://animethemes.moe). Both audio files and video files are supported.

**An AniDB ID is used to find matching themes, so you'll need to install the AniDB plugin. Series and movies without an AniDB ID are ignored.**

## Why this fork exists

This is a maintained fork of [EusthEnoptEron/jellyfin-plugin-animethemes](https://github.com/EusthEnoptEron/jellyfin-plugin-animethemes).
It exists to:

- **Track current Jellyfin releases.** This fork targets Jellyfin 12.2 (.NET 10).
- **Run its own release channel.** Releases and the plugin manifest are published from this repository, so installing and updating don't depend on upstream's release schedule.
- **Fix reliability problems** in the download pipeline (see the [changelog](CHANGELOG.md)), such as ForceSync not re-checking existing items, corrupt files left behind by failed downloads, and one failing item aborting the whole scheduled task.

Credit for the original plugin goes to EusthEnoptEron and its contributors. The code is still MIT licensed.

## How to use

The plugin creates a new scheduled task that you can configure. By default, it will run every 12 hours.
Also make sure to enable theme music / videos in your display settings.

Requires Jellyfin 12.2 or newer.

## How to install

### Plugin Manager

1. Add the following repository to your plugin repositories:
    https://raw.githubusercontent.com/JohnnyTaxs/jellyfin-plugin-animethemes/main/manifest.json
2. Find the plugin in the catalog and install it.

### Manually

1. [Grab the latest build.](https://github.com/JohnnyTaxs/jellyfin-plugin-animethemes/actions/workflows/build.yaml)
2. Download it into your plugins folder [as described here](https://jellyfin.org/docs/general/server/plugins/).

## Configuration

| Configuration                           | Description                                                                                               |
|-----------------------------------------|-----------------------------------------------------------------------------------------------------------|
| Degree of parallelism                   | How many items should be downloaded in parallel. Default is set to 1 to go easy on the servers.           |
| Enforce conformity                      | Check every item on each run and add/remove themes so they match the configuration (e.g. after changing volume or fetch type). Only files created by this plugin (`<name>__<volume>.mp3/.webm`) are ever deleted. |
| Included / excluded libraries           | Case-insensitive regular expressions matched against library names. Excluded wins over included.          |
| Series / Movies tabs                    | Audio and video settings below can be configured separately for series and movies.                        |
| Ignore themes that overlap with episode | Some shows transition into their themes. Use this setting to filter those themes out.                     |
| Ignore themes with credits              | Whether to skip themes with credits in them. Useful for video downloads.                                  |
| Ignore OP themes                        | Use this to control whether OPs are considered.                                                           |
| Ignore ED themes                        | Use this to control whether EDs are considered.                                                           |
| Fetch type                              | Determines if you want all distinct themes, only the best match, or none at all.                          |
| Volume                                  | The desired volume, which will be baked in. For videos, 0 will result in the audio channel being removed. |

## Other notes

If you want the videos to cover the whole screen, try the following CSS:

```css
body:not(.hide-scroll) > .videoPlayerContainer > video {
    object-fit: cover;
}
```
