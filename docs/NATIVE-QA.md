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
