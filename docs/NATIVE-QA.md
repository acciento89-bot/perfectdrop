# Native QA players

Release exports exclude the QA scripts. `BuildAutomation.BuildIOSSimulatorQa` creates a development-only iOS Simulator export; `BuildIOSSimulator` creates the regular release export. Both restore the previous SDK setting after building.

Managed command-line arguments were empty in the tested native IL2CPP simulator player. Supply an explicit simulator launch environment instead:

- `SIMCTL_CHILD_PERFECTDROP_QA_MODE=arcade` runs the full functional matrix.
- `arcade-ui`, `stack`, `city`, and `soak` select the corresponding narrower suites.
- `SIMCTL_CHILD_PERFECTDROP_QA_BLOCKED_BUTTON=1` inserts a transparent input blocker for the negative UI-raycast test. Require `FAIL` and `failure.png`; never count this intentionally failing run as product failure or a pass.

Launch with `xcrun simctl launch --terminate-running-process <device> com.kamilunavo.perfectdrop`. Obtain the current data container with `simctl get_app_container ... data` after each install, because reinstalling can change its UUID. Results normally live in its `Documents/qa-<mode>-<unique-run-id>` directory, with `-blocked` before the ID for the negative probe. Every run starts with `RUNNING`; accept only a fresh completed result. Redirect stdout/stderr to separate evidence files.

QA profiles use separate save keys. On mobile, desktop resize requests are skipped; pose-specific captures are named `native-current-pose`, with actual framebuffer/safe-area metadata. Functional suites advance deterministic simulation timing, invoke actual game UI callbacks, and verify that visible enabled button centers hit the expected button through the actual graphic raycaster. They do not synthesize or verify OS touch delivery and do not measure native performance. Actual simulator captures are rendering evidence; screenshots are not evidence that a finger controlled the game.

Unity prefixes the app's persistent directory when saving mobile screenshots, so the QA helper supplies a relative screenshot path while writing result files to the full data path.

The desktop real-time render soak is separate. It renders retained 64-layer endless, a fully earned city, portrait/landscape, a chapter map, and a synthetic division. It requires at least 95% foreground frames. Invalidate a run interrupted by export/editor UI and repeat it; never reinterpret a missing/zero batch counter as zero draw calls. Desktop timings and synthetic regions do not establish native GPU performance or official Duo pose acceptance.

## Final5 OS-input/extra-screen limitations (2026-10-08)

An additional native CUA click at the visible Level1 centre `(170,323)` in the 600×800 desktop player arrived as Unity GUI `(1706,254)` / legacy input `(1706,546)` with `hit=none`, despite focus. Native mouse down/up arrived, but did not hit the intended content. This bounded automation attempt is not a passed OS-input test and did not justify modifying the shared mobile input logic. Evidence: `work/concept-build5-input-observation.log` and its status receipt.

Official simctl `io screenConfig` powered display1 off and then restored it on. Display3 (2007×2853) still captured black, and launch/container requests stalled until simulator shutdown. This is not a genuine opened/divided/rotation pose pass. Do not substitute synthetic desktop division, inactive second-display pixels, or callback invocation for that acceptance.


## 2026-10-11 — human-accepted final polish, candidate 1.0(11)

Latest human feedback accepts the shown direction as almost finished and explicitly authorizes the best remaining polish followed by TestFlight. Fresh direct review of actual Unity images passes for this internal candidate, under that accepted direction: brighter champagne/gold reflections, actual coin medallions, dark readable feedback backing, corrected cloud-plane/aspect sampling, and settings labels correctly converted to logical points. Settings retain49-point buttons in portrait, paired landscape and compact three-column layouts. Fine architectural ornament remains simpler than the illustration; no literal pixel identity or physical-performance claim.

Frozen source `abf63890abb36028b825c08883fb0c4631f5fff59328bb1e02ed0c96a3c54cb5`. Fresh build exit0; navigation8934 including setting-font completeness/density; four serial Mac modes PASS including all8 real finishes/save/reload and66-floor landscape landing size. Immutable evidence: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/FinalPolishReviewed-abf63890abb3`. Actual unaltered comparison: `/Volumes/SSK SSD/Kamilunavo/Artifacts/GraphicsComparison-20261011/perfect-drop-vergleich-final11.png`. Previous failed review remains preserved in that evidence directory; this new exact-source review supersedes it for the explicitly accepted internal direction.

New native build/export receipt binds every exported file to the reviewed Unity source. Upload refuses stale exports or existing archives, uses CocoaPods workspace when present, and preserves build11. Native IL2CPP/Metal, archive/signature and actual Apple processing results must be added below before claiming delivery. Only Perfect Drop was changed. The historical user-deleted Concept file remains unstaged.

Remove Ads `com.kamilunavo.perfectdrop.removeads` now exists in App Store Connect (AppleID6821711775), Germany base EUR1.99, all175 regions and saved German/English localization. Optional rewarded videos remain independent. Internal builds keep official test advertising; real sandbox purchase/restore/refund, real provider consent/impression and physical touch/FPS/music mix acceptance remain open. No public App Review or production ad activation.


### Candidate11 native evidence

Fresh Xcode simulator build succeeded from the exact source export. Actual iPhone17Pro/iOS26.5 IL2CPP/Metal/UIKit concept/styles/presentation/arcade modes allPASS:21 ordered placements, all8 finishes/shader/reload, real portrait1206×2622 and landscape2622×1206 framebuffers/safe areas, parent navigation and66-floor endless. Root directly inspected native menu/day gameplay/gallery, readable Settings at both orientations and8/66-floor landing scale. Evidence `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/NativeFinal11/native-qa-receipts.json`; native comparison `/Volumes/SSK SSD/Kamilunavo/Artifacts/GraphicsComparison-20261011/perfect-drop-vergleich-ios11.png`.

Ordinary native startup was alive25seconds with StoreKit2 initialization and an actual OS screenshot. Simulator screenshot service cannot write directly to the external volume; the bounded ordinary-startup capture was repeated through an owned temporary screenshot copied to the SSD. The four passed gameplay modes were preserved and not repeated. ASTC fallback on simulator and optional StoreConnected-event subscription warning are recorded, with no claim of real store transaction success. Physical touch/FPS/thermal and real sandbox purchases/ads remain unverified.
