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
- [ ] P04-T06 settings
- [ ] P04-T07 reduced motion
- [~] P04-T08 DE/EN localization

## P05 Production visuals
- [x] P05-T01 runtime palette/material foundation
- [ ] P05-T02 final character + animation
- [ ] P05-T03 authored platform kit
- [ ] P05-T04 skyline/cloud kit
- [~] P05-T05 landing VFX
- [ ] P05-T06 final lighting/post-processing
- [ ] P05-T07 screenshot-quality gate

## P06 Audio/haptics
- [ ] P06-T01 jump
- [ ] P06-T02 landing grade cues
- [ ] P06-T03 recovery cue
- [ ] P06-T04 haptics
- [ ] P06-T05 music/ambience
- [ ] P06-T06 settings

## P07 Persistence/progression
- [ ] P07-T01 versioned profile
- [x] P07-T02 best/coins persistence
- [ ] P07-T03 player level
- [ ] P07-T04 cosmetics
- [ ] P07-T05 migration tests

## P08 Retention/monetization
- [ ] P08-T01 daily reward
- [ ] P08-T02 daily challenge
- [ ] P08-T03 achievements
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
