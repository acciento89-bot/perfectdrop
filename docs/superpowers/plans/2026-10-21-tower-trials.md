# Tower Trials Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL superpowers:executing-plans inline. Root owns all compilers/Unity/native/device/release work; no children or worker execution.

**Goal:**12curated modifier trials with goals,3attempt runs, separate saved medals and explicit replay/resume, preserving old gameplay/economy.
**Architecture:** Pure BCL catalog/state/codec/file repository; narrowly branch existing StackGame to trial state and add contextual uGUI partial. No old save schema, scene, commerce, ad, music or asset overhaul.
**Tech Stack:** Unity6000.6.4f1, C#, Built-in/uGUI2.6, legacy input; root Mono/csc cheap domain harness then real engine validation.
**Spec:** ../specs/2026-10-21-tower-trials-design.md

## Global constraints
- Old checkout/deleted historical concept untouched; isolated branch atd0932b1, no versions/store/signing/main push/merge.
- Domain before production requires root actual missing-file RED, then same harness GREEN before Unity integration.
- Current campaign/Endless/daily/coins/stars/style/monetization/ad pacing/music preserved; new medals separate and no new commerce incentive.
- Trials maximum3attempts per selected-trial run; retries free, no cooldown, old callback tokens never mutate current trial.
- Actual source-bound graphical/icon gates remain pending after feature edits; generated artwork/source review never counts as rendered acceptance.

## Review focus
- Wrong/old run or attempt callback, repeated terminal event, retry button double tap, new run after budget exhaust.
- Failed save after actual geometry update pauses proposed state without publishing a false medal or another drop.
- External corrupt/future trial archive after valid initial load must remain byte-for-byte; old sibling PlayerPrefs/profile data untouched.
- Cold trial resume versus legacy campaign resume; no accidental campaign reward/day/ad eligibility/power activation.
- Modal/Settings/native presentation/resize pause same attempt and preserve exact motion/layers/attempt count.

### Task1 — Actual pure RED harness
Create tests/tower-trials/Program.cs and scripts/run-tower-trials-core-tests.sh; no production .cs yet.
Interfaces TowerTrialCatalog.All/Get; TrialRules.Begin/Retry/Checkpoint/Finish/Abandon/Unlocked/Medal; TrialBook.Empty; TrialCodec.Encode/Decode; TowerTrialsFile.Load/Save(book,expectedRevision). DTO fields exactly spec; integer medal0..3 and explicit attempt states.
- [ ] Assert fixed12catalog/modifier/target/goal thresholds and unlock boundaries.
- [ ] Assert3attempt budget, failed/below-goal terminal, exact idempotent terminal retry, stale/foreign tokens, improved best medals retained and replay independent from old bytes.
- [ ] Assert finite/consistent snapshots, deep immutability and motion checkpoint/cold resume.
- [ ] Assert real codec roundtrip/version/truncation/trailing/size/duplicate records; actual disk atomic save/restart/failure/stale/external replacement/sibling legacy retention.
- [ ] Freeze test files; root command `bash scripts/run-tower-trials-core-tests.sh '<B/PerfectDropUpdate/core-red.exe>'`; expected compiler missing TowerTrials.cs/TowerTrialsArchive.cs, not environment error.

### Task2 — Pure domain/actual archive after observed RED
Create Assets/Scripts/Gameplay/TowerTrials.cs + .meta, Assets/Scripts/Core/TowerTrialsArchive.cs + .meta after root RED; no Unity import by worker.
- [ ] Implement catalog, immutable clone mutations and exact token/state rules, fixed3attempt budget and independent best medal records.
- [ ] Implement bounded explicit binary codec using BCL, strict IDs/EOF/schema/numerics/counts; atomic file store rereads existing bytes and expected revision under lock.
- [ ] Request same harness root GREEN, freeze inputs while command runs. No integration before result.

### Task3 — Narrow real Unity integration after pure GREEN
Files planned StackGame.cs (only necessary partial/branch seam), new TowerTrialGame.cs collaborator/partial, StackHud.cs/CampaignHud.cs contextual seams and new TowerTrialsHud.cs. New deterministic Editor/TowerTrialsValidation.cs (with .meta) uses actual StackRun for all4modifier types, snapshot restore and original reward isolation. Reuse accepted existing geometry/assets.
- [ ] Load separate trial archive with visible failure; map/unlock/medal/replay and matching selected goal/attempt state.
- [ ] Actual StackRun drop→trial snapshot and save-before-publish branch; no legacy coins/stars/best/reward/ads mutations; powers/risk disabled only Trials.
- [ ] Pause/focus/quit/checkpoint and explicit cold resume; settings and navigation restore correct context; leave trial restores original legacy run/home.
- [ ] DEEN >=48logical-point adaptive safe-area/division current bounds, scrollable map/result, no stale state keys or cached hinge constants.
- [ ] Root Unity compiler, new focused deterministic validation plus existing aggregate; no worker engine operation.

### Task4 — Native review/readiness handoff
- [ ] Root independent whole-feature source review; real failures fixed only after targeted RED.
- [ ] Root actual ordinary native iPhone/Android/Duo new mode plus old regression, capture exact source/binary/frame/geometry/old-save evidence. No transactions or live-provider success claims.
- [ ] Fresh actual source-bound visual/icon accuracy review via required repo guards before any upload; preserve previous reports as historical.
- [ ] Per-app B/PerfectDropUpdate source/metadata/QA/evidence ledger and handoff; local scoped commit after root verification, no push/version/store by worker.
