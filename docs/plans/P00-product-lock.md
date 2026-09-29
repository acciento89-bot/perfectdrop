# P00 — Perfect Drop Product Lock

## Identity

- Product name: **Perfect Drop**
- Core promise: one-button precision stacking with the visible Roblox avatar as the visual anchor.
- Primary run metrics: **Score**, **Height**, **Perfect Combo**, **Best**.
- Landing grades: **PERFECT**, **GOOD**, **MISS**.

## Locked V1 gameplay constants

| Constant | V1 value |
| --- | ---: |
| Base block footprint | 10 x 10 studs |
| Block height | 1.2 studs |
| Initial travel half-distance | 14 studs |
| Initial block speed | 12 studs/s |
| Speed increase / successful drop | 0.45 studs/s |
| Maximum block speed | 22 studs/s |
| Travel axis | Alternates X / Z each successful drop |
| Perfect tolerance | <= 0.35 studs center offset on active axis |
| Minimum accepted footprint | 1.25 studs on active axis |
| Perfect combo cap | 10 |
| Retry target | playable again <= 1.0 s after result input |

The inactive axis keeps the previous top block's footprint exactly. The active axis is cut to the geometric overlap.

## Grades

- **PERFECT**: center offset <= 0.35 studs. The moving block snaps to the previous block on the active axis; no footprint is lost.
- **GOOD**: positive overlap remains and accepted overlap is >= 1.25 studs.
- **MISS**: there is no overlap or accepted overlap would be below 1.25 studs.

The server computes the grade from authoritative moving-block state. The client never submits score, overlap or grade.

## Score and combo

- GOOD: 10 base points.
- PERFECT: 15 base points.
- Perfect combo increments only on PERFECT and resets on GOOD.
- Multiplier = 1 + min(combo, 10) * 0.10.
- Final drop points are rounded to nearest integer after multiplier.
- Height is the count of accepted drops.
- Personal best is highest authoritative Score; height is stored separately for analytics/achievements.

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
