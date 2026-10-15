# Perfect Drop V1 Ledger — canonical Stack game

The user explicitly confirmed classic tap-to-stack on 2026-10-07. This supersedes the former precision-jump brief. Historical checks below are not stacking-game acceptance.

Status: `[ ]` open; `[~]` implemented / unverified; `[x]` implemented + compiled + runtime/functionally/visually verified; `[!]` blocked or failing.

## Approved Arcade Builder scope — 2026-10-07

User approved campaign + endless, then the deeper arcade builder direction. The former fixed 30-block tower is now the foundation. Campaign targets range from 6 to 30; endless exceeds 30.

- [x] A01 30 configured levels/three chapters, star goals, next-level unlocks and improved-record rewards; editor matrix and actual first/advanced-level UI flow verified on Mac.
- [x] A02 Perfect energy, slow time/center/save abilities, costs, unlocks, active timer pause/save/reload; pure matrix and actual ability button flows verified on Mac.
- [x] A03 Special bonus/fragile/drift/wind types; distinct moving top colors, rules/motion and visible labels; pure rule checks and actual later-level/daily/endless placements verified on Mac.
- [x] A04 Optional Perfect-or-fail risk, doubled Perfect rewards and protection; actual reward, saved miss and failure/retry verified on Mac.
- [x] A05 Earned-coin styles, purchase once/owned selection; actual shop callbacks and inspected screenshot.
- [x] A06 Owned city built from campaign star records, three selectable/unlocked districts; no legacy skyline occlusion; inspected portrait/landscape captures and geometry clear of navigation.
- [x] A07 UTC daily challenge, three-star +75 once per day; pure idempotence and actual daily clear/reward. Ordinary daily claim remains separate.
- [x] A08 Endless unlock after level 5; logical count beyond 30, capped 64 recent/rendered layers, saved count/geometry/powers; actual 66 placements/reload/retry and pure 1000-placement matrix.
- [x] A09 Contextual map/settings/style/city/results, home settings after terminal result, no ghost gameplay HUD; fresh actual UI regression pass and screenshot inspection.
- [~] A10 Native iOS/Android build/install/input/pose/device matrix for the expanded game, final mobile art/legibility/performance/accessibility.
- [~] A11 Signed archive uploaded and processed; internal TestFlight build 1.0 (5) is Im Test for Kamilunavo Intern with the existing owner tester. Physical installation/real-iPhone smoke remains open.

Evidence: `ArcadeValidation.Validate` includes campaign, stack and historical classifier checks and passes. Actual Mac development player passed full arcade sequence; a later UI-only regression passes; final city framing/opaque-menu check passes. Functional QA uses deterministic 60 Hz simulation timing and actual game UI callbacks, not OS touch automation or a native performance measurement. Advanced unlocks were seeded only in an isolated QA profile to exercise later systems; this does not claim manual playthrough of all 30 levels. Fresh code review found and verified fixes for endless start width, resumed goal text and terminal-map settings.

Storage blocker resolved: user freed space; 13 GiB was verified before fresh native builds. No physical iPhone or Android device was detected in the fresh inventory. Release gates remain open as detailed below.

### Fresh native build evidence — 2026-10-07

- Expanded game's fresh iOS Simulator export and Xcode 27.1 RC Release/arm64 build succeeded. Installed and launched on the official iPhone Duo (iOS 27.1) and iPhone 17 Pro (iOS 26.5). Native screenshots show the fresh campaign map; Duo's outer screen (`display=1`) was inspected. The default inner-screen capture was inactive/black; this is not an inner/open/divided-pose pass.
- Fresh Android IL2CPP Release AAB build 3 succeeded, package `com.kamilunavo.perfectdrop`, version `1.0`, minimum SDK 26 / target SDK 36. Final rebuild includes the new tower icon. The initial local artifact was Android Debug signed. The existing **Kamilunavo central upload identity was subsequently found and restored through the already-authorized GitHub/GCP KMS workflow**. Signing run `37683946086` succeeded; local certificate fingerprint and every non-signature payload entry were verified against the unsigned input. No new key was created and no Play upload occurred.
- Added an opaque 1024×1024 tower icon; exported iOS catalog includes `Icon-Store-1024.png`. The fresh archive no longer emits the missing App Store icon warning.
- Fresh generic-device iOS Release archive succeeded: bundle `com.kamilunavo.perfectdrop`, version `1.0` / build `3`, minimum iOS 15.0, arm64, team `TKG684N5GL`. `codesign --verify --deep --strict` succeeds. The archive itself is development signed. After the user signed into Xcode, App Store Connect distribution export succeeded; the extracted IPA signature verifies as Apple Distribution for the same team and build. No upload occurred.
- The initial **No Accounts/No profiles** export blocker was resolved by the user's Xcode login. The App Store IPA is a technical build-3 checkpoint only: the user rejected its development graphics, and subsequent art changes require new builds before any RC/upload claim.
- Bootstrap no longer resets configured release version/build numbers. Regression reproduced the reset before the fix and passes afterward; read-only code review found no material issue.
- The user enabled Computer Use permissions; Xcode UI now works. Device Hub's UI connection still times out (rather than reporting missing grants). Native touch interaction and official Duo pose changes were not exercised. These simulator launch screenshots do not replace gameplay, physical-device, haptic/audio or performance acceptance.
- Canonical source remains on main. Native build/export logs and screenshots are in the Codex task workspace `work/` and `outputs/`; generated Xcode projects/archive remain outside the repository under `/private/tmp/PerfectDrop-Arcade-*`.

## Concept graphics pass — final build 5 validation, 2026-10-08

The user rejected the prototype visuals and requested graphics matching the attached concept images. The fourth concept's modern navy/gold cloud architecture is the binding art direction; its runner/jump controls remain superseded by the confirmed stacking game. The user authorized autonomous overnight implementation. Final desktop and native current-pose art/gameplay checks now pass. Build 5 is available as an internal-only TestFlight candidate for outstanding physical-device acceptance; this does not establish RC completion. No public App Store or Play release.

- [~] Generated production portrait/landscape cloud-city backdrops and metal-panel albedo live in `Assets/Resources/Art`; real foreground stack/cuts/owned city remain 3D geometry.
- [~] Closed beveled metal decks, continuous emissive bands inside actual landing footprints, shared frame meshes, instanced color/emission, cached studio reflection and quarter-resolution bloom.
- [~] Licensed Barlow regular/bold, graded bordered navy/gold cards, square-fitted stars/locks, transparent map grouping and illustrated chapter atmospheres.
- [~] Perfect landing ring and corrected spark scales, reduced-motion behavior, detailed district bases/rooftops/facade signals.
- [x] Fresh complete desktop art pass (build5 source `b94f930aca3f4f6249b9d570889f8d7db215abe2`): shader-guarded development player built, full actual arcade callback/raycast sequence passed, and the combined editor matrices passed. Inspected actual map, gameplay, styles, city and 66-layer endless screenshots. Regression checks cover semantic cap-triangle winding, decoration footprints after narrow cuts, spark dimensions and map chapter→Continue lighting restoration. The latter was reproduced failing before its fix. Final read-only review found no material remaining code issue. The final navy energy/block-type card improves contrast against bright clouds.
- [x] Final5 real-time 30-minute desktop render/stability soak completed with `PASS`: 1800 seconds, 100% focused frames, no runtime errors, Unity allocated memory first/max 97.6/98.9 MiB. All six modes had median 16.67 ms and p95 17.05–17.41 ms. Modes cover retained 64-layer endless, fully built district, portrait/landscape, chapter map and synthetic division. The batch counter was unavailable (`-1`), so no draw-call or GPU timing claim. This is separate from deterministic functional QA and does not establish native performance. Earlier interrupted/superseded long runs were not accepted.
- [~] Desktop and native current-pose art/layout/error checks and code review passed. Real-device performance/input/audio/haptics and genuine opened/divided/rotated Duo states remain open. Generated artwork alone is not runtime validation.

### Build 5 native and signing receipts

- Final source: `b94f930aca3f4f6249b9d570889f8d7db215abe2`, committed and pushed to canonical main. Version `1.0`, iOS build `5`, Android versionCode `5`.
- Native development iOS Simulator export and Release/arm64 Xcode build succeeded. Full functional sequence passed on iPhone 17 Pro (iOS 26.5), framebuffer `1206×2622`, and official iPhone Duo (iOS 27.1), current outer framebuffer `1398×2034`. Screenshots for map, live stack, powers, city, styles, daily, 66-layer endless and failure were captured at actual native resolution, without desktop framebuffer overrides. Native disabled ASTC support produces simulator texture-decompression warnings; these are not physical-device rendering/performance results.
- Both native results explicitly cover game callbacks and actual visible-button-center raycasts. They do **not** establish OS touch delivery, real-device performance, audio/haptics, or other Duo poses. Every native run writes `RUNNING` before a fresh result into a unique directory; previous scaled diagnostic captures are superseded. See `docs/NATIVE-QA.md`.
- A fresh final5 negative probe on official Duo deliberately put a transparent raycast blocker over the menu. It correctly produced `FAIL`, identifying blocked `Chapter0`, plus a full-resolution failure screenshot. This validates the checker; it is an intentional harness failure, not a product failure.
- Signed Android AAB: central signing run `37698363633` succeeded with the existing Kamilunavo upload certificate. Local `jarsigner` verification passed, expected SHA-1/SHA-256 matched, and every non-signature payload entry matched the unsigned input. Signed SHA-256: `11c40700d2ef75877a24fb6e5b0ad5b87c1981742951b23f5eb7d1ba57620229`. This artifact is superseded: direct AAB icon inspection found the default Unity cube in its empty adaptive icon slots. Android-only source `f7b32aa505128891fc91b99fe1dfb1eb0c9db05a` corrects all adaptive slots with a padded transparent tower foreground and navy background. Fresh Android build5 succeeded; exported foreground/background inspected across all six densities. Replacement central signing run `37704454282` succeeded. Final signed SHA-256 `9bde38f3110cb88e8c81f030071ab220920dc65afe7bac71a0f36a6cd8316756`; local jarsigner reports jar verified, certificate fingerprints match the existing upload identity, and every non-signature payload entry matches unsigned input SHA-256 `e8082cd90cf5dabeb8bec971ac3dffdb9be5d335d484d93efe480589ce188c0f`. Asset `620144270` is a private draft signing input. An editor-only follow-up guards Android module imports/configuration with UNITY_ANDROID; fresh iOS and Android editor release validations passed, with no runtime/gameplay or exported-icon asset change. No Play upload.
- Generic iOS device archive and App Store Connect distribution export succeeded. Extracted IPA passed strict/deep signature verification: `Apple Distribution: Piotr Kaminski (TKG684N5GL)`, bundle `com.kamilunavo.perfectdrop`, build `5`, store profile `get-task-allow=false`. Release generated code excludes the QA launch/test types. Xcode Organizer uploaded this archive successfully using TestFlight Internal Only at 01:25 Europe/Berlin on 2026-10-08; Apple processing completed. Build 1.0 (5) is Im Test in Kamilunavo Intern (one existing owner tester), with German test notes saved. Automatic distribution is disabled. Initial CLI upload failed providerId mapping; the subsequent GUI upload and server status are the success evidence. Missing vendor UnityRuntime.framework dSYM (UUID 392D7A7F-6A4F-3A7A-8788-4089FA96FF38) is an accepted upload warning; its real symbols are absent from the installed Unity support and were not fabricated.
- IPA SHA-256: `0528e4067cf328c7f5913a2f7269cc657b092a43062096911ad64d372163ce80`.
- A regular Mac review player also builds successfully, without development-player overlays. This is a review aid, not mobile release acceptance.
- Additional official simctl screenConfig attempt: display 1 was powered off then restored on; display 3 (2007×2853) still captured entirely black and the app launch/container requests stalled. Simulator shut down after the bounded attempt. This is NOT an opened/rotated/divided pose pass. Device Hub CUA connection remains unavailable; prior genuine current-outer native PASS is unchanged.
- Evidence files are in the Codex task workspace `work/concept-build5-*`, `work/concept-native-build5-*`, `work/concept-*-build5.log`, and `outputs/`. Physical iPhone/Android tests, genuine opened/divided/rotated Duo states, final native performance and TestFlight installation remain open. Earlier build3/build4 signing receipts above are historical and superseded by build5.

## Physical feedback and next monetization scope — 2026-10-08

User tested TestFlight1.0(5) on a real iPhone16ProMax: runs well, sound/haptics good, no perceived stutter; other behavior okay except awkward landscape Drop. This is qualitative physical feedback, not measured FPS/thermal/lifecycle acceptance. No Duo available; its genuine poses remain open.

Landscape investigation confirmed build5 Drop/Risk partial overlap with actual runtime bounds-check FAIL plus failure screenshot. Fix gives landscape Drop larger right-side target and more inset, moves powers to left. Fresh full30-drop/cut/miss/retry/settings/save/resize/reload QA PASS in4:3 and956×440 layouts; inspected captures. Desktop same-aspect evidence is not a new physical-iPhone verification. Portrait unchanged.

User selected optional rewarded videos + design/starter native IAP. Old no-real-money campaign spec is superseded only for this newly requested optional shop; existing free content is preserved. Design/plan: `docs/superpowers/specs/2026-10-08-monetization.md`, `docs/superpowers/plans/2026-10-08-monetization.md`. AdMob account question pending. No new SDK or real money implementation yet at this checkpoint. Prior autonomous implementation/main push/internal TestFlight authorization persists; no public release.

## Verified stacking foundation (historical checkpoint)


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
- [~] S14 Desktop/native current-pose art screenshots and 30-minute desktop stability passed; complete native-device/pose gate remains open.
- [~] S15 Signed archive/upload/processing/internal tester complete for build 5; install/physical TestFlight smoke remains open.

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
- The fixed-30 foundation exported successfully for iOS Simulator before the campaign expansion. That superseded export was removed as an owned temporary artifact; it does not validate the expanded game. Native compile/installation/screenshots remain pending.
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
- [x] P09-T09 TestFlight signed archive + internal-only upload (build8; physical acceptance remains open)
- [x] P09-T10 TestFlight processing + internal tester assignment (build8, internal-only candidate)
- [ ] P09-T11 TestFlight install/smoke test on physical iPhone
- [ ] P09-T12 staged release

## Next open task
Complete physical TestFlight input/audio/haptics/performance and genuine Duo pose acceptance. Android adaptive-icon signing/verification is complete. Perfect Drop remains the priority; subsequent games have not entered serious implementation.


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

## Gold block / upper HUD follow-up — 2026-10-08 evening, native validation pending

User requested block quality closer to PerfectDropIcon.png: standard decks now use dark beveled metal shoulders, a bright metallic gold inset and a continuous lower gold band. Six renderers per block and the existing shared meshes/metal texture are retained. The finish gradient is bound to local UVs; special block plate cues and owned rail/body styles remain separate. Retained/cut footprints and gameplay/economy are unchanged.

Stat heading/value and objective status/hint anchor regions now have an explicit gap rather than overlap. Desktop 956x440 did not reproduce the user's physical upper-HUD defect; do not claim its full physical resolution from source spacing alone. Wide centered landscape Drop is preserved.

Desktop review1 passed the actual 30-placement/cut/miss/retry/pause/resize/save/reload matrix; inspected actual portrait and 956x440 captures. Review2 passed the full campaign/powers/special/risk/city/styles/daily/66-layer endless/reload matrix and its brighter gold tutorial capture was inspected. The subsequent small brushed-finish shader change compiled for iOS/Metal and editor arcade/campaign/stack/cut-geometry/reserved-region/release-version checks pass. Fresh native runtime validation of the final finish and actual rotation remains IMPLEMENTED / UNVERIFIED at this source checkpoint. New development-only opt-in PERFECTDROP_QA_ROTATE requests real native orientation, waits for dimensions to settle and keeps separate pose screenshots. Native QA is functional callback/raycast evidence, not physical touch, genuine commerce or a performance measurement. Evidence is in delivery workspace work/perfectdrop-gold-*.

## Optional monetization work — 2026-10-08 morning (not yet released)
- User approved voluntary rewarded videos + cosmetic/starter purchases, and explicitly chose a wide centered landscape Drop control. Earlier right-side fix is superseded by centered (.20,.07,.60,.20), powers above (.12,.29,.76,.20).
- Installed official UnityIAP5.4.4, GoogleMobileAds11.5.0, EDM1.2.187. AdMob existing account verified through Safari; created unpublished Perfect Drop iOS/Android entries and one 50-Coins rewarded unit each. Runtime configuration deliberately remains internal test ads.
- iOS app ca-app-pub-8944085355624754~8084045088, rewarded /9205555063; Android app ~1614005506, rewarded /1739971899. These are public configuration IDs, not credentials. Account payouts/approval were not changed.
- Starter nonconsumable com.kamilunavo.perfectdrop.starter (Apple6820360326) created as draft, German name/description, EUR1.99 base price and availability draft saved. No App Review submission/public sale. Collection catalog preparation in progress.
- New atomic wallet fulfillment/transaction dedup, confirmed restoration without starter-coin mint, entitlement reconciliation, premium selection, real store prices/disabled offline states, contextual scroll shop and four exclusive styles. Reward core gives50 only on genuine SDK completion, max5/UTCday,60-second cooldown and persistence rollback.
- Commerce editor validation passes migration/replay/unknown/restore/persistence failure/revocation/paid-style selection/reward cap/cooldown/UTC rollover/rollback checks. Initial failing test logs retained in task work.
- Mac shop UI run commerce-ui PASS, full gameplay commerce-full PASS across campaign/special blocks/powers/risk/city/daily/styles/beyond30/endless/geometry, with actual portrait/956x440 shop captures inspected. These first player checks precede final review fixes; final regression still required.
- Fresh read-only whole-change review found old-consent in-flight ad retention and revoked style not repainting existing blocks. Added ad-load generation invalidation and runtime repaint; editor generation checks passed, runtime repaint regression pending. Removed redundant UnityIAP restore FetchPurchases after confirming SDK already performs it.
- Native simulator export succeeded; dependency installation required official CocoaPods CDN instead of cloning Specs history, and dependency deployment targets15.0 for Xcode27.1. Durable postprocessor added at EDM order45. First Xcode build failed on dependency iOS12 target; corrected build running. No native commerce PASS or earnings claim yet.
- User explicitly requested full Unity repositories backed up and verified before temporary-cache cleanup, then Rising Steps completed while away until18:00. Original sequence and no public publishing remain.

- Native simulator dependency build2 succeeded with Google-Mobile-Ads-SDK13.11.0 / UMP3.1.0 and UnityIAPStoreKit2 framework under Xcode27.1. Final editor commerce suite passes after review corrections. Native device execution/test ads and final source rebuild remain pending.


## Native commerce integration and verified backup — 2026-10-08 morning
- Full Unity source backup at `324011d69d8c4d2e7218c341b90fce05e7f2cc3c` verified against GitHub Git tree `caf5e31b0e04ea87fa7b341b70298adfb4e734b1` (219 tracked entries under Assets/Packages/ProjectSettings). No assets or source deleted during cache cleanup.
- Reproduced native startup abort `GADInvalidInitializationException`: export from active desktop Editor omitted the conditional Google iOS plist postprocessor. Corrected export starts Unity with `-buildTarget iOS`. Build automation now rejects native target mismatch and validates AdMob app ID/SKAdNetwork declarations after successful iOS export. Missing-ID export fails the new check; corrected export and native Xcode build pass, actual iPhone17Pro simulator startup screenshot verified.
- Native dependencies resolve with official CocoaPods CDN, Google-Mobile-Ads-SDK13.11.0 and UMP3.1.0. Dependency deployment targets15.0; full CocoaPods Specs Git clone avoided. No secret or signing material added to source.
- Fresh narrow follow-up review confirms stale-consent load invalidation and current style repaint fixes; no remaining concrete findings in reviewed scope. Pure commerce regression checks passed before native export. Latest native gameplay regression pending at this checkpoint; the new QA visual assertion originally expected a moving block after a completed run and was corrected to inspect the existing hidden placed tower.
- AdMob Perfect Drop iOS/Android apps and optional50-Coin rewarded units configured; internal candidate uses official test units. Existing EU consent messages belong to Idle Handwerker/NavoTap and were left unchanged. Perfect Drop consent message and its own verified privacy URL remain required; no live-earnings claim.
- App Store Connect non-consumable drafts: Starter `6820360326` (1.99EUR base Germany, German Starterpaket/Copper+500Coins); Neon Collection `6820365745` (2.99EUR base Germany, German Neon-Kollektion/three designs). All175 territories selected; not submitted for review or published. Native sandbox purchase/cancel/restore and genuine rewarded completion remain unverified. Client fulfillment is idempotent but not server-authoritative custom receipt verification.

- Final native regression PASS on iPhone17Pro1206x2622, safe area(x0,y102,w1206,h2334): campaign/powers/special blocks/risk/city/daily/free and premium style selection/ownership persistence/revocation repaint/66-layer endless/save/reload/retry and visible-button raycasts. Source uses native SDK plugins; functional QA deliberately disables real store/ads and isolates saves. Native normal startup also verified separately with SDK initialization enabled. Evidence `work/commerce-native-final/`, `commerce-native-finalqa3-*`, `commerce-ios-native-build5.log`. Desktop resize requests skipped on native; no claim of physical touch, genuine Duo poses, native measured FPS, sandbox purchases or completed ads.
- Build6 requested for this integration; no build6 archive/upload availability claimed until verified.

- Build6 generic-device Release archive and App Store Connect distribution export succeeded. Strict/deep signature verified after removing filesystem attributes from the extracted inspection copy: Apple Distribution Piotr Kaminski/TKG684N5GL, com.kamilunavo.perfectdrop, version1.0/build6, get-task-allow=false. Original IPA SHA256 `5eaae6bfbf90d5cb547f27976c79a554729fd73c5ce7fe16cb4ce9e1d23d84a7`. Archive retained at `/private/tmp/PerfectDrop-Commerce-Build6.xcarchive`; IPA retained in task outputs. Release generated code excludes RuntimeSmoke/QaLaunch/environment bridge.
- Xcode Organizer TestFlight Internal Only upload completed at06:55 Europe/Berlin, 2026-10-08. Accepted warning: vendor UnityRuntime.framework dSYM UUID392D7A7F-6A4F-3A7A-8788-4089FA96FF38 absent as in build5; no symbols fabricated. Apple processing complete, export questionnaire completed for platform-provided encryption (native imports include CommonCrypto/Security/NSURLSession; no custom first-party encryption). Build `ee8f8f09-c168-43b9-8de2-ef76b6e84bcf`, 1.0(6), shows Im Test and Kamilunavo Intern; detail page confirms1 internal tester and saved German test notes. No public release or App Review submission. Physical build6 install/sandbox/ad completion pending.
- ASC purchase drafts now include saved German and English(USA) names/descriptions. Both remain unsubmitted.
- Android build6 IL2CPP Release AAB succeeded with actual Google ads25.4.0/UMP4.0.0 dependency templates. Generated Android Gradle templates/metas are retained as portable Unity project assets (no machine paths/secrets). Central signing and payload verification pending at this checkpoint.

- Central Android signing run `37730860780` succeeded (prepare+sign). Signed AAB SHA256 `c57225d9f2756fa51d2fe91213f5dd0429f3b4c55db4232b46d0788a78b50b5d`; unsigned `5444b910b1b850a137e009714bcbc15ee0ca7035b2d85ee8bd872c91029c740b`, private draft asset620790389. Local jarsigner verifies, existing SHA1/SHA256 upload-certificate pins match, every non-signature payload entry matches unsigned input. Bundletool1.18.2 hash verified and native manifest checked: com.kamilunavo.perfectdrop/version1.0/code6, correct Android AdMob application ID. No Play upload.
- iOS source `967fbe0710efeb3766c1729cd88e82b1a969108b`; Android source plus resolved portable Gradle templates `1c7f6996d2924d786effd59a548a7c62f963585e`. Full source GitHub tree for the latter `c7e82e77891a07272240844cc619ce1ea56f7006` matched local before deleting untracked5.7GB Unity Library cache. All source/art/settings/packages and signed final outputs/archives/native QA evidence preserved. Available disk6.7Gi after cleanup. Build6 remains an internal candidate pending genuine native purchase/ad and physical acceptance; user unavailable until18:00, so continue authorized next-game work after this verified internal delivery.

Archive storage update 2026-10-08: the completed Xcode archive is retained losslessly as /Users/piotrkaminski/Documents/Codex/2026-10-07/files-mentioned-by-the-user-image/outputs/archives/PerfectDrop-Commerce-Build6.xcarchive.tar.gz. All 60 regular files were checked byte-for-byte before removing the regenerable uncompressed copy from /private/tmp. Compressed SHA-256 a0104dba31fa8ac5ec9f12ffeb57904a03fa59e581acc0b52fb856ef7b5c8b03. Local proof manifest retained beside the archive; restore with tar -xzf to /private/tmp when needed. Unpublished draft archive asset 622081286 upload has matching GitHub server digest.

## Gold volume and landscape framing checkpoint, 2026-10-08

StackGame LayerHeight increases from .34 to .52 for the icon-like slab volume; X/Z overlap, block timing, rewards and saved footprint rules remain unchanged. The landscape camera lowers its world focus to keep the target deck above the existing power row and centered Drop button. Fresh desktop smoke passes the actual moving/drop sequence through30, cut/miss/retry, pause, resizing, daily idempotence and fresh scene restore. Portrait and landscape images inspected. Fresh iOS development export and native ARM64 simulator build both succeed; copied app entries verified byte-for-byte in the delivery receipt. Native run is in progress; this checkpoint is not the updated TestFlight binary and does not claim physical touch/performance acceptance.

Native volume/framing follow-up: source75a8ac1, actual iPhone16ProMax Metal/IL2CPP **PASS** for full30-block smoke (cut, miss, retry, save) in German and full arcade campaign/powers/special/risk/city/daily/styles/66-layer endless/reload/retry in English. Real UIKit orientation settled at1320x2868 and2868x1320; fresh native portrait and gameplay landscape visually inspected. English labels confirmed in the framebuffer. Target deck is above the power row; header headings, values and objective hint remain distinct. Local work/perfectdrop-gold-volume-native-stack-de and perfectdrop-gold-volume-native-arcade-en preserve evidence. Shipping TestFlight remains1.0(6), refreshed native release still pending. No physical touch/fold/performance or genuine store claim.

## Shared commerce recovery correction, 2026-10-09

Regression probes reproduce both missing late-consent presenter gate and missing same-session reconnect gate (RED). Minimal production correction follows the already validated OneMoreFloor/Repair adapters: consent update completion only presents in the currently open paused shop and never over a purchase; synchronous presenter failure resets UI state. Store reconnect retains one controller/event subscription set, forbids overlapping pending async connects even after failure callbacks, and allows retry after5s. Shop exposes a reconnect row with existing52-point scroll sizing. Fresh editor **11 recovery checks PASS**, existing migration/purchase/replay/restore/revocation/reward rules PASS. work/perfectdrop-commerce-recovery-{red,green}.log retains evidence. Refreshed player UI/native delivery and actual SDK purchase/reward acceptance remain pending. Product IDs, economy and save schema unchanged.

Fresh combined desktop player follow-up,2026-10-09: complete campaign/powers/special/risk/city/styles/daily/66-layer endless/retry/reload PASS; new reconnect row visible and reachable at shop bottom in956x440landscape. work/perfectdrop-gold-commerce-review. No genuine native commerce/physical performance claim.

Shipping follow-up,2026-10-09: fresh1.0(7) device Release archive and internal-only IPA export succeed. Strict/deep Apple Distribution signature/teamTKG684N5GL/get-task-allow=false verified; IPA SHA256 b832fac84f85e4a7c6fb768a69924e8d0928b64ef637ff01475272afc1f01f2f. Generated production code contains no RuntimeSmoke/QaLaunch. Xcode Organizer InternalOnly upload06:50Berlin completed with the known vendor UnityRuntime.framework dSYM UUID392D7A7F-6A4F-3A7A-8788-4089FA96FF38 warning. ASC upload table confirms1.0(7) Processing; tester availability not yet claimed. Archive compressed and all60files/symlinks verified; SHA256 b959558f23d25c0b8dc4109305edda1baf03da225e91fb7d14bb770829406f14. Preserved at outputs/archives/PerfectDrop-Gold-Commerce-Build7.xcarchive.tar.gz in delivery workspace before removing generated original/export/cache. Android7 pending.

Android7 follow-up: native compilation succeeded; initial Gradle buildReleasePreBundle hit disk exhaustion. Existing98native/source Gradle inputs preserved and only compilation cache removed; same project packaging retry **BUILD SUCCESSFUL18s/122tasks**. All570non-signature entries unchanged when stripping the local debug JAR wrapper. Bundletool/pinned jar validates com.kamilunavo.perfectdrop/version1.0/code7/own AdMob Android ID, release non-debuggable; all6ARM64 ELF PT_LOAD alignments16KB. Central signing run37887344171success. Final signed AAB SHA25674ae62cf3284ffbb2073a739698c4fe1d1dde0543d0e72379b572913e446d6ee; jarsigner and both universal certificate pins match, all570payloads and manifest identical. Durable unpublished draft407519602 final package/proof server digests verified. Full Unity source remote matched before deleting Library. Apple browser session expired again during processing; final TestFlight7tester availability still requires owner login. No Play upload/public App Review/physical or actual-commerce acceptance.


## Internal build7 availability,2026-10-09
Authenticated App Store Connect now restored. Platform-provided encryption declaration saved for build7; no custom first-party algorithms changed from build6. Existing Kamilunavo Intern group1f1e1ba3-ea2f-4827-86b3-5c74e06ddbc2 explicitly assigned; actual build7 row shows Im Test and Kamilunavo Intern. Receipt work/perfectdrop-testflight-build7-status.json. No public release/App Review submission; physical build7 installation and genuine commerce gates remain external.


## Presentation and guided stacking correction — 2026-10-09
User device screenshots establish landscape power controls obscuring moving blocks, oversized primitive menus and the poor district board. Fixed landscape side rail and wide centered drop leave an explicit perspective-fitted gameplay pane, respecting safe/reserved regions. Actual camera projection checks include moving-axis extremes and large manual-yaw changes; no anisotropic projection scale. Gold level gallery has measured rows, dimensional previews, fixed Play/utility hierarchy, scrollable compact-phone levels and reduced unused space. Styles/shop expose dimensional specimens. Built districts now have paving/roads/sidewalks, gardens, varied glass facades, setbacks and crowns, with material-batched meshes and owned disposal. Existing earned progression determines towers.
Guided first run observes real placements/Perfect/direction change; skip/replay persists completion and preserves wallet, paid rights and current run. Specific nonstandard block rules are visible. New Graphics explicitly create CanvasRenderer. Actual Unity/SDK/Metal player builds succeed, strict editor tests include782 presentation/perspective/migration and1210 measured UI targets/renderers, plus existing campaign/release/stack/power gates. Fresh actual player work/perfectdrop-presentation-mac2/result.txt passes orientations, three actual tutorial placements, replay/save integrity, motion framing, level/gallery reachability, shop/settings/style and city. Screenshots reviewed including landscape clear stack pane and dense portrait gallery. Neither actual physical iPhone touch nor real SDK purchases/rewards are claimed. Build numbers prepared8; native delivery pending.

## Presentation build8 internal delivery — 2026-10-09
- Source9cb973f5595b7a8580b69ed130a334e8caf6a30d. Actual Mac player verified orientations, three tutorial placements, replay/save integrity, city and galleries. Editor presentation782 and measured UI targets/renderers1210 passed on both native exports. Landscape world space, left power rail and broad bottom drop control are reserved separately. Physical iPhone16ProMax rotation/ergonomics remains user acceptance.
- iOS Release archive and distribution signature/team/build verification succeeded. IPA SHA25678e9e9a195b78ecf3c38331cf88244bed3a591af8fe18cc9f63cd113aeb2e547. Native QA callbacks excluded. Byte-verified archive ca09c7001163cbfecce9515952713fba8fcc779b7d7f823053f15a2ab100d462,60files, IPA, proofs and successful Organizer upload metadata retained in unpublished draft perfectdrop-build8-presentation-tutorial; server digests verified.
- App Store Connect build5b37a811-c554-4eee-b452-28133d65504d /1.0(8) observed Im Test, existing Kamilunavo Intern group,1invitation, German physical-orientation test notes saved. Known UnityRuntime vendor dSYM warning did not prevent upload. No public submission.
- Android8 compilation succeeded. Existing central upload identity verified in signing run37978526145; signed AAB SHA256e99d55c18c0b1c5267c079baa61d9feea8fb19c82b1e0a23d84ce7982298bbbd,570payload entries unchanged, manifest identical and certificate pins matched. Signed bundle/provenance/certificate/local verification backed up in same draft. No Play upload.
- Owned generated Library/native export/derived and archive duplicates cleaned after verified retained backups. Source Unity project remains in canonical main. Physical performance and genuine store/ad callbacks remain separate acceptance gates.

## 2026-10-09 selected design remains Starter Gold — source regression correction

The user reports that choosing other designs does not visibly change the live blocks. Tracing the actual UI→StackGame.SelectStyle→RefreshProfileStyle→WorldArt.StyleStackBlock path shows that selection, ownership, save and repaint calls are connected. The repaint excluded TopPlate; styles0–3 also retained the same MetalDeck body. The dominant plate and body therefore remained the Starter finish while only narrow gold-named rim renderers changed. The actual PerfectDropSurface shader consumes instanced `_Color`; `_DeckBaseColor` is only the original-body cache, and the unrelated PerfectDropSignal `_Tint` shader is not used for these block parts.

Before correction, the lightweight production-method harness executes the exact current SelectStyle/StyleStackBlock method bodies with renderer/property-block test doubles: style0 and style1 both produce body0.300/0.350/0.440 and plate1.000/0.800/0.350, then fail `STYLE_RENDER_FAIL style 1 keeps the same dominant body/plate as another design`. Baseline output retained at /private/tmp/pd-skin-production-red.log. This is source-path evidence, not GPU/runtime visual evidence; its entitlement normalization is a fixture stub for pre-owned designs.

A single shared StackStylePalette now defines distinct main body, inset plate and accent finishes for all eight styles. Real MetalDeck/DeckCrown/TopPlate/rim property blocks consume that palette, and gallery/style/shop specimens bind the same definition. Starter and golden designs retain the owned brushed-gold finish; cyan/pink/jade/ice/neon now change the large visible plate and body instead of only the thin rim. Existing shared meshes/materials, six renderers, footprint and progression remain. Style reset restores original MetalDeck base color. RefreshProfileStyle re-applies CurrentKind after moving-block restyling so bonus/pink/blue/green special cues retain priority. No prices, wallet/ownership rules, save schema, build numbers or SDK adapters were changed.

Lightweight production-path regression now passes8 distinct dominant finishes and repeated pre-owned selection without a wallet charge. Actual runtime C# and the new editor fixture compile against bundled Unity/uGUI engine references with commerce interface stubs; existing deprecation/unused-stub warnings only. Diff whitespace checks pass. No Unity/editor fixture execution, runtime screenshots, heavy build, version change, signing, commit, push or publication was performed by this source worker.

Root next acceptance: ArcadeValidation now calls StackStyleValidation, which constructs a production block, verifies actual shared shader and consumed property overrides on body/crown/plate/rims, preview parity, reset/special cue and unchanged six-renderer budget. Runtime `-qaStyles OUTPUT`, or PERFECTDROP_QA_MODE=styles with PERFECTDROP_QA_OUTPUT, uses an isolated QA company and genuine style UI callbacks for all8 owned designs. It checks actual moving+placed renderer/property blocks, duplicate selection wallet/progression invariants, fresh StackSave.Load and real scene reload for each choice, captures style-N-gameplay/style-N-reloaded, then checks a real advanced special moving block retains its cue after repaint. Inspect all8 screenshots for clearly distinct main surfaces and preserved gold volume; run existing commerce/revocation/gameplay regressions afterward. Source tests do not establish physical or native visual acceptance.


Pedestal follow-up: root's actual all8styles run passed the first skin fixture, but inspecting those rendered shots exposed a stale prior-color foundation. Initialize created Pedestal through the correct palette path, while RefreshProfileStyle omitted it because it was not part of `_placed`. Added a bounded `_pedestal` owner reference assigned at creation and repainted alongside placed/moving slabs; offcuts remain untouched. The production-method harness was extended to execute the actual RefreshProfileStyle body and fails before the change with `STYLE_RENDER_FAIL actual RefreshProfileStyle leaves owned pedestal in previous color` (retained /private/tmp/pd-pedestal-production-red.log); it now passes. Editor fixtures also construct and verify pedestal geometry, while CheckLiveStyle requires the real named Pedestal and validates its actual shader/MPB on every UI choice and post-scene-reload check. Bundled-engine runtime/editor-style compilation and scoped whitespace checks pass; new actual Unity/UI-selection/reload/screenshots remain root-coordinated.

## Coordinated polish/design switching engine acceptance — 2026-10-09
Actual Unity Mac build passes8style renderer/preview/reset/special probes,782presentation core and15910measured gallery/renderer/target/bounds assertions; shaders compiled without Shader error. Initial added mesh-reflection probe hit AmbiguousMatchException because Unity also exposes a deprecated Mesh overload of OnPopulateMesh; the fixture now explicitly selects VertexHelper. Production change was not required for that fixture failure. Independent review found signed pow bands could produce invalid GPU values and disabled gold contrast was too low; squared bands now use multiplication and unavailable buttons keep opaque brighter tint.
Fresh actual -qaStyles passed all eight UI selections with live shared-shader consumed properties on body/crown/plate/rims, no wallet/ownership/repeat charges, save/scene reload, and special moving-block cues. Inspection of the first8screens exposed an additional stale-color pedestal, which was omitted from the placed list. An explicit pedestal owner now repaints with the selected style. Standalone actual RefreshProfileStyle probe RED before correction/GREEN after, editor fixture includes the actual pedestal, then fresh -qaStyles2 repeats all8+scene-reload with the pedestal included. Root re-inspected jade/pearl screenshots: stale prior-color rim is gone.
Fresh -qaPresentation passes actual guide placement/replay/save, galleries, city, portrait/landscape clear game area. Root inspected gold gameplay, all8designs, landscape layout, illustrated level gallery and architectural city. Fresh -qaArcade passes campaign completion/next, powers, special blocks, risk failure/reward, city/daily/styles and beyond30endless/bounded geometry. No physical OS-touch/performance or real provider-purchase assertion. Source/ProjectSettings prepared for internal9; older TestFlight8 still lacks these corrections and native9 export/upload is pending.


## Verified internal presentation revision — 2026-10-09
Fresh native **1.0(9)** delivery from source `95d76275d8ee5b71ae82a1f60d0c26572aea8250`. iOS Release archive/export succeeded, strict/deep Apple Distribution signature and teamTKG684N5GL verified, get-task-allow=false, QA runner excluded. IPA SHA256 `c0238c85b0292c103b76544c630ba1102a36f0f84bdf621b6edecad1aa8dac88`. Compressed archive all60files byte-verified, SHA256 `9e316452fb5a700cea94577da5506e299affeba4eb05c8157c64d8905d970c82`; full archive and upload metadata have verified durable digests in unpublished draft `perfectdrop-build9-presentation-tutorial`. Xcode GUI InternalOnly upload completed with the known vendor UnityRuntime.framework dSYM warning; own symbols preserved. Authenticated ASC UI confirms **Im Test**, existing Kamilunavo Intern group,1tester and saved DE test notes, build ID `91529c26-b976-41bf-b35f-5776c4a99c63`.
Android manifest/version/own AdMob and all6ARM64 ELF16KB alignments validated. Central signing run37995021514success; both existing universal certificate pins match, all570non-signature payload entries and manifest byte-identical. Final signed AAB SHA256 `c26abe449ee588e0780810214cbfec3d520a76e616256eef588931d413d9d223`; final package/provenance and proof asset digests freshly verified on durable unpublished draft.
Native compile inputs were retained after disk exhaustion; identical generated Gradle inputs successfully packaged after removing only owned regenerable compile caches. OneMoreFloor additionally retried native generation after a debug-symbol copy exhausted disk; RepairEmpire retried an unchanged Xcode export after its linker exhausted disk. These environmental failures were not counted as passing runs.
All nine coordinated runtime acceptance receipts are retained locally at `/Users/piotrkaminski/Developer/KamilunavoDelivery-20261009/work/runtime-acceptance-manifest.json`; actual rendered screenshot checks and DE/EN portrait/landscape flows are documented above. Full Unity sources are on canonical main. Owned desktop/simulator QA output bundles, Library/export/DerivedData and duplicate uploaded archives removed only after newer acceptance and retained verified artifacts; original user simulator devices, player profiles, receipts and keys untouched.
No public App Review submission or Play publication. Real physical iPhone multi-finger input/performance and genuine provider purchase/reward acceptance remain distinct device gates. These builds implement specific tested graphics/UI improvements; they do not establish full cinematic equivalence to concept stills. Delivery record: `docs/INTERNAL-PRESENTATION-REVISION.json`.


## Approved October 10 architectural menu gallery — source implementation

- [~] Campaign menu now has an opaque ivory gallery, larger actual shared-mesh hero over owned architectural scenery, three illustrated chapter cards, a compact ten-level scrolling gallery and one gold Continue/Play. White/navy utility hierarchy, gallery designs/shop and city navigation use the same palette. Logical-point typography/targets and distinct portrait/landscape layouts replace canvas-unit assumptions. Last-row padding removed so the final level can fit wholly at the clamped scroll bottom. Existing callback/QA names, progression/commerce/tutorial semantics and world reservations preserved.
- [~] New `GalleryMenuValidation.Validate`, integrated into `ArcadeValidation.Validate`, rejects the previous clear/dark map identity and covers three distinct chapter scenes, actual specimen, one gold primary, DE/EN all-state4.5contrast, >=48point targets and first/last-card reachability across five compact/reserved panes. Root owns actual engine execution/rendered acceptance.
- Fresh all-source Roslyn typecheck succeeds with existing deprecated API/reference warnings only; focused changed-file whitespace check passes. Global whitespace still reports parent-owned pre-existing `ProjectSettings.asset:303`; that file was not edited by this source owner. No Unity/build/native/version/commit/push/publication operations performed. Root's mesh/studio/palette work remains separately owned. Existing user-deleted historical concept preserved. Details/files/entrypoints: `docs/PERFECT-DROP-GALLERY-2026-10-10.md`.

## October 10 final coordinated desktop acceptance
Rounded six-renderer slabs, broad brushed gold plate, clean circular stone compass dais, real cached style specimens and the ivory/navy gallery are now exercised in the actual Unity Metal player. Styles change dominant body, plate and pedestal; jade and pearl have dedicated consumed shader finishes. Districts use earned architectural silhouettes, streets and landscaping with owned batched meshes.
Fresh mac-gallery-final-build.log succeeds with strict style, gallery, city and presentation checks and compiled shaders. runtime-gallery-final/result.txt PASS covers guide/replay/skip, portrait/landscape reserved play areas, moving-block framing, compact gallery/shop scrolling and earned city. runtime-styles-final/result.txt PASS covers all eight real UI selections, distinct GPU specimens, save/scene reload, duplicate selection/wallet invariants and special cues. runtime-arcade-final/result.txt PASS covers campaign/next, powers, risk, daily exact-once rewards and 66-block endless bounded geometry. The stale default accent expectation in the QA fixture was corrected to the production palette; no economy behavior changed. Root reviewed actual menu, clear landscape stack pane, city and selected-style captures.
Evidence root: /Volumes/SSK SSD/Kamilunavo/Builds/VisualRevision-20261010/PerfectDrop. Original deleted PRIMARY-CONCEPT.png remains unstaged. This is desktop engine/rendered acceptance, not new native TestFlight delivery, physical multi-touch/performance or genuine purchase/ad callback acceptance. Cinematic equivalence to illustrated concept stills is not claimed.


## Verified internal concept-polish delivery — 2026-10-10

Native **1.0(10)** is built from source `69c34195b54e0b72b65e687c211bf24a43e56d56`. Fresh iOS Release archive/export, strict Apple Distribution signature, team TKG684N5GL, get-task-allow=false, ARM64 app/framework and excluded QA runner were verified. IPA SHA256 `818f6e1a5da315886c3e7c3a5780e737630832f5da2fd580866cc2eaa31401d7`. The compressed archive was verified byte-for-byte across 60 files and retained on the APFS SSD, with a server-digest-verified private draft backup `perfectdrop-build10-concept-polish`. Xcode InternalOnly upload completed. Authenticated App Store Connect UI confirms **Im Test**, the existing **Kamilunavo Intern** group with one tester and saved German test instructions. New build ID `4af6e971-2599-4e48-ab02-5351c3b37e73`.

Android native export and bundle validation passed with the expected manifest, version, own AdMob ID and six ARM64 libraries aligned for 16 KiB pages. The existing central-signing workflow [38076258994](https://github.com/acciento89-bot/perfectdrop/actions/runs/38076258994) succeeded from the exact source commit; the original universal certificate SHA1/SHA256 pins match and all 570 non-signature bundle entries are byte-identical. Signed AAB SHA256 `cd235d589851825bdf30fbefa4ead8d3fc9501454ef084ba65541525d90f6033`. Signed package/provenance/proofs and desktop evidence are retained on the SSD and in the same private draft, with fresh server asset digests checked. No local private keystore was created or downloaded.

The coordinated desktop acceptance receipts and actual rendered views are recorded in `docs/INTERNAL-PRESENTATION-REVISION.json`. [Three actual Unity views](QA/2026-10-10/actual-game-views.png) show the present implementation, not an illustrated concept or an iPhone capture. All Unity sources are backed up on canonical main; the original user-deleted historical `docs/concepts/PRIMARY-CONCEPT.png` remains unstaged. Native caches/exports/archives are retained now that the SSD has sufficient capacity.

Acceptance remains bounded: desktop event-driven input and rendered QA do not establish physical iPhone raw multi-touch, thermal or 60 FPS acceptance; genuine provider purchase/reward transactions and full cinematic equivalence to the concept illustrations remain open. The known vendor UnityRuntime.framework dSYM warning does not prevent internal delivery; the game's own and UnityFramework symbols remain retained. No public App Review submission or Play publication was performed.


## Review revisions and original music — 2026-10-11; TestFlight blocked

Four different original musical scores now ship as bounded imported loops with an independent Music choice and master Sound mute. Fresh local Unity aggregate/Mac build exit 0 includes 42 music checks and zero music validation errors. Detailed provenance, current limitations and evidence: `docs/MUSIC-DESIGN-20261011.md`. Intended music source gain .22, .35-second fade, separate focus/pause gates, one owned voice; no third-party music or samples. Physical audible mix acceptance remains pending.

Own focused navy landing separates Campaign navigation and preserves submenu parent/scroll. Navigation2268 editor checks passed. New modeled terrace world/pedestal and taller rounded tower bands pass290046 geometry/lifetime checks. Actual66-block player completed campaign, powers, special-block/risk, reward, bounded endless/reload and menu flows. Fresh all-eight-design player render validation remains required for the final shader/art revision. Actual tutorial/menu screenshots still fail concept similarity: environment is too flat/sparse and the menu skyline artwork is inconsistent with the architectural concept.

All current visual release reports remain failed/pending. Passing tests, mesh counts or a local build is not graphical acceptance. Do not upload any new TestFlight build until exact-source actual Unity renders satisfy the user’s concept gate. The user-deleted historical docs/concepts/PRIMARY-CONCEPT.png remains unstaged.

Fresh final-style actual Mac player PASS: all8 real UI design selections change moving/placed shader finishes, preserve special-block cues, wallet replay and scene/save reload. Jade and Perle actual gameplay pictures were inspected. Evidence: Builds/RisingExpansion-20261011/Drop/MusicStylesRuntime/result.txt. Styles are technically applied; overall sparse/flat environment still FAILS concept similarity, so no TestFlight upload.

## Approved Shop and completion ads — 2026-10-11

New source adds a visible direct Shop, permanent separate Remove Ads nonconsumable bit4 (Germany EUR1.99 requested catalog target; actual store metadata only), premium purchase and Restore before long lists. Forced interstitials are considered only at safe genuinely successful result boundaries after at least3 successful completions and at least180 seconds since session start/last actual fullscreen ad. Tutorials, driving/repair/active play, background, other native providers and unavailable ads are excluded; no deferred show. Voluntary rewarded videos remain optional and delay subsequent forced advertising. Purchase persistence/idempotency/partial-catalog revocation protection and parent Back are preserved. QA disables native SDK requests including Store reconnect/purchase/restore; privacy invalidates cached forced ads and generations.

New engine fixture: `Kamilunavo.PerfectDrop.Editor.DropInterstitialValidation.Validate`. Independent source review findings were corrected before fresh root validation. Exact final validation/build/runtime receipt lives under `/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/Commerce-verified-evidence.json`; it distinguishes engine/player success from unrun real-device/store/graphics gates. Catalog creation/price, live units, actual device sandbox purchase/restore/refund and UMP/SDK lifecycle remain pending. All4 visual concept gates remain FAILED; no TestFlight upload or version increment.


## 2026-10-11 — actual Concept comparison and graphics refinement

Replace unrelated modern-skyline menu artwork with cached genuine terrace-world renders; deterministic chapter lighting/probe snapshot, closer architecture, all-side windows/cornices, mineral surfaces, bounded owned 3D cloud relief and stronger metal reflections.

Mac build exit0; preview27 incl opposite ambient-probe cold renders/leases/restoration; terrace177143 vertices/14active renderers; cloud12/nav2436/style8/interstitial53/music42. Fresh actual UI and all8 style selection/shader/save/reload suites PASS; runner intentionally terminated only after PASS (exit143).

The sparse historical skyline, repeated luminous windows, simplified vegetation/cloud shapes, lighting, slab proportions and flat menu typography still fail the approved Concept. **Visual gate stays FAILED; no TestFlight.** Exact proof/capture paths and remaining gaps: `docs/GRAPHICS-COMPARISON-20261011.md`.


## 2026-10-11 — original authored asset pass and actual Concept evidence

Original rounded lacquer shoulder and turned gold rim profiles; continuous proportional perimeter UVs and geometric face normals on real overhang cuts; cached architectural metal reflection; larger portrait game reservation/lower camera elevation and compact HUD with minimum49point Menu; static authored world cloud billows. Added isolated actual21-drop three-chapter capture mode with accurate desktop/mobile receipts.

Final Mac aggregate/build exit0; authored slab15600checks; compact actual Menu RED44points to GREEN>=49, measured16060checks; cloud12checks. Actual Mac21orderedUI drops/3chapters plus eight actual design/shader/scene/save/reload selections on final cloud build PASS; player terminated only after receipts PASS.

The real slab shoulders improve, but cloud volume, sparse historical skyline, material lighting and flat menu hierarchy still fail the approved Concept. New sky still needs atmospheric polish and mobile GPU measurement. **Visual acceptance stays FAILED. No TestFlight or version bump.** Exact proof/scene-source digests: `docs/GRAPHICS-COMPARISON-20261011.md`; provenance: `docs/ORIGINAL-ASSETS-20261011.md`. User-deleted historical Concept PNG remains unstaged.


## 2026-10-11 — complete original Perfect Drop presentation rebuild

Complete Perfect Drop presentation rebuild: real rounded thick slab kit, original metal/mineral/limestone materials, transparent Roman gold title, compact HUD and circular powers, gold pill actions, genuine chapter/hero/style previews, explicit parent Menu and visible Shop. Original open arcades, canal bridges, recessed facades, turned columns, cornices, ribbed domes, planters and 50 buildings/landmarks per chapter in owned batched meshes. Smaller grounded compass dais, separate day/sunset/neon grading, HDR highlight shoulder, depth-normal contact shadows and corrected downward cloud mapping.

Final Unity6000.6.4f1 Mac aggregate/build exit0. Slab15600, measured menu27440, navigation5406, terrace1312165 checks with249137 owned vertices/14 active scenery renderers; cloud18, environment previews46, interstitial53, music42. Actual Mac player:21 ordered UI placements across3 chapters, all8 real design selections/shader/special-cue/wallet/save/reload checks, portrait/landscape/tutorial/parent-menu flows, campaign/powers/risk/daily/city and66-layer endless PASS. Bundle and canonical source hashes match all4 serial receipts. These are engine/desktop functional results, not visual approval or mobile performance.

**Concept acceptance is FAILED; TestFlight remains blocked.** Below approved Concept: sparse visible skyline composition, simplified vegetation and architectural silhouettes, insufficient rich/champagne metal lighting, coarse plate grain, broad stone paving, stretched-looking cloud projection and flatter menu/gallery surfaces. Day/sunset/neon and chosen styles visibly differ, but fidelity to the illustration remains inadequate. Pale secondary gameplay text needs stronger contrast on bright scenery. Physical mobile GPU/thermal/frame-rate/touch, audible music mix and real store/ad-provider acceptance remain unverified.

Exact reviewed source `296655b882bf2d22a5ff850c42aa39bf50812993d3bef5c9f2283fe8df51a2d5`; immutable framebuffers/logs/receipts: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/CompleteRebuildReviewed-296655b882bf`. Comparison and source review: `docs/GRAPHICS-COMPARISON-20261011.md`; original asset prompts/hashes/license: `docs/ORIGINAL-ASSETS-20261011.md`. Other games were not changed or run during this rebuild. Historical Concept deletion remains unstaged.


## 2026-10-11 — original distant city and engraved compass refinement

Complete Perfect Drop presentation rebuild: real rounded thick slab kit, original metal/mineral/limestone materials, transparent Roman gold title, compact HUD and circular powers, gold pill actions, genuine chapter/hero/style previews, explicit parent Menu and visible Shop. Original open arcades, canal bridges, recessed facades, turned columns, cornices, ribbed domes, planters and 41 buildings/landmarks per chapter in owned batched meshes. Smaller grounded compass dais, separate day/sunset/neon grading, HDR highlight shoulder, depth-normal contact shadows and original painted day/sunset/neon distant environments on a closed camera-following shell with far-plane depth. Real tower, support, near architecture and earned city remain meshes. Bounds-normalized original compass artwork and actual3.57-radius support correct the unit-radius builtin cylinder; anchored lanterns, original botanical cypress cards, original soft world-anchored cloud cards, polished gold, outlined gameplay hints, fluted/gilded showroom arch, stone specimen plinths and49-point visible selection plates form one combined graphics batch. Landscape uses a fixed five-floor camera window, a tall left world reservation and right action rail. Short safe panes reflow to three rows, tested at190/226/333/349/355/394 point heights, with actual feedback-text completeness checks at both native point units and fractional Canvas scaling.

Final Unity6000.6.4f1 Mac aggregate/build exit0. Slab15600, measured menu109816, navigation5406, terrace1220435 checks with236731 owned vertices/15 active scenery renderers, distant-city22 actual rendered coverage/near-far depth checks, original cutout6 actual alpha/orbit/resource checks; cloud18, environment previews49, interstitial53, music42. Actual Mac player:21 ordered UI placements across3 chapters, all8 real design selections/shader/special-cue/wallet/save/reload checks, portrait/landscape/tutorial/parent-menu flows, campaign/powers/risk/daily/city and66-layer endless PASS, including actual landscape landing-width comparison at8/30/66 floors. Bundle and canonical source hashes match all4 serial receipts. These are engine/desktop functional results, not visual approval or mobile performance.

**Concept acceptance is FAILED; TestFlight remains blocked.** Fresh actual Unity menu/gameplay/showroom images remain below the approved Concept: near facades and foliage are simpler and darker, gold lacks the same reflective corner highlights, the cloud/canal horizon shows a visibly flatter painted/geometry transition, and the showroom arch/plinth detail and menu card hierarchy still lack the reference ornament and material depth. The showroom foot is now fully framed and owned styles are visibly distinct, but this does not approve Concept similarity. Gameplay hint text remains visually small over the busy world; measured text completeness is not visual readability approval. No TestFlight or native distribution.

Exact reviewed source `d8533cc03727c88aa260cc3572fff72bd41b310a79b9ea9cec037ede828fec14`; immutable framebuffers/logs/receipts: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/CompleteRebuildReviewed-d8533cc03727`. Comparison and source review: `docs/GRAPHICS-COMPARISON-20261011.md`; original asset prompts/hashes/license: `docs/ORIGINAL-ASSETS-20261011.md`. Other games were not changed or run during this rebuild. Historical Concept deletion remains unstaged.


## 2026-10-11 — human-accepted final polish, candidate 1.0(11)

Latest human feedback accepts the shown direction as almost finished and explicitly authorizes the best remaining polish followed by TestFlight. Fresh direct review of actual Unity images passes for this internal candidate, under that accepted direction: brighter champagne/gold reflections, actual coin medallions, dark readable feedback backing, corrected cloud-plane/aspect sampling, and settings labels correctly converted to logical points. Settings retain49-point buttons in portrait, paired landscape and compact three-column layouts. Fine architectural ornament remains simpler than the illustration; no literal pixel identity or physical-performance claim.

Frozen source `abf63890abb36028b825c08883fb0c4631f5fff59328bb1e02ed0c96a3c54cb5`. Fresh build exit0; navigation8934 including setting-font completeness/density; four serial Mac modes PASS including all8 real finishes/save/reload and66-floor landscape landing size. Immutable evidence: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/FinalPolishReviewed-abf63890abb3`. Actual unaltered comparison: `/Volumes/SSK SSD/Kamilunavo/Artifacts/GraphicsComparison-20261011/perfect-drop-vergleich-final11.png`. Previous failed review remains preserved in that evidence directory; this new exact-source review supersedes it for the explicitly accepted internal direction.

New native build/export receipt binds every exported file to the reviewed Unity source. Upload refuses stale exports or existing archives, uses CocoaPods workspace when present, and preserves build11. Native IL2CPP/Metal, archive/signature and actual Apple processing results must be added below before claiming delivery. Only Perfect Drop was changed. The historical user-deleted Concept file remains unstaged.

Remove Ads `com.kamilunavo.perfectdrop.removeads` now exists in App Store Connect (AppleID6821711775), Germany base EUR1.99, all175 regions and saved German/English localization. Optional rewarded videos remain independent. Internal builds keep official test advertising; real sandbox purchase/restore/refund, real provider consent/impression and physical touch/FPS/music mix acceptance remain open. No public App Review or production ad activation.


### Candidate11 native evidence

Fresh Xcode simulator build succeeded from the exact source export. Actual iPhone17Pro/iOS26.5 IL2CPP/Metal/UIKit concept/styles/presentation/arcade modes allPASS:21 ordered placements, all8 finishes/shader/reload, real portrait1206×2622 and landscape2622×1206 framebuffers/safe areas, parent navigation and66-floor endless. Root directly inspected native menu/day gameplay/gallery, readable Settings at both orientations and8/66-floor landing scale. Evidence `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/NativeFinal11/native-qa-receipts.json`; native comparison `/Volumes/SSK SSD/Kamilunavo/Artifacts/GraphicsComparison-20261011/perfect-drop-vergleich-ios11.png`.

Ordinary native startup was alive25seconds with StoreKit2 initialization and an actual OS screenshot. Simulator screenshot service cannot write directly to the external volume; the bounded ordinary-startup capture was repeated through an owned temporary screenshot copied to the SSD. The four passed gameplay modes were preserved and not repeated. ASTC fallback on simulator and optional StoreConnected-event subscription warning are recorded, with no claim of real store transaction success. Physical touch/FPS/thermal and real sandbox purchases/ads remain unverified.

### Candidate11 signed upload (2026-10-11)

Fresh device Release export from the same frozen source passed the provenance guard before and after native archiving. Xcode workspace archive and local signed export succeeded. Version1.0(11), bundle `com.kamilunavo.perfectdrop`, Apple Distribution teamTKG684N5GL, `get-task-allow=false`, deep/strict signature verification PASS; QA runners absent from the signed player. Signed IPA SHA256 `091975d2780538786b2f81f1dacb9b938be78cdb7b0e652919304fd75e7c1db1`.

The command-line upload failed at the account providerId lookup. Xcode Organizer successfully uploaded the same reviewed archive using TestFlight Internal Only. App Store Connect acknowledges Build11, uploaded Oct11 at22:48 local, initially Processing. Upload warning is limited to the vendor UnityRuntime.framework dSYM; game/UnityFramework symbols remain retained. No second upload or source change. Evidence directory: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/NativeFinal11`, including signed-verification.json, xcode-upload-completed.jpeg/text and testflight-delivery-receipt.json. Source implementation commit `93033df` is pushed to main. Test availability is not asserted until the actual Build11 group/status is confirmed below.

### Candidate11 delivery confirmed

Actual authenticated App Store Connect UI confirms **1.0(11) Im Test**, AppleBuildID `7d2ca7a3-35cb-4179-bfea-ee8e1360a3d4`, assigned to existing **Kamilunavo Intern**, one tester. German What to Test notes saved; saved state and group details inspected. Completed the standard encryption questionnaire with none of the listed additional algorithms, based on the absence of custom app encryption and Apple OS networking/StoreKit use. No source/build mutation.

Delivery proof hashes and exact source/IPA are recorded in `docs/QA/testflight-build11-delivery.json` and the external receipt. Only internal TestFlight was activated. RemoveAds EUR1.99 is configured in Apple catalog, but actual sandbox purchase/restore/refund, consent/ad callbacks and physical-device touch/FPS/music mix still require device testing. Official test ads remain active; production monetization and public App Review were not activated. This internal delivery is complete.


## 2026-10-13 — completed coordinated desktop Concept polish

User authorizes Perfect Drop→Rising Steps→Repair Empire sequentially while OneMoreFloor candidate5 awaits physical-device/login handoff. Perfect Drop now hides tower/HUD/action behind an opaque complete Canvas in Settings and restores the exact original submenu, including Designs→Shop→Settings→Shop→Designs. Menu scene fills its pane; actual portrait/wide hero leases use the real reserved region, larger rectangular chapter cards and a direct gold coin/Shop medallion. Native UnityMac created an editable original architectural art scene/resource library and six-face HDR terrace/softbox reflection, consumed once by the player. Camera captures exclude display grading, keep64pixel HDR/mipmaps, and add no live probes. Saved navy, jade and pearl materials stay aligned with game finishes.

Actual before-fix REDs were preserved. Native Unity6000.6.4f1 aggregate/build passed, including8934navigation,1.2Mterrace, minimumtouch/font/density constraints, opaque-menu/parent/run-integrity and actual HDR/studio binding checks. Final source0e4c630fba7f8feb1a6998addc81ea5e74b356662634612fcc2eb720bcab44d6: four serial realMacplayer modes allPASS —21ordered placements/threeworlds, all8actualUI finishes/wallet/shader/cue/save/reload, orientation/tutorial/city/menu, and66-floorcampaign/abilities/risk/daily/endless. Constant landscape landing size verified at8/30/66. Receipts record exact executable/wholebundle/source hashes:docs/QA/drop-polish-20261013.json. The nativeEditor artifacts include the generated defaultIAP billing resource, PackageManager UI preferences and whitespace-only PlayerSettings serialization, tracked to reproduce exact frozen source.

Functional-pass revisions with dark/washed-out gold were explicitly rejected visually; their raw evidence remains. Final independent actual-image review clears architectural/UI/material handoff in previously human-accepted direction; this is qualitative similarity, never a measured90% or literalpixelidentity. Reference versus actual image comparison:Builds/DropPolish-20261013/perfect-drop-concept-unity-vergleich.png. Final gallery retains ordinary view-dependent plate brightness and simpler filigree than the illustration. Visual gate passes the exact desktop source; new nativeiOS/physicaltouch/performance and real store/provider transactions remain separate and unverified. No new TestFlight distribution; build11 remains the last delivered iOS binary. Historical user-deleted Concept remains unstaged. Proceed directly to Rising Steps, then Repair Empire.


### Human rejection of shown October13 comparison

The user explicitly rejects the presented comparison as worse and insufficiently close to Concept. This supersedes the internal visual acceptance recorded above: visual gate nowFAILED, new PerfectDropTestFlight blocked. Keep passed functional/nativeMac receipts as bounded functional evidence. Reopen complete tower/material/menu own-asset reconstruction, comparing against the approved three-view board rather than repeating lighting-only tweaks. RisingSteps was read-only onboarded; its pre-existing uncommitted art remains untouched and no Editor/build was run. Resume RisingSteps and RepairEmpire sequentially after correcting current PerfectDrop visual target.


## 2026-10-13 — original ConceptV2 production assets, still below graphical release gate

Human rejection remains controlling; the prior internally passed graphical review has been withdrawn. PerfectDrop stays the active correction; RisingSteps and RepairEmpire remain queued in that order.

Built-in imagegen created seven original production assets, copied unmodified into Resources/Art/ConceptV2: actual glossy 9slice gold action, separate navy satin/champagne gold physical surface maps, three original rectangular world illustrations, and transparent minted gold coin. Exact source/output hashes and normalized prompt intents are in docs/QA/drop-concept-v2-assets.json. Menu illustrations are labeled production art, never gameplay proof. Shared live slab meshes now have cleaner profiled shoulders, restrained gold trim and distinct physical finishes. Menu has a progressively stepped actual grounded specimen, lifted composition, own larger gold CTA and selected design actions, improved chapter/nav hierarchy, compact header controls and larger authentic coin/Shop capsule. Preview softbox is a real temporary spot light on the isolated rendering layer; no live per-frame probe/light or gameplay foot-print changes.

Native Mac Unity Editor imported assets, saved actual mesh/material library updates with preserved asset identities, compiled, ran the strict full aggregate and built the native candidate. Before fixes, added regressions reproduced duplicate capsule frame, flattened saved mineral seeds and remaining owned-selection frame. Source and persisted studio now pass, including real owned/revoked/restored selection transitions. Saved mineral seeds retain actual per-floor values. Read-only code recheck has no remaining findings.

Frozen source c84bc3a34ddebdc0558d519cf8e2026b5c73a4f7400355fe062a22b0ab1b74b1 passed all four serial native player matrices: 21 actual placements across three chapters; all eight actual UI designs and shader/ownership/save/reload; menus/tutorial/portrait/landscape; campaign/powers/risk/daily/endless66 with fixed landscape sizes at8/30/66. Exact executable/bundle/source evidence and untouched native screenshots are recorded in docs/QA/drop-concept-v2-20261013.json. Actual Concept/Unity comparison pairs are retained alongside the candidate.

This verifies the concrete asset integration and functionality, not 90% likeness or a completed graphical rebuild. Gold reflection/detailing and foreground architecture/vegetation remain visibly simpler than the illustration. FAILED visual gate is retained with a pending-candidate pointer. No new TestFlight upload or iOS/physical-device/provider success is claimed. Historical user-owned Concept deletion stays unstaged.


## 2026-10-13 — ConceptV3 own Menu / Design artwork and sculpted editable models

User asks to continue the approved direction, finish remaining Menu/Design details and create as much original art as possible. Six additional original raster assets were generated using built-in imagegen: Navy, Ivory, Jade and Pearl card surfaces; transparent selected-card gold frame; transparent Designs navigation illustration. Unmodified source hashes and exact prompts are retained in docs/QA/drop-concept-v3-assets.json and drop-concept-v3-prompts.json. They are consumed as scalable 9slice UGUI surfaces with real clipping, semantic state tint preserved in byte/vector space, and actual selected chapter/style highlight. Gold primary actions remain the prior own sprite. The illustration is explicitly navigation artwork, not a substitute for live tower geometry.

Design view now has compact upper contextual Back and authentic coin/Shop shortcut; Shop still returns to Designs. Original slate/Navy, emerald and dark Pearl glass surfaces replace flat card fills; all eight existing real tower designs retain purchasing/ownership/save/cue behavior. New modeled plinths have turned profiles, fluting and gilded ornament; the native Editor persisted three editable prefab + mesh/material libraries, included in the actual built player and consumed by its specimen renderer. Export omits temporary resource ownership, so destroying a preview does not destroy persistent source assets. Original gameplay six-renderer pieces, physical placement footprint and five-floor landscape view remain unchanged.

Native Unity6000.6.4f1 import/authoring, strongest aggregate and build passed, including8934navigation checks, exact selected-frame authority, original card binding, owned action transitions, new coin-Shop-parent behavior, renderer margins and preview-resource ownership. Read-only code review reports no important findings. All four serial actual-player modes passed for frozen source c6df021b3e3992be2db9ce800782ca2a11bdbd234bdaf51532a6ab66d3b8dc6a:21ordered placements, all eight UI styles/wallet/save/reload/actual shader properties, portrait/landscape/menu/tutorial, and campaign/powers/risk/daily/endless66 with fixed8/30/66 landing sizes. Source/executable/bundle fingerprints, actual screenshots and original prefabs are recorded in docs/QA/drop-concept-v3-20261013.json.

Current Concept/menu and Concept/selected-Jade comparison pairs are unretouched actual Mac renders, proportionally scaled only. This closes the concrete original Menu/Design asset integration; it does not assert numerical90% or clear the earlier failed overall graphical release gate. Foreground architecture / metal reflections and native iOS/device/provider gates remain separate. No new TestFlight upload. PerfectDrop remains active; RisingSteps→RepairEmpire queue unchanged. Historical user-owned deleted Concept stays unstaged.

## 2026-10-13 — actual sculpted tower / main-menu reconstruction

User specifically rejects the remaining real main-menu and gallery tower forms, and requests original models implemented in the game. Shared six-renderer geometry now has rounded shoulder profiles, broad turned double gold molding (0.18 physical layer), a separate raised and beveled gold plate and its original crown. Body/rim stay below1000vertices each and within full and narrow retained/cut footprints. Actual menu contains six progressively contracted grounded tiers, and the design showroom has seven substantial narrower tiers; live-game physical heights, movement, placement and landscape framing remain unchanged.

Created an original polished emerald jade material with built-in imagegen, copied its PNG byte-for-byte and imported natively with1024max, mipmaps, Repeat/Trilinear and sRGB. Exact prompt/source/output hashes live in docs/QA/drop-sculpted-towers-{prompts,assets}.json. It is consumed by the actual selected Jade shader; Pearl/Navy remain distinct. Native editable studio synchronizes actual part transforms and preserves existing asset identities; newly distinct crown/plate profiles get separate persistent meshes instead of overwriting the historically shared undercore box.

Actual screenshots exposed bleached white gold and noisy mineral normals. Native GPU regression reproduced HDR gold(3.6,1.8,.5) becoming(.975,.942,.500). Final shared-peak tone compression retains(.975,.488,.135), ordered highlight gradients, dark black metal and original alpha; source-bound GPU checks pass. Real preview lighting is scoped/temporary and restores incoming ambient/reflection state. Shader/art/geometry/sidebar changes introduce no render-pipeline migration or live per-frame preview lights.

Main menu now has a smaller correctly placed logo, larger navigation row, rectangular world cards positioned higher, a smaller pill level plaque and a freer stepped tower/dais. Portrait gallery removes its redundant header, raises the actual selected column, preserves common card bottom/mask limits and top-aligns real specimens. Selection callbacks immediately update height from the actual profile; ownership/purchase/restore/revocation and Shop-parent navigation remain authoritative.

Final native Unity6000.6.4f1 strongest aggregate/build pass, including8934navigation checks, <1000profile budgets, actual narrow bounds/winding/tangents, genuine opaque settings, parent navigation, seven/nine specimen margins and cold-preview lighting/ownership. Four focused read-only reviews have no important findings. Frozen source360464d61a92b254a6e724521739486c2c1e660859232cb174f5d9004faad9e1 passed all four serial actual-player modes:21ordered drops across three worlds; all eight actual UI designs/shader/wallet/save/reload; portrait/landscape/tutorial/menus; campaign/powers/risk/city/daily/endless66 with fixed landing widths at8/30/66. Full exact source/executable/bundle evidence is docs/QA/drop-sculpted-menu-20261013.json.

Intermediate timing/button failures are preserved with frames/logs; OS foreground was observed false in the second run and the external focus guard's quoting was corrected. The controlled exact-source run then passed all modes without altering gameplay lifecycle/pause handling. Final receipts record any activation of the runner's own Player PID. Desktop functional timing uses captureDeltaTime and is not a physical mobile performance claim.

Unretouched Concept/native menu and selected-Jade comparison pairs are retained with the candidate. Fine metal reflections, architectural richness and carved base details still differ from the illustration. No numeric90% or overall graphical release is asserted. Existing failed human visual gate stays controlling, with this exact-source candidate linked for evidence. No new TestFlight upload. PerfectDrop stays active; RisingSteps→RepairEmpire queue remains. User-owned historical Concept deletion stays unstaged.


## Public App Review submission — 2026-10-13

The human explicitly accepted the sculpted-menu candidate and authorised completing all store metadata, purchases, ads, screenshots and public App Review. Source94956d071e4d6903c2e495661e019e7eac82a69cc351cdc3ba49210a6657c50c preserves the accepted graphics; only production ad configuration, non-personalised/G-rated requests and build12 changed. Native iPhone concept21 drops, all8styles, corrected real-rotation arcade/powers/risk/daily/endless66/reload and iPad concept21 drops pass. Retained failed arcade probe compared a landscape threshold while deliberately retaining portrait; rerun enables the existing native-rotation option, with no weakened assertions or game changes.

A separate external Apple XCTest driver touched the real wallet/Shop control in an ordinary native simulator profile with SDKs enabled. Actual StoreKit metadata loaded all3prices (USD1.99/1.99/2.99). No paid transaction, restoration fulfilment, live advertising earnings or physical-device performance result is claimed. The simulator project was restored afterward; the public archive was unchanged.

AdMob production iOS interstitial2586872480 and existing rewarded9205555063 use the correct owned app8084045088. A DE/EN Google UMP message with a visible decline choice is published. Existing pacing remains >=3completed rounds and >=180seconds, result-only, no tutorial/gameplay interruption, permanent RemoveAds suppression and optional rewarded videos.

PerfectDrop1.0(12) uploaded successfully with public eligibility. Apple processed it VALID / APP_STORE_ELIGIBLE, buildca83602d-fe99-49de-ac3b-e0065bd9c3fd. Vendor UnityRuntime symbols still produce an accepted upload warning. Free download, store description/keywords/support/copyright/genres/age/content/encryption declarations, published privacy label,5actual iPhone and4actual iPad screenshots and3native StoreKit review screenshots are saved. RemoveAds has GermanyEUR1.99; StarterEUR1.99 and CollectionEUR2.99.

Review submissionf3a143e9-5768-4b45-b9fc-df1713687add contains the app plus all3purchase versions and is WAITING_FOR_REVIEW, submitted2026-10-13T21:19:56.138Z. Release is manual after approval. Current receipt: docs/QA/drop-public12-20261013.json. No90%numerical similarity claim is asserted. Next work: OneMoreFloor public submission, then RisingSteps own-asset concept rebuild, then RepairEmpire.


## 2026-10-14 — device regression report; submission withdrawn by human

The human reports withdrawing PerfectDrop from Apple review. This is a human-reported withdrawal, not a new Apple API status verification. Earlier WAITING_FOR_REVIEW remains only the historical2026-10-13 submission receipt. They report that choosing an earlier world/level still starts the latest Level30, blocking completion of missing stars; framed/discontinuous menu background, poor wallet visibility and hidden Endless access also require correction. Root cause is not yet reproduced or inferred from screenshots. Latest visual release gate is FAILED and the previous accepted public12 report is archived separately. No unchanged-build resubmission, no concurrent PerfectDrop implementation. Explicit priority is RisingSteps, then RepairEmpire, then PerfectDrop correction and renewed public review; existing submission authorisations remain available.

Required later acceptance: use a completed30-level save and invoke every actual UI level card across all three chapters; each launches exactly its requested global level/world and block target. Improve a previously incomplete rating and verify independent saved best stars after cold reload, without resetting progress. Check direct visible Endless, wallet/Shop/Restore and full-window background in compact portrait/iPad/landscape/safe areas; settings hide gameplay controls. Retake genuine native store shots, build/retest advertising pacing/RemoveAds, and reselect the corrected build and all required IAP review items before resubmission. Evidence: user supplied2026-10-14 Foto1/2/3 (chapter selector, Level30 gameplay, annotated main menu).


## 2026-10-15 — replay/menu correction and renewed public submission13

Human explicitly moves PerfectDrop ahead of RepairEmpire and authorises correction/resubmission followed immediately by RepairEmpire. Reproduced cause: chapter cards changed only the preview; Play still used frontier30. They now open their ten individual level cards. Home exposes Endless/best/unlock condition and a larger wallet/Shop target. The owned original3D render fills the complete Canvas; ivory footer reaches the actual window edge, Designs/Shop remove old inset backings, and small-phone wordmark fits between controls. Settings remain opaque with no active tower/Drop and return to the correct parent/chapter. An additional red regression corrected frontier30 being described instead of the actual saved campaign/Endless/daily run. Gameplay/save/economy/monetization authority is unchanged.

Frozen source7af9cf5709efc2703e104a0b9e689a8108a0d40fee303ea5ae2b201011eca8d6 passes the strongest native Unity Editor aggregate,9462navigation checks and separate red/green replay/footer/gallery/resume regressions. Nine serial actual iOS Simulator sessions pass: compact/large iPhone and iPad replay120starts each (30actualcards ×DE/EN ×orientations), independent Level1stars1→3 with29others intact, scene reload, all8actual materials/save/reload, arcade/powers/risk/daily/city/Endless66, and current DE/EN native phone/iPad world/menu/design frames. A read-only independent reviewer approved the complete diff. Actual current Unity framebuffers were directly inspected against the unchanged approved Concept. Exact-source visual gate passes; earlier failed human withdrawal record is archived, no new human or numerical90% acceptance is invented.

An ordinary fresh process, with QA environment unset and real SDK providers enabled, displays wallet2347, world stars12/10/10 and Endless65 from the genuinely improved isolated save. External Apple XCTest taps the actual wallet and opens Shop; actual StoreKit prices1.99/1.99/2.99USD and Restore are visible. Xcode migrated the app data to another Simulator container UUID: the successful test's external post-processing wrapper initially used the stale path. Independent recovery verified migrated records/screenshots, retained them, and restored original preferences/project exactly. No paid purchase/restore, live ad impression/earnings, physical touch gameplay or device FPS claim.

Fresh Release export, signed archive and local public IPA pass source/payload/signature checks; actual release C++ excludes QA probes. Production AdMob/UMP configuration and >=3completions/180seconds, result-only interstitial rules remain validated. All20new actual native DE/EN phone/iPad Store images are COMPLETE; copy explicitly explains individual replay and direct Endless. Existing3IAP review images, German base prices1.99/1.99/2.99EUR,175territories, published privacy, free app and manual release remain. English name uses Perfect Drop - Kamilunavo because the unqualified English name is unavailable.

Build1.0(13), b81b0560-3855-41cb-9795-a44ba937559c, uploaded successfully and processed VALID / APP_STORE_ELIGIBLE. Apple accepted the existing vendor UnityRuntime missing-dSYM warning; it is retained in the report. Review submission14167654-8d95-43a6-a034-4868e52378eb submitted2026-10-14T22:52:28.453Z (15October00:52Berlin). Independent reads confirm parent submission, actual app version and all3IAP versions WAITING_FOR_REVIEW. Review item container records still report READY_FOR_REVIEW, a distinct readiness field; no duplicate/withdrawal was performed. Existing owner TestFlight group includes13, no tester invitation sent. Evidence: docs/QA/drop-replay13-20261014.json and SSD Builds/DropReplay13-20261014. User-owned historical Concept deletion remains unstaged. Next active game: RepairEmpire.


## 2026-10-15 — user-required accurate icon gate

Following the subsequent One More Floor Apple2.3 Icon notice, future uploads for every app now require source/native icon and actual gameplay comparison, effective iOS assignment and small/large variant review in `docs/QA/store-icon-review.json`. This project’s observed icon motif matches its central activity, but the new explicit native variant evidence is still pending; no artwork/Apple approval is inferred from format or tests. The guard checks full PNG integrity, source hashes and iOS overrides; 21 synthetic protocol cases pass per project (84 across the four identical guards). Evidence: `docs/QA/store-icon-guard-validation.json`. No Unity source, native binary, submitted public metadata, existing submission or distribution group was changed. Historical Concept deletion remains unstaged.
