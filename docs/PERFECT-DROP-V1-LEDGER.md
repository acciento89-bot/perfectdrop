# Perfect Drop V1 Ledger

Status: `[ ]` open · `[~]` implemented/partially verified · `[x]` verified complete · `[!]` externally/runtime blocked

## P00 Product Definition
- [ ] P00-T01 Lock game identity, terminology and score vocabulary
- [ ] P00-T02 Lock core gameplay constants and difficulty boundaries
- [ ] P00-T03 Lock monetization boundaries and no-pay-to-win rules

## P01 Technical Foundation
- [ ] P01-T01 Rojo project/source layout
- [ ] P01-T02 Shared config/remotes/state modules
- [ ] P01-T03 Formatting/lint/test toolchain
- [ ] P01-T04 CI and release-readiness checks
- [ ] P01-T05 Dev/prod place configuration documentation

## P02 Core Character Loop
- [ ] P02-T01 Character spawn and safe arena
- [ ] P02-T02 Camera keeps avatar and next objective readable
- [ ] P02-T03 Touch/mouse/controller input abstraction
- [ ] P02-T04 Round state machine and instant restart
- [ ] P02-T05 Runtime core-loop verification

## P03 Primary Mechanic
- [ ] P03-T01 Mechanic rules/model
- [ ] P03-T02 Server-authoritative round validation
- [ ] P03-T03 Player-facing mechanic implementation
- [ ] P03-T04 Failure/recovery edge cases
- [ ] P03-T05 Representative runtime acceptance

## P04 Scoring & Combo
- [ ] P04-T01 Score and landing/action grades
- [ ] P04-T02 Combo/multiplier rules
- [ ] P04-T03 Personal best persistence
- [ ] P04-T04 Anti-replay/anti-score-spoof validation
- [ ] P04-T05 Feedback hierarchy runtime check

## P05 Procedural Challenge Generation
- [ ] P05-T01 Deterministic generator
- [ ] P05-T02 Difficulty curve
- [ ] P05-T03 Safe reachability/bounds rules
- [ ] P05-T04 Variety guardrails
- [ ] P05-T05 Long-run generation test

## P06 Progression
- [ ] P06-T01 Currency model
- [ ] P06-T02 Cosmetic unlock catalog
- [ ] P06-T03 Server purchase/equip rules
- [ ] P06-T04 Profile persistence/migration
- [ ] P06-T05 Progression balance smoke test

## P07 Retention
- [ ] P07-T01 Daily login reward
- [ ] P07-T02 Daily challenge
- [ ] P07-T03 Achievement hooks
- [ ] P07-T04 Best-score celebration/return loop

## P08 Monetization
- [ ] P08-T01 Product/pass catalog and pricing config
- [ ] P08-T02 Revive/boost rules
- [ ] P08-T03 Receipt idempotency
- [ ] P08-T04 Entitlement UI
- [ ] P08-T05 Pure purchase tests
- [!] P08-T06 Real Marketplace receipt/rejoin verification

## P09 UI/UX
- [ ] P09-T01 Production HUD
- [ ] P09-T02 Retry/result flow
- [ ] P09-T03 Shop/cosmetic presentation
- [ ] P09-T04 Compact-phone layout
- [ ] P09-T05 Tablet/desktop layout
- [ ] P09-T06 Controller navigation/accessibility

## P10 Production Art
- [ ] P10-T01 Environment/arena art kit
- [ ] P10-T02 Gameplay-object production art
- [ ] P10-T03 Character/avatar readability pass
- [ ] P10-T04 Lighting/material pass
- [ ] P10-T05 Screenshot-quality acceptance

## P11 Audio & VFX
- [ ] P11-T01 Input/action feedback
- [ ] P11-T02 Perfect/combo escalation
- [ ] P11-T03 Failure/retry feedback
- [ ] P11-T04 Reward/unlock feedback
- [ ] P11-T05 Reduced-motion/audio runtime QA

## P12 Security & Persistence
- [ ] P12-T01 Remote/rate-limit audit
- [ ] P12-T02 Score/currency authority audit
- [ ] P12-T03 DataStore migration/recovery
- [ ] P12-T04 Receipt/purchase abuse audit
- [ ] P12-T05 Structured diagnostics

## P13 Runtime QA
- [ ] P13-T01 Full first-session journey
- [ ] P13-T02 Repeated retry/long-session stability
- [ ] P13-T03 Collision/reachability/edge cases
- [ ] P13-T04 Performance baseline
- [ ] P13-T05 Reconnect/persistence

## P14 Device & Input QA
- [ ] P14-T01 Compact phone
- [ ] P14-T02 Tablet
- [ ] P14-T03 Desktop
- [ ] P14-T04 Touch
- [ ] P14-T05 Keyboard/mouse
- [ ] P14-T06 Controller

## P15 Release
- [ ] P15-T01 Store icon/thumbnails/metadata
- [ ] P15-T02 Content questionnaire/privacy declarations
- [ ] P15-T03 Private Roblox publish
- [ ] P15-T04 Rollback/release candidate record
- [!] P15-T05 Controlled public launch after paid receipt gate

## P16 Live Operations
- [!] P16-T01 First telemetry review requires real players
- [!] P16-T02 Evidence-based balance patch requires live evidence
- [ ] P16-T03 Cosmetic/content cadence
