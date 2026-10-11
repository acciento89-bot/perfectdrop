# AGENTS.md

## Product
Perfect Drop is a native mobile game for iOS and Android built with Unity and C#.

## Execution
- Read README -> MASTER-PLAN -> ART-DIRECTION -> V1 LEDGER before implementation.
- The ledger is the canonical implementation state.
- Mobile first: portrait, touch, safe areas, 60 FPS target.
- iOS and Android share gameplay and visual structure.
- Do not add legacy platform files, Lua/Luau, Rojo or place files.
- No placeholder/debug UI in release paths.
- Keep progression, purchases and migrations deterministic and testable.
- Commit coherent verified work to main.

## Quality
- Compact-phone readability is mandatory.
- Retry/respawn is immediate.
- Touch targets >= 48 logical points.
- Menus must not block core play.
- Accessibility includes contrast, non-color feedback and reduced motion.

## TestFlight visual release gate — 2026-10-11

The user requires actual Unity-rendered gameplay, menus, character/tower and world/design variants to resemble the approved concept before ANY new TestFlight upload. Compilation and functional tests alone do not satisfy this gate. Run `python3 scripts/verify-visual-release.py` before scripted or Xcode GUI distribution. A failed/pending/stale review blocks upload. Inspect actual rendered evidence against the approved board before recording a passed report in `docs/QA/visual-release-gate.json`; bind it to the exact Unity source fingerprint. Never mark illustrations or generated mockups as actual gameplay. All other release checks remain required.
