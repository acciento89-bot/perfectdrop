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
- [x] P00-T01 Lock game name, “Perfect / Good / Miss” vocabulary and score presentation — locked in docs/plans/P00-product-lock.md
- [x] P00-T02 Lock block dimensions, travel axis, initial speed, speed curve and minimum survivable footprint — 10x10 base, 1.2 height, alternating X/Z, 12→22 studs/s, 1.25 minimum footprint
- [x] P00-T03 Lock Perfect tolerance and combo growth so Perfects feel demanding but learnable — 0.35 stud tolerance, combo cap 10, +0.10 multiplier step
- [x] P00-T04 Lock fail condition, revive rules and retry timing — sub-1.25/no-overlap MISS, one revive/run, <=1.0s retry target
- [x] P00-T05 Lock ethical monetization boundaries and cosmetic categories — no score/height purchase; revive, shield, coin boost and cosmetics only
- [x] P00-T06 Write measurable V1 acceptance criteria and release blockers — explicit 10-blocker release contract recorded

## P01 Technical foundation
- [x] P01-T01 Rojo project with strict client/server/shared ownership
- [x] P01-T02 Shared config for difficulty, scoring, economy and monetization
- [x] P01-T03 Remote definitions with schema/rate-limit expectations — DropRequest/RetryRun/RequestState/RoundStateChanged with server authority contract
- [x] P01-T04 Selene/StyLua/Luau tests and deterministic build command — local verification: 0 errors, 0 warnings, 4 pure test modules, release-readiness pass, Rojo build pass
- [x] P01-T05 GitHub CI for lint, tests, build and release-readiness rules
- [ ] P01-T06 Development/production place documentation and one canonical build artifact

## P02 Spawn, character, camera and input
- [~] P02-T01 Safe spawn beside/on the tower with no invisible fall on join — dedicated operator pad/SpawnLocation and character placement implemented; Studio runtime acceptance pending
- [~] P02-T02 Third-person camera keeps avatar, active block and landing footprint visible — adaptive avatar+tower Scriptable camera implemented; runtime acceptance pending
- [~] P02-T03 Camera follows horizontal/vertical growth without detaching or clipping into the tower — height-aware follow logic implemented; runtime acceptance pending
- [~] P02-T04 Unified drop input for touch, mouse/keyboard and controller — HUD button, mouse, Space and ButtonA wired; device/runtime QA pending
- [~] P02-T05 Input debounce prevents duplicate drops and stale input after retry — client 0.16s debounce + busy guard and server 0.12s rate guard implemented; runtime abuse test pending
- [ ] P02-T06 Runtime test: spawn → move camera state → drop → fail → retry → respawn

## P03 Core drop mechanic
- [~] P03-T01 Deterministic moving-block state and server-authoritative drop timestamp/action — server Heartbeat owns motion and evaluates DropRequest; runtime acceptance pending
- [~] P03-T02 Correct overlap calculation on both travel directions — pure left/right overlap tests pass
- [~] P03-T03 Overhang is visibly cut away and discarded with clean collision — falling overhang geometry implemented; visual runtime acceptance pending
- [~] P03-T04 New top surface exactly matches accepted overlap — accepted block resize/recenter implemented from authoritative overlap result; runtime acceptance pending
- [~] P03-T05 Perfect drop snaps cleanly without cumulative floating-point drift — pure rule preserves previous center/footprint; long-run runtime drift test pending
- [~] P03-T06 Complete miss triggers failure once, never duplicate rewards/failures — state transition and request guards implemented; runtime replay test pending
- [~] P03-T07 Minimum-footprint and extreme-overlap edge cases covered by tests — minimum footprint, left/right, miss and NaN cases covered; broader fuzz/boundary suite still planned
- [ ] P03-T08 Runtime acceptance: at least 30 consecutive mixed drops with no state corruption

## P04 Score, combo and game feel
- [~] P04-T01 Height/score model with server authority — score/height mutated only by RoundService; persistence/abuse acceptance still open
- [~] P04-T02 Perfect combo multiplier with defined cap/decay — cap 10; Perfect increments, Good resets; pure tests pass
- [~] P04-T03 Immediate Perfect / Good / Miss feedback readable without color alone — textual grade feedback implemented; visual runtime QA pending
- [ ] P04-T04 Personal best update is atomic and replay-safe
- [~] P04-T05 Score animation never blocks input or retry — presentation tween is non-blocking; runtime retry timing still pending
- [ ] P04-T06 Runtime verification of combo build, combo break, PB and failure result

## P05 Difficulty and run generation
- [ ] P05-T01 Speed progression curve by tower height
- [ ] P05-T02 Travel distance/axis variation without impossible states
- [ ] P05-T03 Optional visual/environment variation never changes collision truth
- [ ] P05-T04 Deterministic seed support for QA
- [ ] P05-T05 500+ simulated/generated drops remain within configured bounds
- [ ] P05-T06 Long-run tower stability: no precision drift, unreachable camera or runaway part count

## P06 Progression and persistence
- [ ] P06-T01 Coin earning tied to legitimate run performance
- [ ] P06-T02 Cosmetic block/theme/drop-effect catalog
- [ ] P06-T03 Server-side purchase/equip validation
- [ ] P06-T04 Versioned profile schema and migration path
- [ ] P06-T05 Autosave/leave save and lock/recovery rules
- [ ] P06-T06 Real rejoin test preserves PB, coins, ownership and equipped cosmetics

## P07 First-session UX and retention
- [ ] P07-T01 First-time tutorial teaches one action in under 10 seconds
- [ ] P07-T02 Tutorial disappears after understanding and does not obstruct play
- [ ] P07-T03 Daily reward with duplicate-safe claim
- [ ] P07-T04 Daily challenge based on legitimate drop/combo goals
- [ ] P07-T05 Achievement hooks for first Perfect, combo milestones and height milestones
- [ ] P07-T06 New-PB celebration encourages retry without delaying it

## P08 Monetization
- [ ] P08-T01 Final product/pass catalog and price configuration
- [ ] P08-T02 Revive restores a valid tower state, not an exploitably larger footprint
- [ ] P08-T03 Perfect shield/boost has explicit limits and no leaderboard score purchase
- [ ] P08-T04 Receipt processing is allowlisted, atomic and idempotent
- [ ] P08-T05 Purchase prompts are explicit user actions only
- [ ] P08-T06 Shop shows ownership/price/state clearly
- [ ] P08-T07 Pure duplicate/retry/aborted-purchase tests
- [!] P08-T08 Successful real Developer Product receipt + rejoin verification

## P09 Production UI/UX
- [~] P09-T01 Minimal production HUD: score, combo and height implemented with first-session instruction; coins/PB and responsive acceptance remain open
- [ ] P09-T02 Result/retry flow requires at most one obvious action
- [ ] P09-T03 Shop/cosmetic preview is visually production-ready
- [ ] P09-T04 Compact phone safe-area and touch-target pass
- [ ] P09-T05 Tablet/desktop scaling pass
- [ ] P09-T06 Controller focus/navigation pass
- [ ] P09-T07 Accessibility: contrast, non-color state cues, reduced-motion option

## P10 Production art and environment
- [~] P10-T01 Build a distinct rooftop arena/tower/operator-pad first pass; screenshot-quality art acceptance remains open
- [ ] P10-T02 Blocks have intentional material, edge treatment and Perfect-state readability
- [ ] P10-T03 Background/horizon supports height sensation without visual noise
- [~] P10-T04 Lighting/atmosphere/material first pass implemented; mobile runtime readability acceptance remains open
- [ ] P10-T05 Cosmetic themes visibly change presentation without changing hitboxes
- [ ] P10-T06 Screenshot gate: mechanic is understandable from a normal gameplay screenshot

## P11 Audio and VFX
- [ ] P11-T01 Short drop/impact cue
- [ ] P11-T02 Distinct escalating Perfect-chain cue
- [ ] P11-T03 Clean cut/fall feedback for overhang
- [ ] P11-T04 Fast failure and PB/reward cues
- [ ] P11-T05 Verified owned/Roblox-safe assets only
- [ ] P11-T06 Reduced-motion/audio settings verified at runtime

## P12 Security and persistence hardening
- [ ] P12-T01 Remote inventory and rate-limit audit
- [ ] P12-T02 Server recomputes valid drop outcome; client cannot submit score/overlap
- [ ] P12-T03 NaN/infinite/extreme-value guards
- [ ] P12-T04 Currency/cosmetic mutation serialization
- [ ] P12-T05 DataStore migration, stale-lock and recovery test
- [ ] P12-T06 Structured diagnostic logging without noisy production spam

## P13 Mandatory full runtime journey
- [ ] P13-T01 Fresh player: spawn → tutorial → first drop → Perfect → miss → retry
- [ ] P13-T02 Complete a representative run and receive legitimate rewards
- [ ] P13-T03 Buy/equip a cosmetic and confirm presentation
- [ ] P13-T04 Fail/revive/retry paths
- [ ] P13-T05 Respawn camera/input recovery
- [ ] P13-T06 New-session rejoin verifies persisted PB/currency/ownership/equipment
- [ ] P13-T07 20-minute repeated-run stability test
- [ ] P13-T08 Runtime logs contain no gameplay-breaking errors

## P14 Device, performance and control QA
- [ ] P14-T01 Compact phone touch
- [ ] P14-T02 Tablet touch
- [ ] P14-T03 Desktop keyboard/mouse
- [ ] P14-T04 Controller
- [ ] P14-T05 Character/block readability at each viewport
- [ ] P14-T06 Stable performance and bounded tower cleanup/part count

## P15 Release
- [ ] P15-T01 Store icon, thumbnails and metadata match actual production art
- [ ] P15-T02 Content questionnaire/privacy declarations
- [ ] P15-T03 Publish canonical build to private development place
- [ ] P15-T04 Repeat full P13 journey in the published private place
- [ ] P15-T05 Record build hash, place/version and rollback candidate
- [!] P15-T06 Public release only after paid receipt/rejoin gate and zero known P0/P1 defects

## P16 Post-launch
- [!] P16-T01 First telemetry review requires real players
- [!] P16-T02 Balance changes require player evidence
- [ ] P16-T03 Cosmetic/theme cadence

## Definition of Done

Perfect Drop V1 is done only when the **actual published game** is playable start-to-finish, visually production-ready, understandable without explanation, stable across supported devices, persistent across rejoin and free of known P0/P1 gameplay defects. CI alone can never close the release gate.
