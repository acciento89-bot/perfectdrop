# Optional Monetization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Real optional native-store purchases and rewarded ads, preserving the free game and correcting landscape reachability.

**Architecture:** StackProfile holds atomic fulfillment alongside wallet. Pure commerce rules enforce replay/idempotence; a Store service and independent Rewarded service own SDK lifecycles. StackHud contextual shop owns only presentation.

**Tech Stack:** Unity6000.6.4f1, C#, UnityIAP5.4.4, GoogleMobileAds11.5.0 candidate, StoreKit2/GoogleBilling, UGUI.

**Spec:** docs/superpowers/specs/2026-10-08-monetization.md

## Global Constraints
- No public publication; main canonical and internal TestFlight authorized.
- Existing schema2/gameplay/coin styles preserved; new fields additive.
- Known non-consumable IDs and styles4–7 exactly as spec.
- Starter500 once on pending original transaction, confirmed restoration only style.
- Reward50, max5/UTCday, cooldown60seconds, only genuine completed reward callback.
- Store localized price; no release fake fulfillment; minimum48-point controls.

## Review Focus
- Interrupted/replayed fulfillment persists wallet and dedup marker together before ack.
- Restore/reinstall never regrants confirmed starter coins.
- Unknown/cancelled/deferred products cannot mint coins or cosmetics.
- Native ad overlay lifecycle cannot drop a block or lose progress on resume.
- Unavailable account/catalog/consent/no-fill must leave playable offline game, no reward.

### Task1: landscape correction
- [x] Add actual Drop-vs-powers bounds check and reproduce Risk overlap FAIL in old player.
- [x] Move landscape Drop and powers to disjoint comfortable zones; full30-drop/save/reload PASS.
- [x] Test956x440 capture; inspect, record user physical report; full30-drop QA PASS. Commit coherent fix.

### Task2: purchase fulfillment + store adapter
Files: Assets/Scripts/Monetization/CommerceRules.cs, StorePurchases.cs; Core/StackSave.cs; Editor/CommerceValidation.cs; Packages/manifest.json.
Interfaces: CommerceRules.ApplyPending(profile,productId,transactionId), RestoreEntitlement(profile,productId), returning grant/change outcome; StorePurchases exposes fetched products/prices/busy/status plus Buy and Restore.
- [ ] Write failing pure replay/restore/unknown/migration/persistence-boundary checks.
- [ ] Install pinned package, implement rules/store SDK lifecycle and run checks.
- [ ] Compile editor + native iOS/Android; inspect SDK events/receipt boundaries against installed APIs.
- [ ] Commit verified purchase subsystem and ledger actual pending live-store configuration.

### Task3: premium shop and cosmetics
Files: UI/CommerceHud.cs, CampaignHud.cs, StackHud.cs; Gameplay/StackGame.cs; Visuals/WorldArt.cs; QA/RuntimeSmoke.cs.
- [ ] Add tests for premium entitlement selection and old four styles unchanged.
- [ ] Add styles4–7 and contextual shop with store prices/owned/offline/restore states.
- [ ] Run full arcade and shop raycast/layout/save regression portrait/landscape/division.
- [ ] Commit verified UI without fake release transactions.

### Task4: rewarded SDK + consent
Files: Monetization/RewardedVideos.cs, MonetizationConfig.cs; Editor/build plugin config; UI/CommerceHud.cs; Editor/CommerceValidation.cs.
- [ ] Write fail cases no-fill/cancel/duplicate/cap/cooldown/UTC rollover.
- [ ] Implement adapter, consent/privacy options and exactly-once grants, overlay pause/resume.
- [ ] Configure actual account IDs if available, official test-only units for internal test.
- [ ] Validate native dependencies, completed test ad and cancellation on real device; record remaining credential/account blockers accurately.

### Task5: final integration/release candidate
- [ ] Fresh whole-change read-only review, fix concrete findings with regression checks.
- [ ] Native simulator/layout compile, signed Android with existing central key, signed iOS archive/internal TestFlight next number.
- [ ] Actual StoreKit sandbox buy/cancel/restore and rewarded completion/no-fill. No live earnings claim without real catalog/account configuration.
- [ ] Commit/push clean source, update ledger and actionable device/store handoff.
