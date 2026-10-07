# Perfect Drop Unity Project Context

Inspected 2026-10-07, baseline commit `78dfc9b0805b29a89a2dc50c07a8715b70c4bc49`.

## Confirmed environment

- Root: `/Users/piotrkaminski/Developer/PerfectDrop`.
- Unity `6000.6.4f1` (`12bfff696524`), native iOS/Android, bundle ID `com.kamilunavo.perfectdrop`.
- Built-in rendering: no SRP package in `Packages/manifest.json`; custom surface, sky and grade shaders in `Assets/Resources`.
- Legacy Input Manager (`activeInputHandler: 0`), uGUI `2.6.0`, Test Framework `1.8.0`.
- Enabled startup scene: `Assets/Scenes/Main.unity` in `EditorBuildSettings.asset`.
- No first-party assembly definition files found. Runtime C# is under `Assets/Scripts`; editor utilities under `Assets/Editor`.
- No Unity MCP tools are exposed in this session. Native editor control is currently denied.

## Architecture and ownership

`GameBootstrap.Start` builds the world, player, camera and Canvas and wires component references. `PrecisionCourse` owns the deterministic 30-floor route, landing registration, run completion and checkpoint recovery. `PlayerMotor` uses a CharacterController and camera-relative movement; `OrbitCamera` handles player swipe orbit and desktop right-mouse input. `WorldArt` supplies procedural meshes/materials, skyline/clouds and runner parts; animation/style and landing effects are separate components.

`SafeAreaFitter`, `ResponsiveHud` and `ReservedRegionProvider` handle layout. `Assets/Plugins/iOS/PerfectDropReservedRegions.mm` bridges UIKit active reserved/division regions and native haptics. Preserve these systems and course progress through layout changes.

`PlayerProfileStore` owns versioned JSON progression and migrations. `GamePreferences` owns persistent feedback/accessibility preferences. `FeedbackSystem` supplies procedural audio and invokes haptics. Settings/profile/completion panels are contextual.

## Conventions and constraints

Read `README.md`, `docs/MASTER-PLAN.md`, `docs/ART-DIRECTION.md` and `docs/PERFECT-DROP-V1-LEDGER.md` before implementing. Namespaces use `Kamilunavo.PerfectDrop.*`, types use PascalCase and private fields use `_camelCase`. Preserve Unity meta files. Main is canonical. No legacy runtime/platform artifacts; no parallel rewrite of existing gameplay or Duo support.

The primary visual reference is the fourth user attachment. Procedural character construction remains a production-art concern, not a verified final asset. Target 60 FPS, >=48-point touch controls, DE/EN, immediate recovery, optional boost and player-controlled camera.

## Validation entry points

- `Kamilunavo.PerfectDrop.Editor.GameplayValidation.Validate`: scoring, conservative ballistic reachability, progression/completion/restart and preference/profile restoration.
- `Kamilunavo.PerfectDrop.Editor.DuoReadinessValidation.Validate`: layout classification and vertical/horizontal division orientation. This is a helper matrix, not a rendered overlap or device test.
- `Kamilunavo.PerfectDrop.Editor.ProfileValidation.Validate`: profile migration and retention/progression matrix.
- `BuildAutomation`: Mac preview, Android development APK/release AAB, iOS device export and iOS simulator export.
- `scripts/testflight-upload.sh`: archive/export upload after RC QA; verify its existing destructive archive-path removal before invoking. Use a fresh archive destination and preserve prior archives.

## Baseline preservation and current limitations

Git tracked work was clean and main matched the locally recorded origin/main. Untracked `Assets/Scripts/QA.meta` and `Assets/Scripts/QA/VisualQaCapture.cs` plus meta were copied with complete Git history to the Codex task's `work/perfectdrop-backup-20261007`. The Git bundle verified successfully. No pull, reset or clean was performed.

New validation in this session is BLOCKED before compilation: graphics batch launch stopped at restricted OS services; headless launch aborted with `Failed to initialise UDS client in the Editor`. Simulator CLI could not connect to CoreSimulatorService and could not write its log. These failures do not prove a project compile/gameplay defect. Historical ledger checks have not been rerun here. No device, runtime visual, signing, upload, TestFlight or Android result is newly verified.

## Source files inspected

`AGENTS.md`, `README.md`, master plan/art direction/V1 ledger, project version/packages/build scene settings, `GameBootstrap.cs`, `PlayerMotor.cs`, `OrbitCamera.cs`, `WorldArt.cs`, all validation entry points, `BuildAutomation.cs`, native reserved-region bridge, untracked screenshot capture and TestFlight scripts.
