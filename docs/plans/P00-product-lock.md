# P00 — Perfect Drop Product Lock

## Identity

- Product name: **Perfect Drop**
- Core promise: one-button precision stacking with the visible Roblox avatar as the visual anchor.
- Primary run metrics: **Score**, **Stage 0/30**, **Perfect Combo**, **Player Level**, **Best**.
- Landing grades: **PERFECT**, **GOOD**, **MISS**.

## Locked V1 gameplay constants

| Constant | V1 value |
| --- | ---: |
| Base block footprint | 10 x 10 studs |
| Block height | 1.2 studs |
| Travel half-distance | 12 studs |
| Initial block speed | 10.5 studs/s |
| Speed increase / successful drop | 0.25 studs/s |
| Maximum block speed | 17.5 studs/s |
| Travel axis | Alternates X / Z each successful drop |
| Perfect tolerance | <= 0.45 studs center offset on active axis |
| Minimum accepted footprint | 1.75 studs |
| Intentional stage shrink | 0.07 studs from X and Z per accepted stage |
| Tower length | exactly 30 accepted stages |
| Perfect combo cap | 10 |
| Retry / next-tower target | playable again <= 1.0 s after result input |

The active axis is first cut to the geometric overlap. Every accepted stage then narrows both horizontal axes by 0.07 studs, clamped to the minimum footprint, so even a perfect run visibly becomes tighter toward the top without becoming unreasonable on phone screens.

## Grades

- **PERFECT**: center offset <= 0.45 studs. The moving block snaps to the previous block on the active axis before the deliberate per-stage shrink is applied.
- **GOOD**: positive overlap remains and the post-cut footprint stays above the configured 1.75-stud minimum.
- **MISS**: there is no safe overlap remaining after server-side latency-compensated evaluation.

The server computes the grade from authoritative moving-block state. The client never submits score, overlap or grade.

## Score and combo

- GOOD: 10 base points.
- PERFECT: 15 base points.
- Perfect combo increments only on PERFECT and resets on GOOD.
- Multiplier = 1 + min(combo, 10) * 0.10.
- Final drop points are rounded to nearest integer after multiplier.
- Stage is the count of accepted drops in the current tower and is capped at 30.
- Stage 30 changes the round state to **COMPLETED**, stops further drops, grants the tower-completion reward and increments persistent **Player Level** by exactly 1.
- **NEXT TOWER** starts a clean 0/30 tower while preserving Player Level, completed-tower count, currency, cosmetics and best records.
- Personal best is the highest authoritative Score; best stage is stored separately for progression/achievements.

## Failure, retry and revive

- A MISS ends the run exactly once.
- Failure presentation must not delay an immediate retry action.
- Retry creates a clean run from the canonical base state.
- One revive may be consumed per run.
- Revive restores the last valid top footprint and starts a fresh moving block. It does not award score for the missed drop and cannot enlarge the footprint.

## Monetization boundaries

Allowed:
- single-run revive,
- one-use Perfect Shield,
- time-limited 2x coin earning,
- block themes,
- arena themes,
- landing/drop VFX,
- cosmetic trails.

Not allowed:
- buying score,
- buying height/floors,
- direct leaderboard placement,
- surprise purchase prompts,
- paid geometry with easier collision while competing on the same leaderboard.

## Release blockers

V1 cannot be called complete while any of these are true:

1. Character or active block can spawn invisible/off-camera.
2. Camera can detach during normal drop, failure, retry or respawn.
3. Drop input can double-fire.
4. Server accepts client-provided grade/score/overlap.
5. Generated motion can enter an impossible/non-deterministic state.
6. Result/retry flow has a dead end.
7. Compact-phone UI hides the action or core metrics.
8. Persistence has not survived a real new-session rejoin.
9. Published-private-place end-to-end journey has not passed.
10. Known P0/P1 gameplay, economy or purchase defect remains.
