# Perfect Drop

Native Unity mobile arcade tower builder. Bundle ID `com.kamilunavo.perfectdrop`, Unity `6000.6.4f1`, main is canonical.

Tap moving blocks to preserve overlap; trim overhangs and build Perfect combos. Thirty short campaign levels target 6–30 placements in three chapters. Stars unlock the next level and enrich an owned city. Endless unlocks after level 5 and retains only 64 recent layers while preserving total height.

Perfects charge slow-time, centered placement and next-drop protection. Bonus, fragile, drift and wind blocks vary the timing/rules. Optional risk requires Perfect and doubles rewards. Earned coins buy persisted tower designs. A daily challenge awards +75 for three stars once per UTC day; ordinary daily claim is separate.

`GameBootstrap → StackGame → StackRun/StackPowers/StackCampaign/StackSave + StackHud` owns active gameplay. Old jumping components are inactive and preserved as history. Save migration keeps wallet/daily/best and preserves the historical jump namespace.

Current internal TestFlight1.0(6) includes the concept cloud-city art, textured live tower geometry, Barlow typography, VFX, wide bottom-center landscape Drop and optional native purchases/rewarded test videos. The existing Kamilunavo Intern group has1 owner tester; no public App Store/Play release. User physically tested build5 on iPhone16ProMax with good sound/haptics and no perceived stutter; this is qualitative feedback, not measured FPS. Latest native iPhone gameplay/raycast checks pass including ownership persistence/revocation repaint and66-layer endless/reload/retry; desktop stability and layout evidence are recorded in the ledger. Android versionCode6 is centrally signed with preserved native payload and correct AdMob ID.

Full Unity Assets/metas, Packages and ProjectSettings are committed on main. Generated Library/build caches are intentionally excluded and may be regenerated with Unity6000.6.4f1. Native builds must launch Unity with `-buildTarget iOS` or `-buildTarget Android` so SDK postprocessors run. iOS export validates AdMob/SKAdNetwork plist declarations; CocoaPods uses its CDN and dependencies target iOS15.0.

Monetization: UnityIAP5.4.4, GoogleMobileAds11.5.0; Starter500Coins once+Copper, Neon Collection3 designs; restore does not repeat the starter coin grant. Optional50-Coin videos, max5/UTC day with60-second cooldown, only genuine SDK completion awards. Internal builds use official test ad units. StoreKit/Play Billing sandbox purchase/cancel/restore, completed reward/cancel/no-fill, own AdMob consent message and verified privacy URL remain open. Do not claim live earnings or a fully accepted RC. Physical build6 acceptance, native performance and genuine Duo opened/divided/rotated poses remain open; see [V1 Ledger](docs/PERFECT-DROP-V1-LEDGER.md).

Validation entrypoints: `Kamilunavo.PerfectDrop.Editor.ArcadeValidation.Validate`, `CampaignValidation.Validate`, `StackValidation.ValidatePlatform`. Development-only player probes: `-qaSmoke`, `-qaArcade`, `-qaArcadeUI`, `-qaCity` followed by a separate output directory. QA isolates its profile and runs deterministic functional timing; it is not a device FPS test.
