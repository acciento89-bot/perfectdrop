# AGENTS.md

## Product
Perfect Drop is a deliberately small but production-quality Roblox game.

## Execution rules
- Read README -> MASTER-PLAN -> ART-DIRECTION -> V1 LEDGER before implementation.
- Work only the next open ledger task plus required dependencies.
- Use strict Luau.
- Prefer server authority for score, currency, progression, entitlement and purchase grants.
- Add deterministic pure-Luau tests for rules/state transitions where practical.
- Run formatting, lint, Rojo build and test suite before marking a task complete.
- Commit/push after coherent verified tasks.
- Mark `[x]` only after acceptance criteria are actually verified.
- Runtime/device-dependent items stay `[~]` or `[!]` until verified.

## UX rules
- Avatar remains visible in core play.
- Restart is one obvious action and should return to gameplay rapidly.
- No raw IDs, developer terminology, placeholder text or default-looking production UI.
- Touch targets and HUD must remain readable on a compact phone.
- Controller selection/focus must be deliberate.

## Artifact hygiene
- NEVER write QA screenshots, videos, builds or temporary folders to the user's Desktop.
- Temporary QA: `/tmp/perfectdrop-qa`.
- Delete temporary QA after use.
- Persist only curated release evidence under `docs/evidence/`.
