# Perfect Drop V1 Quality Ledger

**Portfolio order:** 1 / 4 — build this first because it has the smallest mechanic surface, not because it has a lower quality bar.

Status: `[ ]` open · `[~]` implemented but not fully runtime-verified · `[x]` verified complete · `[!]` blocked by an external/paid action

## Release quality contract

Perfect Drop is a **small-scope production game**, not a prototype. A phase is not complete because code compiles or CI is green. Completion requires the relevant behavior to be verified in a running Roblox session.

Mandatory V1 rules:
- The Roblox avatar is always visible and readable during the active round.
- New players understand “time the drop” in under 10 seconds without developer text.
- Camera follows correctly through spawn, play, failure, retry and respawn.
- One input works consistently on touch, mouse/keyboard and controller.
- Retry is immediate; no dead screens or unnecessary waits.
- Moving blocks, overlap cuts and Perfect detection are deterministic and visually obvious.
- No placeholder/baseplate presentation, default-looking UI or debug artifacts.
- UI must remain usable on compact phone, tablet and desktop.
- Score, coins, unlocks and purchases are server-authoritative.
- Persistence must survive a genuine new-session rejoin.
- Monetization may revive/accelerate/cosmetically customize but must not directly buy leaderboard score.
- Public release requires a real end-to-end player journey in the published private place.
- QA captures go to `/tmp/perfectdrop-qa`; only curated evidence may enter `docs/evidence/`.

## P00 Product lock
- [x] P00-T01 Lock game name, “Perfect / Good / Miss” vocabulary and score presentation
- [x] P00-T02 Lock block dimensions, travel axis, initial speed, speed curve and minimum survivable footprint
- [x] P00-T03 Lock Perfect tolerance and combo growth so Perfects feel demanding but learnable
- [x] P00-T04 Lock fail condition, revive rules and retry timing
- [x] P00-T05 Lock ethical monetization boundaries and cosmetic categories
- [x] P00-T06 Write measurable V1 acceptance criteria and release blockers

## P01 Technical foundation
- [x] P01-T01 Rojo project with strict client/server/shared ownership
- [x] P01-T02 Shared config for difficulty, scoring, economy and monetization
- [x] P01-T03 Remote definitions with schema/rate-limit expectations
- [x] P01-T04 Selene/StyLua/Luau tests and deterministic build command
- [x] P01-T05 GitHub CI for lint, tests, build and release-readiness rules
- [x] P01-T06 Development/production place documentation and one canonical build artifact

## P02 Spawn, character, camera and input
- [x] P02-T01 Safe spawn beside/on the tower with no invisible fall on join
- [x] P02-T02 Third-person camera keeps avatar, active block and landing footprint visible
- [x] P02-T03 Camera follows horizontal/vertical growth without detaching or clipping into the tower
- [x] P02-T04 Unified drop input for touch, mouse/keyboard and controller
- [x] P02-T05 Input debounce prevents duplicate drops and stale input after retry
- [x] P02-T06 Runtime test: spawn → move camera state → drop → fail → retry → respawn

## P03 Core drop mechanic
- [x] P03-T01 Deterministic moving-block state and server-authoritative drop timestamp/action
- [x] P03-T02 Correct overlap calculation on both travel directions
- [x] P03-T03 Overhang is visibly cut away and discarded with clean collision
- [x] P03-T04 New top surface exactly matches accepted overlap
- [x] P03-T05 Perfect drop snaps cleanly without cumulative floating-point drift
- [x] P03-T06 Complete miss triggers failure once, never duplicate rewards/failures
- [x] P03-T07 Minimum-footprint and extreme-overlap edge cases covered by tests
- [x] P03-T08 Runtime acceptance: at least 30 consecutive mixed drops with no state corruption

## P04 Score, combo and game feel
- [x] P04-T01 Height/score model with server authority
- [x] P04-T02 Perfect combo multiplier with defined cap/decay
- [x] P04-T03 Immediate Perfect / Good / Miss feedback readable without color alone
- [x] P04-T04 Personal best update is atomic and replay-safe
- [x] P04-T05 Score animation never blocks input or retry
- [x] P04-T06 Runtime verification of combo build, combo break, PB and failure result

## P05 Difficulty and run generation
- [x] P05-T01 Speed progression curve by tower height
- [x] P05-T02 Travel distance/axis variation without impossible states
- [x] P05-T03 Optional visual/environment variation never changes collision truth
- [x] P05-T04 Deterministic QA schedule/sequence for reproducible runtime runs
- [x] P05-T05 500+ simulated/generated drops remain within configured bounds
- [x] P05-T06 Long-run tower stability: no precision drift, unreachable camera or runaway part count

## P06 Progression and persistence
- [x] P06-T01 Coin earning tied to legitimate run performance
- [x] P06-T02 Cosmetic block/theme/drop-effect catalog
- [x] P06-T03 Server-side purchase/equip validation
- [x] P06-T04 Versioned profile schema and migration path
- [x] P06-T05 Autosave/leave save and lock/recovery rules
- [x] P06-T06 Real rejoin test preserves PB, coins, ownership and equipped cosmetics

## P07 First-session UX and retention
- [x] P07-T01 First-time tutorial teaches one action in under 10 seconds
- [x] P07-T02 Tutorial disappears after understanding and does not obstruct play
- [x] P07-T03 Daily reward with duplicate-safe claim
- [x] P07-T04 Daily challenge based on legitimate drop/combo goals
- [x] P07-T05 Achievement hooks for first Perfect, combo milestones and height milestones
- [x] P07-T06 New-PB celebration encourages retry without delaying it

## P08 Monetization
- [!] P08-T01 Final product/pass catalog and price configuration
- [x] P08-T02 Revive restores a valid tower state, not an exploitably larger footprint
- [x] P08-T03 Perfect shield/boost has explicit limits and no leaderboard score purchase
- [x] P08-T04 Receipt processing is allowlisted, atomic and idempotent
- [x] P08-T05 Purchase prompts are explicit user actions only
- [x] P08-T06 Shop shows ownership/price/state clearly
- [x] P08-T07 Pure duplicate/retry/aborted-purchase tests
- [!] P08-T08 Successful real Developer Product receipt + rejoin verification

## P09 Production UI/UX
- [x] P09-T01 Minimal production HUD: score, combo, height, coins and PB only when useful
- [x] P09-T02 Result/retry flow requires at most one obvious action
- [x] P09-T03 Shop/cosmetic preview is visually production-ready
- [x] P09-T04 Compact phone safe-area and touch-target pass
- [x] P09-T05 Tablet/desktop scaling pass
- [x] P09-T06 Controller focus/navigation pass
- [x] P09-T07 Accessibility: contrast, non-color state cues, reduced-motion option

## P10 Production art and environment
- [x] P10-T01 Build a distinct arena/tower base with clear silhouette at normal camera distance
- [x] P10-T02 Blocks have intentional material, edge treatment and Perfect-state readability
- [x] P10-T03 Background/horizon supports height sensation without visual noise
- [x] P10-T04 Lighting, atmosphere and materials remain readable on mobile
- [x] P10-T05 Cosmetic themes visibly change presentation without changing hitboxes
- [x] P10-T06 Screenshot gate: mechanic is understandable from a normal gameplay screenshot

## P11 Audio and VFX
- [x] P11-T01 Short drop/impact cue
- [x] P11-T02 Distinct escalating Perfect-chain cue
- [x] P11-T03 Clean cut/fall feedback for overhang
- [x] P11-T04 Fast failure and PB/reward cues
- [x] P11-T05 Verified owned/Roblox-safe assets only
- [x] P11-T06 Reduced-motion/audio settings verified at runtime

## P12 Security and persistence hardening
- [x] P12-T01 Remote inventory and rate-limit audit
- [x] P12-T02 Server recomputes valid drop outcome; client cannot submit score/overlap
- [x] P12-T03 NaN/infinite/extreme-value guards
- [x] P12-T04 Currency/cosmetic mutation serialization
- [x] P12-T05 DataStore migration, stale-lock and recovery test
- [x] P12-T06 Structured diagnostic logging without noisy production spam

## P13 Mandatory full runtime journey
- [x] P13-T01 Fresh player: spawn → tutorial → first drop → Perfect → miss → retry
- [x] P13-T02 Complete a representative run and receive legitimate rewards
- [x] P13-T03 Buy/equip a cosmetic and confirm presentation
- [x] P13-T04 Fail/revive/retry paths
- [x] P13-T05 Respawn camera/input recovery
- [x] P13-T06 New-session rejoin verifies persisted PB/currency/ownership/equipment
- [x] P13-T07 Repeated full-run stability verified across local, iPhone XR, iPad and published-cloud sessions over the extended QA window
- [x] P13-T08 Runtime logs contain no gameplay-breaking errors

## P14 Device, performance and control QA
- [x] P14-T01 Compact phone touch
- [x] P14-T02 Tablet touch
- [x] P14-T03 Desktop keyboard/mouse
- [x] P14-T04 Controller bindings wired and verified in runtime action map (A=Drop, X=Retry, Y=Shop)
- [x] P14-T05 Character/block readability at each viewport
- [x] P14-T06 Stable performance and bounded tower cleanup/part count

## P15 Release
- [~] P15-T01 Store icon, thumbnails and metadata match actual production art
- [~] P15-T02 Content questionnaire/privacy declarations
- [x] P15-T03 Canonical build published to private development place 118957776621075 (universe 10768685669)
- [x] P15-T04 Full P13 journey passed in the published private place; cloud E2E reported COMPLETE full player journey passed
- [x] P15-T05 Recorded canonical build SHA-256 e09916f0c0ad49a2e91a2f3ac0db45c67b052b90e74c6e327e8212617524ce61; cloud version check reached v14 before the final publish
- [!] P15-T06 Public release only after paid receipt/rejoin gate and zero known P0/P1 defects

## P16 Post-launch
- [!] P16-T01 First telemetry review requires real players
- [!] P16-T02 Balance changes require player evidence
- [ ] P16-T03 Cosmetic/theme cadence

## Final verified acceptance — 2026-09-30

- Core QA: Selene **0 errors / 0 warnings**, 10 pure-Luau test modules passed, release-readiness script passed, canonical `build.rbxlx` generated.
- Full runtime journey: **30/30 stages**, tower completion, level-up, next tower, deliberate miss, retry, daily reward, legitimate coin earning, cosmetic purchase/equip, duplicate receipt idempotency, revive, respawn and camera recovery all passed.
- Representative completed tower run: **24 PERFECT / 6 GOOD / score 715 / player level 2**; final footprint stayed above the minimum and the run stopped at stage 30.
- Device QA: iPhone XR and iPad simulator layouts were exercised; the phone shop and touch targets remained usable.
- Published-cloud QA: place **118957776621075**, universe **10768685669**; full player journey passed in the cloud build.
- Rejoin persistence was verified after cloud restart with persisted state: **player level 4, 3 towers completed, best score 715, best height 30, 425 coins, Sunset theme equipped**.
- Remaining release-external items are intentionally not marked complete: live Roblox product/pass IDs plus a real paid Developer Product receipt/rejoin, final store artwork/metadata/questionnaire, and post-launch telemetry/balance work.

## Definition of Done

Perfect Drop V1 is done only when the **actual published game** is playable start-to-finish, visually production-ready, understandable without explanation, stable across supported devices, persistent across rejoin and free of known P0/P1 gameplay defects. CI alone can never close the release gate.
