# Changelog

## 8.0.0 - Jellyfin 12.2

**Breaking:** requires Jellyfin 12.2 or newer (.NET 10). Stay on 6.x for Jellyfin 10.11.

- Target Jellyfin 12.2 (`targetAbi` 12.2.0.0) and .NET 10.
- `ForceSync` now re-evaluates items that already have themes, so changing volume or fetch type takes effect.
- Failed or cancelled downloads no longer leave corrupt files that were treated as valid; ffmpeg output goes to a temp file and is moved into place on success, and ffmpeg is killed on cancellation.
- ForceSync cleanup only deletes files created by this plugin (`<name>__<volume>.mp3/.webm`).
- One failing item or API chunk no longer aborts the whole task.
- Every library item sharing an AniDB ID is processed, using themes from all matching anime.
- The AnimeThemes API is paginated; empty lookups skip the request; responses are disposed.
- Invalid library regexes, parallelism of 0 and out-of-range volumes are handled instead of crashing the task.
- Downloads use a dedicated HTTP client with a long timeout, retries and a User-Agent.
- Polly retries dispose failed responses and honor `Retry-After` dates.
- Config page: no crash on a missing movie configuration, regex validation, dark-theme friendly tabs.
- Release workflow: pinned build workflow, explicit target framework, tag validation, manifest version taken from the built artifact.
- Added unit tests for the API client.

## 7.0.2 / 7.0.1 / 7

Fork releases. **Do not use:** the 7.0.2 package was built for .NET 10 but advertised ABI 10.11.3 and its manifest version did not match the plugin version. Fixed in 8.0.0.

- Remove `System.Linq.Async` and process chunks with `await foreach`.
- Publish releases and manifest from this repository's GitHub Actions.

## 6.0.0 and earlier

Upstream releases by EusthEnoptEron: batched AnimeThemes requests, Polly retry handling, movie support, library include/exclude filters, ForceSync, audio and video downloads.
