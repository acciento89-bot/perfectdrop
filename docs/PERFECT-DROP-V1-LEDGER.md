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
- [ ] P01-T07 first iOS dev build
- [ ] P01-T08 first Android dev build

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
- [ ] P04-T08 DE/EN localization

## P05 Production visuals
- [~] P05-T01 runtime palette/material foundation
- [ ] P05-T02 final character + animation
- [ ] P05-T03 authored platform kit
- [ ] P05-T04 skyline/cloud kit
- [ ] P05-T05 landing VFX
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
- [ ] P07-T02 best/coins persistence
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
- [ ] P09-T09 staged release

## Next open task
P01-T06: open with Unity, compile/import and capture first runtime evidence.


## Unity 6.6 bootstrap verification - 2026-10-06
- [x] Project imported and compiled successfully with Unity 6000.6.4f1.
- [x] Canonical Assets/Scenes/Main.unity generated and registered in Build Settings.
- [x] iOS and Android application identifiers are configured in PlayerSettings.
