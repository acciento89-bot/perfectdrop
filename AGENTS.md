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
