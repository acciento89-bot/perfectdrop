# Perfect Drop: replay and menu correction, 14 October 2026

Scope authorised by the human: correct the reported 30-level replay/star, framed menu background, wallet visibility and hidden Endless problems; validate the real Unity/iOS build; resubmit with complete purchases/ads/store screenshots; then proceed directly to Repair Empire. The accepted tower/art direction remains controlling. No numerical likeness claim.

## Baseline and cause

- Canonical SSD main begins with only the existing user-owned deletion of `docs/concepts/PRIMARY-CONCEPT.png`. Preserve it unstaged.
- Apple confirms version 1.0 was DEVELOPER_REJECTED and its former four-item submission COMPLETE, matching the human withdrawal.
- Actual chapter callbacks only changed `_chapter`/preview. The main action still used `Profile.UnlockedLevel`, so selecting an earlier world followed by Play launched the frontier, level 30 on the reported completed save. The actual ten level-button callbacks and `StackGame.StartLevel(id)` already routed requested IDs correctly; this was an undiscoverable/misleading entry flow.
- Layout explicitly hid Endless outside the campaign subpage.
- The menu illustration filled the safe-area pane while the game scene remained visible above/below it. A full-window background must be separate from safe-area controls.
- Existing settings already hide the active tower/Drop and use a full opaque canvas backing; retain and revalidate this behaviour.

A new real Unity UI regression fixture, before product edits, fails with `ENDLESS_HIDDEN`, `HOME_INSET` and `WORLD_PREVIEW_ONLY` for all three chapters. Baseline log is retained in SSD Builds/DropReplay13-20261014/baseline-regression.log.

## Correction

- Tapping a world now opens that world's ten individually selectable level cards directly. Existing unlock guards, requested global level IDs and best-star recording remain authoritative.
- Endless is visible on home, with best record or its explicit level-5 unlock condition; full-size and compact landscape targets retain their minimum touch size.
- The existing original 3D scene render is drawn once across the full Canvas. Its old inset render is disabled. Wallet/buttons remain in the safe area; level selection uses an opaque full-window navy backing. No replacement mockup or extra scene is introduced.
- Three IAPs are prepared in the new Apple review draft. DE/EN store copy explicitly mentions replaying levels and the direct Endless entry. English name uses the existing publisher qualifier because the unqualified English app name belongs to another account.

## Validation contract

- Full Editor aggregate, including meaningful real UI/replay regression, native shaders/geometry, ≥49-point layout/contrast and existing commerce/ad pacing/recovery suites.
- Actual native iOS Simulator: each of the 30 cards from a completed save, DE/EN and portrait/landscape (120 launches per replay run), requested world/target identity, real earlier-level star improvement without altering other records, save/reload, direct Endless/Shop, opaque Settings and full Canvas edges.
- Compact iPhone, large iPhone and iPad frames, plus current selected tower styles and live three-world gameplay. Inspect unaltered native frames against the unchanged approved board.
- New public eligible build 1.0(13), current screenshots, three IAP items and persisted WAITING_FOR_REVIEW before claiming submission. No authenticated paid transaction, live ad impression or physical-device performance claim without corresponding evidence.

## Final source and additional reproduced defects

Frozen candidate: `7af9cf5709efc2703e104a0b9e689a8108a0d40fee303ea5ae2b201011eca8d6`, iOS 1.0(13). Native export succeeded after the complete Editor aggregate; navigation alone covers 9,462 checks. The earlier red fixture remains archived.

Native visual review revealed a safe-area footer strip and the same inset background in the design gallery. Separate real UI tests failed for both before correction. The ivory home footer now reaches the real window edge; the gallery and Shop share the full Canvas background. The small-phone wordmark fits between controls.

The home plaque also always described the frontier rather than the active saved run. A regression reproduced `RESUME_FRONTIER_MIXUP` before correction. It now describes the actual resumable level/mode, and Settings preserves the browsed chapter on return. Actual play/save authority is unchanged.

Current native Simulator results: large iPhone, compact iPhone and iPad each passed 120 actual chapter/level button launches (DE/EN × both orientations), independent Level 1 improvement from one to three stars, intact other 29 records, save/scene reload, direct Endless/wallet and full-window Settings. Current style tests verify all eight actual shader/material variants and reloading. Arcade tests pass a real 66-layer endless run and existing powers/risk/daily/city flows. Current DE/EN phone/iPad Concept capture sessions use actual ordered UI drops; their raw native frames supply the store images.

The native probe initially attempted to raycast newly activated uGUI before end-of-frame culling updated. That historical probe failure is retained; waiting one render frame resolved the test harness timing, without disabling raycast visibility assertions or changing product behavior.

A separate read-only code reviewer approved the complete final diff and source binding, with no concrete defect. Direct native visual inspection against the unchanged approved board passes the exact-source release gate. It does not claim new human acceptance, literal pixel identity, a numerical similarity score, physical-device FPS, a live paid transaction or a live ad impression.

Store submission and ordinary cold-start proof are recorded separately in the final QA receipt and canonical ledger. No submission is claimed before Apple's persisted status confirms it.
