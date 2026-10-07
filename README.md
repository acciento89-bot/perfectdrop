# Perfect Drop

Native Unity mobile arcade tower builder. Bundle ID `com.kamilunavo.perfectdrop`, Unity `6000.6.4f1`, main is canonical.

Tap moving blocks to preserve overlap; trim overhangs and build Perfect combos. Thirty short campaign levels target 6–30 placements in three chapters. Stars unlock the next level and enrich an owned city. Endless unlocks after level 5 and retains only 64 recent layers while preserving total height.

Perfects charge slow-time, centered placement and next-drop protection. Bonus, fragile, drift and wind blocks vary the timing/rules. Optional risk requires Perfect and doubles rewards. Earned coins buy persisted tower designs. A daily challenge awards +75 for three stars once per UTC day; ordinary daily claim is separate.

`GameBootstrap → StackGame → StackRun/StackPowers/StackCampaign/StackSave + StackHud` owns active gameplay. Old jumping components are inactive and preserved as history. Save migration keeps wallet/daily/best and preserves the historical jump namespace.

Mac functional/visual QA and editor matrices pass. Native mobile/art/performance and TestFlight gates remain open; see [V1 Ledger](docs/PERFECT-DROP-V1-LEDGER.md). No public App Store publishing is authorized.

Validation entrypoints: `Kamilunavo.PerfectDrop.Editor.ArcadeValidation.Validate`, `CampaignValidation.Validate`, `StackValidation.ValidatePlatform`. Development-only player probes: `-qaSmoke`, `-qaArcade`, `-qaArcadeUI`, `-qaCity` followed by a separate output directory. QA isolates its profile and runs deterministic functional timing; it is not a device FPS test.
