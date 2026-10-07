# Perfect Drop Unity Project Context — Stack transition

Updated 2026-10-07 after the user's explicit correction to classic tap-to-stack. Unity 6000.6.4f1, built-in rendering, uGUI 2.6.0, legacy input, existing Main scene/bootstrap GUID and native iOS reserved-region/haptic bridge remain. Bundle IDs/team/App Store entry remain unchanged.

Active entry: `GameBootstrap.Start` constructs `StackGame`; no jump-game components are instantiated. `StackRules`/`StackRun` own deterministic overlap, cut/perfect/miss, streak coins and exactly 30 placements. `StackGame` owns moving phase, drop events, cut-piece presentation, camera, lifecycle and persistence wiring. `StackHud` owns DE/EN contextual UI and native-division/safe-area reflow. `StackSave` owns the separate `perfectdrop.stack.profile.v1` schema, leaving old jump saves untouched.

Shared presentation: existing surface/sky/grade shaders, `WorldArt` beveled blocks/skyline, new cloud-sea shader, `FeedbackSystem`, `GamePreferences`, `HapticFeedback`. The scene remains the same. Historical jump/runner/joystick scripts are inactive and retained during validation; their tests do not validate stack gameplay.

Primary current validation: `Kamilunavo.PerfectDrop.Editor.StackValidation.Validate`. `RuntimeSmoke` and `VisualQaCapture` are included only in editor/development players; use `-qaSmoke <directory>` and `-qaScreenshot <path>`. `BuildAutomation` supports Mac, iOS device/simulator, Android APK/AAB.

Filesystem/network restrictions were lifted. Shell Unity compilation/builds and simulator access work. Native Computer Use still returned app-approval denial for Unity/Perfect Drop. No physical iPhone is reported by the current device inventory. Fresh stack runtime/native/device/TestFlight acceptance is tracked in the canonical V1 ledger; historical readiness assertions below must not be reused.

Historical onboarding is preserved in Git commit `4b2ef0f`. Follow the confirmed spec in `docs/superpowers/specs/2026-10-07-stack-game.md` and plan in `docs/superpowers/plans/2026-10-07-stack-game.md`. Main remains canonical; protect the existing local QA work and backups.

## Arcade expansion checkpoint
`StackCampaign` configures targets/goals/unlocks/records/styles/daily. `StackPowers` is run-owned serialized energy/slow/protection/risk. `StackRun` keeps total count and at most 64 recent endless layers. `StackSave` schema2 migrates fixed-tower profile metadata without clearing wallet/daily/best. `StackHud` partials `CampaignHud`/`ArcadeHud` build contextual map/abilities/style/city screens. `WorldArt.BuildPlayerCity` renders selected district only; legacy skyline is disabled for the city view. Pure validations and actual Mac arcade/UI/city matrices pass; native builds remain pending with approximately400MB free disk.
