# Mobile regression revalidation — 2026-10-03

## Scope

Corrective pass based on real phone play: the active block/tower could leave the useful camera composition and the compact HUD consumed too much gameplay space.

## Implemented corrections

- Camera framing now tracks both the avatar and active MovingBlock/top of tower.
- Camera distance is derived from the actual vertical and horizontal gameplay span instead of stage count alone.
- Compact-phone HUD now prioritises Stage, Score and Perfect status; secondary Level/Coin data is removed from the gameplay strip.
- Tutorial, Shop, Drop and result layouts were resized/repositioned for compact touch viewports; the final compact Shop overlay is limited to 76% viewport width × 72% height and remains scrollable.
- The first E2E rerun correctly failed the Shop touch-target gate; the compact Shop button was then raised to 92x44 and the entire E2E was rerun.

## Verification

- StyLua, Selene and Rojo build: pass, 0 warnings/errors.
- Pure Luau: 10 test files passed.
- Release-readiness static rules: pass.
- Studio iPhone XR / touch viewport: 801x392.
- Second full E2E: `[Perfect Drop E2E] COMPLETE full player journey passed`.
- 30-stage tower completed with 24 Perfect / 6 Good placements, score 715.
- Stage-30 camera readability gate passed.
- 30/30 HUD, NEXT TOWER cleanup, deliberate miss, retry, touch-target sizing, Daily reward, cosmetic ownership/equip, duplicate Developer Product receipt handling, Revive, respawn/follow camera and progression persistence all passed.

No external release gate is reclassified by this corrective pass.
