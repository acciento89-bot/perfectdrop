# Perfect Drop V1 Ledger — canonical Stack game

The user explicitly confirmed classic tap-to-stack on 2026-10-07. This supersedes the former precision-jump brief. Historical checks below are not stacking-game acceptance.

Status: `[ ]` open; `[~]` implemented / unverified; `[x]` implemented + compiled + runtime/functionally/visually verified; `[!]` blocked or failing.

## Current stacking scope

- [x] S01 Alternate X/Z moving blocks; tap placement; overlap retained; overhang falls.
- [x] S02 Perfect snap without shrink; clear Perfect/Good/Miss and streak/coin rewards.
- [x] S03 Exactly 30 placements excluding pedestal; terminal state rejects duplicate placement.
- [x] S04 Immediate miss/result/retry; no runner, joystick, jump or boost in active game.
- [~] S05 Separate versioned stacking best/coins/tower save; historical jump profile preserved.
- [~] S06 Resume settled tower; pause settings; lifecycle save; no progress loss on resize.
- [~] S07 DE/EN stat/action/settings/result text and contextual daily claim.
- [~] S08 Reused audio/preferences/native haptic bridge and gold landing burst.
- [~] S09 Beveled metal blocks, corrected top-cap winding, visible gold rails and procedural cloud sea.
- [~] S10 Safe-area/native division-aware HUD with contextual panels.
- [x] S11 Actual Mac-player compact/wide/landscape runtime matrix and inspected screenshots (mobile gate remains S12/S13).
- [ ] S12 Official iPhone Duo open/divided/rotation states; ordinary portrait iPhone.
- [ ] S13 Fresh iOS/Android stacking builds and signing; physical devices/audio/haptics/performance.
- [ ] S14 Final art/mobile screenshot gate and 30-minute stability.
- [ ] S15 Signed archive, App Store Connect upload/processing, internal tester/install/physical TestFlight smoke.

## Current evidence

- Pre-transition Git history and local diff/untracked QA were backed up before the mechanic changed.
- Stack rule tests first failed on missing perfect alignment, then passed overlap/cut symmetry, edge miss, perfect preservation, 30-layer completion and terminal guards.
- A mesh regression test reproduced downward-facing top caps; it passed after the winding correction.
- Stack profile round-trip and malformed-data recovery pass in the editor matrix.
- A fresh Mac development player builds and renders the new stacking game; first screenshots inspected.
- Fresh Mac player passed the complete 30-drop UI callback sequence, cut-piece presence, perfect streak, terminal rejection, immediate retry/miss, settings pause, daily once-only claim, resize/progress preservation, and scene reload of saved layers/rewards.
- Functional QA uses deterministic 60 Hz simulation timing; this is not a real frame-rate/performance measurement. The overly narrow original harness timing window was corrected to match real Perfect tolerance.
- Actual screenshots inspected: 540x960, 800x600, 600x800, settings/result/miss/resume and a synthetic division. The latter does not establish official native Duo pose validation.
- Camera checks cover horizontal slab corner bounds at motion extremes and on first resize frame. A fresh reviewer identified and verified fixes for terminal-menu retry loss, portrait clipping and transient resize clipping.
- Fresh StackValidation.ValidatePlatform passes overlap/mesh/profile/motion range and historical display classifier checks.
- New release iOS Simulator Unity export succeeds; Xcode compile/installation/actual native screenshots remain pending.
- On 2026-10-07 the user additionally confirmed a level campaign plus unlockable endless mode. The 30-block tower is the validated foundation, not the complete approved game.
- Ruling: preserve old jump code/history during transition, but keep it inactive behind the existing scene bootstrap. Do not credit its historical QA toward this game.
- Ruling: use a separate `perfectdrop.stack.profile.v1` namespace so reinterpretation cannot overwrite historical jump progress.

---

# Historical pre-correction jump-game ledger (superseded)

The following record is retained as history only. Its checkboxes do not establish completion of the confirmed stacking product.

# Perfect Drop V1 Ledger

Status: `[ ]` open · `[~]` implemented but not runtime/device verified · `[x]` verified complete · `[!]` blocked

## P00 Product reset
- [x] P00-T01 Native iOS/Android direction locked
- [x] P00-T02 Portrait composition locked
- [x] P00-T03 Bundle identifiers locked
- [x] P00-T04 Legacy runtime removed from current tree

## P01 Unity mobile foundation
- [x] P01-T01 Unity/C# structure
- [x] P01-T02 automatic Main scene bootstrap
- [x] P01-T03 iOS + Android IDs
- [x] P01-T04 portrait + 60 FPS target
- [x] P01-T05 safe-area Canvas
- [x] P01-T06 first Unity editor compile
- [x] P01-T07 first iOS dev build
- [x] P01-T08 first Android dev build
- [x] P01-T09 adaptive safe-area resize handling
- [x] P01-T10 wide portrait / 4:3 layout profile
- [x] P01-T11 fold-safe landscape fallback layout
- [x] P01-T12 iPhone Duo runtime preparation
- [x] P01-T13 Xcode 27.1 + iPhone Duo Device Hub validation

## P02 Controls and camera
- [x] P02-T01 touch joystick
- [x] P02-T02 jump action
- [x] P02-T03 editor keyboard fallback
- [x] P02-T04 swipe orbit camera
- [~] P02-T05 compact-phone ergonomics
- [ ] P02-T06 physical iPhone verification
- [ ] P02-T07 physical Android verification

## P03 Precision gameplay
- [x] P03-T01 generated 30-floor vertical slice
- [x] P03-T02 marked landing bays
- [x] P03-T03 landing quality evaluation
- [x] P03-T04 streak + coin rewards
- [x] P03-T05 instant fall recovery
- [x] P03-T06 session boost
- [~] P03-T07 runtime reachability/balance
- [ ] P03-T08 final authored pattern library

## P04 UI/UX
- [x] P04-T01 four-stat HUD
- [x] P04-T02 objective card
- [x] P04-T03 course progress
- [x] P04-T04 joystick / Boosts / Jump layout
- [~] P04-T05 exact safe-area/text verification
- [~] P04-T06 settings
- [~] P04-T07 reduced motion
- [~] P04-T08 DE/EN localization

## P05 Production visuals
- [x] P05-T01 runtime palette/material foundation
- [ ] P05-T02 final character + animation
- [~] P05-T03 authored platform kit
- [~] P05-T04 skyline/cloud kit
- [~] P05-T05 landing VFX
- [~] P05-T06 final lighting/post-processing
- [ ] P05-T07 screenshot-quality gate

## P06 Audio/haptics
- [~] P06-T01 jump
- [~] P06-T02 landing grade cues
- [~] P06-T03 recovery cue
- [~] P06-T04 haptics
- [~] P06-T05 music/ambience
- [~] P06-T06 settings

## P07 Persistence/progression
- [x] P07-T01 versioned profile
- [x] P07-T02 best/coins persistence
- [~] P07-T03 player level
- [~] P07-T04 cosmetics
- [x] P07-T05 migration tests

## P08 Retention/monetization
- [~] P08-T01 daily reward
- [~] P08-T02 daily challenge
- [~] P08-T03 achievements
- [ ] P08-T04 cosmetic catalog
- [ ] P08-T05 StoreKit sandbox
- [ ] P08-T06 Google Play Billing sandbox
- [ ] P08-T07 restore/retry receipts

## P09 QA/release
- [ ] P09-T01 EditMode tests
- [ ] P09-T02 PlayMode tests
- [ ] P09-T03 iPhone safe-area matrix
- [ ] P09-T04 Android aspect matrix
- [ ] P09-T05 30-minute stability
- [ ] P09-T06 thermal/performance
- [ ] P09-T07 App Store assets
- [ ] P09-T08 Play Store assets
- [ ] P09-T09 TestFlight RC archive + upload
- [ ] P09-T10 TestFlight processing + internal tester assignment
- [ ] P09-T11 TestFlight install/smoke test on physical iPhone
- [ ] P09-T12 staged release

## Next open task
P03/P05: finish gameplay balance and replace remaining procedural/prototype presentation until the PRIMARY-CONCEPT screenshot-quality gate is met.


## Unity 6.6 bootstrap verification - 2026-10-06
- [x] Project imported and compiled successfully with Unity 6000.6.4f1.
- [x] Canonical Assets/Scenes/Main.unity generated and registered in Build Settings.
- [x] iOS and Android application identifiers are configured in PlayerSettings.


## Mobile platform build verification - 2026-10-07
- [x] Android IL2CPP development APK builds successfully with Unity 6000.6.4f1.
- [x] Android manifest verified: application ID `com.kamilunavo.perfectdrop`, versionName `1.0`, versionCode `1`.
- [x] Unity iOS Xcode export builds successfully.
- [x] Generic iOS device Debug build succeeds in Xcode 27.0 with automatic signing.
- [x] Code signature verified: identifier `com.kamilunavo.perfectdrop`, Apple Team `TKG684N5GL`.
- [ ] Store-ready 1024x1024 app icon and final release/archive validation remain release tasks.
- [x] iOS 27.1 Simulator Runtime is installed and the app launches on the official iPhone Duo simulator.
- [x] Latest Perfect Drop simulator build renders correctly on the iPhone Duo inner display with the adaptive landscape HUD.
- [x] Automated gameplay progression/restart matrix passes, including conservative no-boost reachability for all 30 floors.


### iPhone Duo readiness note
- [x] Safe area is recalculated when the OS window or display dimensions change.
- [x] HUD reflows between compact portrait, wide portrait and landscape/resizable layouts without rebuilding gameplay.
- [x] Landscape fallback reserves a center gutter for the fold/division region.
- [x] Minimum iOS deployment target is pinned to iOS 15 for the April 2027 submission baseline.
- [x] Apple team ID and automatic signing are persisted in Unity PlayerSettings.
- [x] Xcode 27.1 RC + iOS 27.1 runtime are installed; Perfect Drop builds, installs and launches on the official iPhone Duo simulator.

### Adaptive display verification - 2026-10-07
- [x] Unity editor compile is warning/error clean after the adaptive layout changes.
- [x] Automated layout matrix passed for 9:16 portrait, 3:4 wide portrait, 4:3 landscape and a wider resizable landscape window.
- [x] Android IL2CPP development build revalidated after the adaptive changes.
- [x] iOS Xcode export + signed generic-device build revalidated after the adaptive changes.
- [x] Exported iOS project confirms deployment target 15.0, Team TKG684N5GL and bundle ID `com.kamilunavo.perfectdrop`.

### Interactive desktop-player QA - 2026-10-07
- [x] Development player launched without runtime exceptions after the production-material shader fix.
- [x] Compact portrait layout rendered and remained playable.
- [x] Wide portrait layout reflowed without overlapping mandatory controls.
- [x] Resizable 4:3 landscape layout reflowed with stats left, objective right and critical controls outside the center band.
- [x] Real keyboard-controlled jump reached Floor 2 and produced `GOOD`, streak x1 and 4 coins.
- [x] Intentional fall recovered to the last safe platform; Floor/Best stayed at 2, streak reset to x0 and feedback returned to `READY`.
- [x] Two-axis landing scoring now measures both lateral and longitudinal error in world units.
- [x] Spawn/checkpoint position is aligned to the actual top surface of the platform instead of floating above it.
- [~] Procedural production pass now includes rounded HUD/controls, cyan landing bays, gold platform trim, skyline structures, milestone posts, a stylized runner and a distant goal beacon.


### Concept-source and current gameplay verification - 2026-10-07
- [x] Final primary concept is stored at `docs/concepts/PRIMARY-CONCEPT.png` and referenced by `docs/ART-DIRECTION.md`.
- [x] Only the next scoring platform exposes an active precision bay; completed/future bays do not clutter the route.
- [x] 30-floor progression, scoring, coin persistence, completion state and Run Again were exercised by the batch gameplay validation matrix.
- [x] Best-floor and coin persistence survive a run restart in the validation matrix.
- [x] Completion disables movement input and the restart path restores it.
- [x] Completion overlay participates in compact, wide, landscape and fold/division responsive layouts.
- [x] Current concept-driven presentation compiles and a fresh macOS development preview launches without runtime exceptions.
- [~] World presentation now has a procedural dusk sky, cloud layer, skyline, layered platform construction, route emission and a human runner silhouette. It remains below the final concept screenshot-quality gate and must receive authored-production assets/polish before release.

- [x] Settings/reduced-motion preference persistence is covered by the automated gameplay validation matrix; visual/device QA remains open.
- [x] Release-config iOS Simulator build succeeds on Xcode 27.1 RC and launches on the official iPhone Duo plus iPhone 17 Pro simulator.
- [~] Platform deck/core/top silhouettes now use a reusable beveled production mesh instead of visible primitive cubes; final authored material/texture pass remains open.

- [x] Authored deterministic 29-gap route pattern replaces random floor spacing and passes the automated conservative no-boost reachability gate.

- [x] Mobile control visual pass: pill-shaped BOOST, gold-ring jump control and directional joystick affordances visually verified in the Mac preview.

### Progression/retention implementation - 2026-10-07
- [x] Versioned JSON profile (`perfectdrop.profile.v1`) migrates legacy best-floor/coin data and survives cache reload.
- [x] Automated profile matrix passes: one-claim-per-UTC-day reward, daily challenge progress/claim lock, first-Perfect/streak/tower achievements, XP/level growth and unlocked style persistence.
- [~] Profile panel exposes level/XP, daily reward, daily challenge and runner style selection without adding permanent gameplay HUD clutter; visual/device QA is still open.
- [~] Three runner colorways are level-gated (Signal Gold, Neon Cyan, Rose Pulse) and update through renderer property blocks; final device visual QA remains open.

### Mobile audio/haptics verification - 2026-10-07
- [x] Jump/landing/recovery/completion audio code compiles and all automated gameplay/profile/Duo matrices remain green.
- [x] Procedural looping cloud-city ambience responds live to the audio preference.
- [x] Native iOS haptic bridge compiles against the Xcode 27.1 SDK and is included in the generated Xcode Sources phase.
- [x] Latest generic iOS device build succeeds and its code signature verifies for `com.kamilunavo.perfectdrop` / team `TKG684N5GL`.
- [x] Latest Android IL2CPP APK succeeds; manifest verified as `com.kamilunavo.perfectdrop`, version `1.0` (`1`).
- [~] Physical-device audio mix and actual tactile haptic feel remain the final acceptance gate.

### Concept HUD / cinematic presentation pass - 2026-10-07
- [x] Four top stat cards now use dedicated vector-style floor/crown/flame/coin glyphs matching the primary concept hierarchy.
- [x] BOOST and JUMP controls use dedicated vector glyphs instead of text-only placeholders.
- [x] Low-cost mobile cinematic grade adds controlled contrast, saturation, warmth and edge vignette.
- [x] Cloud layer receives subtle reduced-motion-aware drift and the distant goal beacon receives a restrained pulse/rotation treatment.
- [x] Unity 6.6 compile succeeds after the presentation pass; gameplay, profile and Duo validation matrices remain green.
- [x] Android release AAB build 2 succeeds and reports package `com.kamilunavo.perfectdrop`, version `1.0` / code `2`.
- [x] Xcode 27.1 RC generic iOS Release build 2 succeeds and signs as `com.kamilunavo.perfectdrop` / team `TKG684N5GL`.
- [~] Official iPhone Duo simulator runtime currently hits a CoreAudio/Data Migration simulator-service failure on launch; native device build and reserved-region compile/validation are unaffected. Physical/TestFlight validation remains the release gate.

### Handoff baseline and blocked revalidation — 2026-10-07

- Baseline: `78dfc9b`; tracked tree clean; existing main and local origin/main point to the same commit. A fresh `git ls-remote origin refs/heads/main` also confirms `78dfc9b0805b29a89a2dc50c07a8715b70c4bc49`; no fetch or pull was needed.
- Existing untracked QA capture script/meta files and complete Git history were backed up in the Codex task workspace; the Git bundle was verified. No pull/reset/clean or source overwrite occurred.
- Primary concept SHA-256 matches the supplied fourth image exactly. Art direction now specifies all requested visual/feedback categories and corrects the earlier helmet/visor instruction to the reference's human hoodie runner.
- [!] Fresh GameplayValidation: BLOCKED before compilation. Headless Unity aborts with `Failed to initialise UDS client in the Editor`; graphics launch also encounters restricted macOS services. No test-pass claim is made.
- [!] Fresh DuoReadinessValidation/ProfileValidation and rendered device matrix: NOT RUN. Native Unity computer control was denied and additional cache/simulator filesystem permissions were not granted.
- [!] Simulator CLI: cannot connect to CoreSimulatorService in this execution environment; this is not evidence of an app runtime failure.
- [~] Existing untracked VisualQaCapture remains IMPLEMENTED / UNVERIFIED; it has not been added to a release build or claimed as tested.
- No fresh Android build, iOS archive, upload, TestFlight availability or physical-device smoke test in this session. Perfect Drop remains short of RC; work on subsequent games has not begun.

Next execution block: restore authorized Unity/system-service access; compile and run GameplayValidation, DuoReadinessValidation and ProfileValidation; launch the current game and capture compact portrait plus official Duo layouts; fix only reproduced defects; iterate art to the concept gate before RC archive/upload.
