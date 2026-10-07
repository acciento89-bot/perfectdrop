# Perfect Drop Stack Implementation Plan

> Execute inline with superpowers:executing-plans; the user explicitly requested autonomous implementation.

**Goal:** Play classic tap-to-stack Perfect Drop through 30 placements.
**Architecture:** Pure overlap rules and run/profile state feed one StackGame MonoBehaviour; StackHud handles contextual UI and region-aware reflow. Existing GameBootstrap scene entry remains, as do shared rendering/audio/preferences/native iOS bridges. Prior jump code is inactive and preserved until the replacement is validated.
**Tech Stack:** Unity 6000.6.4f1, C#, built-in renderer, uGUI, UIKit bridge.
**Spec:** `docs/superpowers/specs/2026-10-07-stack-game.md`.

## Constraints
- Target exactly 30 placed layers, excluding the pedestal.
- iOS/Android ID `com.kamilunavo.perfectdrop`; Apple team `TKG684N5GL`.
- Preserve local/untracked work and existing app entries; work on main.
- No jump/joystick/runner UI in the active game.
- No external dependency beyond the needed screenshot module.

## Review focus
- Positive/negative offsets and exact non-overlap edge: test overlap symmetry and cut pieces.
- Perfect tolerance on shrinking blocks: test no unintended growth or loss.
- Completion and repeated taps: test terminal state cannot place layer 31 or reward twice.
- Saved tower integrity and corrupted JSON: validate bounded dimensions/count and discard only invalid run data.
- Resize, division region, modal pause and drag-vs-tap: exercise live player scenarios and capture screenshots.

## Task 1 — deterministic stack rules/state
- [x] Create `StackRules.Evaluate(Vector2 center, Vector2 size, Vector2 previousCenter, StackAxis axis)` and `StackRun.Place(float offset)` with editor validation.
- [x] Run failing `StackValidation.Validate` before implementing overlap, perfect snaps, miss, reward and 30-layer terminal behavior.
- [x] Implement and rerun cases; confirm repeated terminal placements do not mutate state.

## Task 2 — runtime, save and mobile HUD
- [x] Add `StackGame`, `StackHud`, `StackSave`; switch the existing scene bootstrap to them.
- [x] Bind drop/tap/orbit, moving phase, cut-piece fall, grade feedback, retry, completion and contextual settings/daily claim.
- [ ] Preserve settled layers through save/reload; pause settings; reflow outside native division without rebuilding gameplay.
- [x] Replace the development smoke test with stacking scenarios, build and run the actual Mac player, inspect screenshots.

## Task 3 — native QA and coherent checkpoint
- [ ] Run rule/profile/layout validation, iOS simulator export/build, official Duo/portrait screenshots and Android release build.
- [ ] Review diff and current ledger, commit/push the verified checkpoint to main.
- [ ] Keep physical-device, final art/performance and TestFlight gates open until actually proven.

## Product expansion
User confirmed a level campaign plus unlockable endless mode after this foundation passed. Continue with a separate campaign design and plan; this foundation checkpoint is not a release candidate.
