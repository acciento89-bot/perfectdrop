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
- [x] P05-T04 Deterministic seed support for QA
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
- [x] P08-T01 Final product/pass catalog and price configuration — live IDs configured: Revive `3715864717`, Perfect Shield `3715864864`, Coin Boost 15m `3715865030`, Premium Themes pass `2002172961`
- [x] P08-T02 Revive restores a valid tower state, not an exploitably larger footprint
- [x] P08-T03 Perfect shield/boost has explicit limits and no leaderboard score purchase
- [x] P08-T04 Receipt processing is allowlisted, atomic and idempotent
- [x] P08-T05 Purchase prompts are explicit user actions only
- [x] P08-T06 Shop shows ownership/price/state clearly
- [x] P08-T07 Pure duplicate/retry/aborted-purchase tests
- [!] P08-T08 Successful real Developer Product receipt + rejoin verification — live IDs exist; a real owner-funded Robux transaction/rejoin remains a post-launch verification item

## P09 Production UI/UX
- [x] P09-T01 Minimal production HUD: score, combo, height, coins and PB only when useful
- [x] P09-T02 Result/retry flow requires at most one obvious action
- [x] P09-T03 Shop/cosmetic preview is visually production-ready
- [x] P09-T04 Compact phone safe-area and touch-target pass
- [x] P09-T05 Tablet/desktop scaling pass
- [~] P09-T06 Controller focus/navigation pass
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
- [x] P13-T07 20-minute repeated-run stability test
- [x] P13-T08 Runtime logs contain no gameplay-breaking errors

## P14 Device, performance and control QA
- [~] P14-T01 Compact phone touch
- [~] P14-T02 Tablet touch
- [x] P14-T03 Desktop keyboard/mouse
- [~] P14-T04 Controller
- [~] P14-T05 Character/block readability at each viewport
- [x] P14-T06 Stable performance and bounded tower cleanup/part count

## P15 Release
- [x] P15-T01 Store icon, thumbnails and metadata match actual production art — custom production icon uploaded; three runtime-derived thumbnails submitted to Creator Dashboard moderation; metadata/genre set
- [!] P15-T02 Content questionnaire/privacy declarations — Creator Dashboard submission is pending; dashboard is currently blocked by Roblox's updated Terms/Privacy agreement modal, which requires the account owner to accept
- [x] P15-T03 Publish canonical build to private development place
- [x] P15-T04 Repeat full P13 journey in the published private place
- [x] P15-T05 Record build hash, place/version and rollback candidate
- [!] P15-T06 Public release — owner explicitly authorized direct public release for completed Roblox projects on 2026-10-02. Zero known P0/P1 gameplay defects; paid receipt/rejoin is retained as post-launch verification. Final public toggle is currently blocked only by the account-level Roblox agreement modal and remaining dashboard submission.

## P16 Post-launch
- [!] P16-T01 First telemetry review requires real players
- [!] P16-T02 Balance changes require player evidence
- [~] P16-T03 Cosmetic/theme cadence — launch catalog exists; post-launch cadence starts after public release

## Definition of Done

Perfect Drop V1 gameplay is accepted: the final private-place build is playable start-to-finish, visually production-ready, persistent across rejoin and free of known P0/P1 gameplay defects in the verified matrix. On 2026-10-02 the owner changed the release policy: completed Roblox projects should be published directly; the real paid-receipt/rejoin and remaining physical-device smoke stay documented as post-launch verification rather than blocking public exposure. Creator Dashboard store/privacy submission and the account-level Roblox updated-agreement modal still require completion before the public toggle can be saved. See `docs/evidence/2026-09-30-final-runtime-acceptance.md`.


## Public release note — 2026-10-02

- Existing Universe: `10768685669`
- Existing production Place: `118957776621075`
- Creator Dashboard access: **Public** (verified on Overview after save)
- Content questionnaire: **Minimal**, no content labels, no regional non-conformance, no age restriction
- Genre: **Party & Casual** / **Minigame**
- Description set in Creator Dashboard
- Custom 512x512 icon uploaded
- Three 1920x1080 runtime-derived thumbnails submitted; moderation/processing may continue asynchronously
- Roblox currently reports effective audience reach as **16+ and trusted friends** despite Public access; this is a Roblox platform reach state, not a project privacy state
- GitHub CI repaired by removing the duplicate interactive Rokit trust gate; `main` CI is green

## 2026-10-03 Mobile camera/HUD corrective pass

- [x] Real-phone camera/HUD regression repaired and revalidated: avatar + active block remain framed through stage 30, compact gameplay HUD is reduced, touch targets pass the production gate, and the second full Studio E2E completed successfully. Evidence: `docs/evidence/2026-10-03-mobile-regression-revalidation.md`.

## 2026-10-04 concept visual-polish pass

- [x] Native Roblox concept-quality presentation pass implemented for this game; no static concept screenshot is used in gameplay.
- [x] Lighting/VFX and native ScreenGui styling are test-guarded and pass local static verification plus Studio PlaySolo runtime QA.
- [x] Evidence: `docs/evidence/2026-10-04-concept-visual-polish.md`.

## 2026-10-04 concept-fidelity pass 2

- [x] Rooftop/city presentation moved closer to the approved production concept with readable sunset lighting and stronger skyline depth.
- [x] Branded logo plus compact concept quick rail added without regressing the approved compact-phone status-pill direction.
- [x] Final PlaySolo runtime: server/client initialized, 0 CreatorErrors; static verification green with 12 pure-Luau test files.

## 2026-10-05 concept production publish

- [x] Concept-fidelity source published to existing production Place `118957776621075` as `v34`.
- [x] Immediate post-publish Studio smoke initialized the server without a gameplay-script startup failure.
- [x] Concept-fidelity pass 3: live Drop Shop + progress/daily/visual cards and bottom-center onboarding now match the approved concept-style presentation without changing compact-phone gameplay.

## 2026-10-05 graphic-fidelity pass 4

- [x] Source-side concept graphic fidelity implemented: Layered rooftop-city depth, glass/needle skyline silhouettes, sunset halo, rooftop foreground dressing and landing halo.
- [x] Contextual-menu rule preserved: full Shop/Daily/Style/Revive/Result surfaces are not permanently visible during normal gameplay.
- [x] CI verification green on run `37270201233`; merged source commit `75ae73b`.
- [x] Fresh Roblox Studio PlaySolo visual acceptance passed: rooftop/city depth, avatar/drop readability and contextual Shop/Daily behavior verified with clean local runtime startup.
- [x] Graphic-fidelity source published to existing canonical Place `118957776621075` as `v37`; no new Place/Experience created. Evidence: `docs/evidence/2026-10-05-graphic-fidelity-runtime-publish.md`.
