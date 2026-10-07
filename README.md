# Perfect Drop

Native iOS/Android **tap-to-stack game** by Kamilunavo.

A block moves horizontally, alternating X/Z each placement. Tap to drop it onto the tower. Overhang falls away; a perfect alignment keeps the full footprint and awards a streak bonus. No overlap ends the run. Stack **30 blocks**, excluding the pedestal, to complete the tower. Retry starts immediately.

The scene's existing `GameBootstrap` now boots `StackGame`; the former jump-game code is inactive and retained during the verified transition. There is no runner, joystick, jump or boost in active play. The user's 2026-10-07 correction supersedes the earlier jump-game brief.

- Unity 6000.6.4f1; C#; built-in rendering; uGUI.
- iOS/Android bundle ID: `com.kamilunavo.perfectdrop`.
- Apple team: `TKG684N5GL`; existing App Store Connect app: `6819872064`.
- Main scene: `Assets/Scenes/Main.unity`.
- Input: Drop button or tap free world space; drag free space to orbit; desktop Space drops, right-mouse drag orbits.
- `StackValidation.Validate` covers overlap/cut/perfect/miss/completion/save and deck normals.
- Development players accept `-qaScreenshot <path>` and `-qaSmoke <directory>`; release players exclude QA components.
- Stack progression is in `perfectdrop.stack.profile.v1`; historical jump saves are preserved.

Read `docs/MASTER-PLAN.md`, `docs/ART-DIRECTION.md`, `docs/CONCEPT-SPEC.md`, `docs/PERFECT-DROP-V1-LEDGER.md`. A build is not a TestFlight RC until all required gates have fresh evidence.
