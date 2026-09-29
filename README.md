# Perfect Drop

A compact third-person stacking game with the avatar always visible beside/on the build area. Moving pieces are dropped with one input; overhang is cut away and Perfect drops build combo.

## Product rule

This is intentionally a **small-scope, high-quality Roblox game**. Small scope does not permit placeholder presentation, debug-looking UI, inaccessible geometry, broken mobile layouts or unverified monetization.

## Core loop

Time each drop, preserve platform area, build height and Perfect combo, fail when the next piece misses the remaining footprint.

## Non-negotiables

- The Roblox avatar remains visible during core gameplay.
- Retry from failure must be fast and obvious.
- First-time understanding target: under 10 seconds.
- Short-session loop with score, best score and readable progression.
- Server-authoritative rewards, purchases and persistent progression.
- Mobile, tablet, desktop and controller support.
- No surprise purchase prompt on spawn.
- Monetization accelerates/revives/cosmetics; it must not directly buy leaderboard placement.
- Production-quality UI, lighting, sound/VFX and environment treatment before public release.
- No QA screenshots or temporary artifacts on the user's Desktop. Use `/tmp/perfectdrop-qa`; only intentionally retained evidence belongs under `docs/evidence/`.

## Monetization direction

Revive, one-use Perfect shield, temporary coin multiplier, block/theme cosmetics and landing/drop effects. No direct score purchase.

## Canonical execution order

1. `README.md`
2. `docs/MASTER-PLAN.md`
3. `docs/ART-DIRECTION.md`
4. `docs/PERFECT-DROP-V1-LEDGER.md`
5. Detail plan for the next open phase

The ledger is the source of truth for implementation state.
