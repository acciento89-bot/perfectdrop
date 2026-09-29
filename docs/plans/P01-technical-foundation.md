# P01 — Perfect Drop Technical Foundation

## Ownership

- **Server**: round truth, moving-block state, drop evaluation, score, rewards, persistence, receipts.
- **Client**: input intent, camera, HUD, local presentation/VFX.
- **Shared**: immutable config, pure rules, remote names and UI tokens.

## Source layout

- `src/server/` — server bootstrap and services.
- `src/client/` — client bootstrap/controllers.
- `src/shared/config/` — locked product/economy/monetization constants.
- `src/shared/game/` — pure deterministic rules.
- `src/shared/remotes/` — remote definitions.
- `tests/` — pure Luau tests.
- `scripts/` — deterministic test/release-readiness entrypoints.

## Build / verification

Canonical local commands:

```sh
stylua --check src tests scripts
selene src tests scripts
lune run scripts/run-tests
lune run scripts/release-readiness
rojo build default.project.json --output build.rbxlx
```

Only `build.rbxlx` is the canonical Roblox build artifact. Parallel numbered QA builds are prohibited.

## Remote contract

The first gameplay slice exposes:
- `DropRequest` — client signals one drop intent; server calculates outcome.
- `RetryRun` — client requests a fresh run after failure.
- `RequestState` — client asks for authoritative round snapshot.
- `RoundStateChanged` — server pushes presentation state.

All mutation remotes require server-side state validation and rate limiting before release.

## Environment policy

- Development and production place IDs are documented when created.
- DataStore names are environment-qualified.
- Purchase IDs live only in shared monetization config.
- Public exposure remains blocked until real receipt/rejoin evidence exists.
