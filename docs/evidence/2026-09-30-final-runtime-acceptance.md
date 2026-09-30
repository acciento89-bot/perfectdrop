# Perfect Drop V1 — Final Runtime Acceptance

Date: 2026-09-30

## Canonical build

- Build artifact: `build.rbxlx`
- SHA-256: `49ff35815a25454e67186b2fd5915f45cdbaecf2b09eb50b64e0f0e902b2613c`
- Private Roblox place ID: `118957776621075`
- Universe ID: `10768685669`
- Latest place-version check observed after final overwrite: `17`

## Published private-place E2E

The final cloud-place run completed the complete player journey successfully.

Verified runtime gates:
- avatar spawns and remains visible;
- scripted follow camera remains attached and close enough at stage 30;
- production art landmarks, block treatment and landing guide are present;
- one tower contains exactly 30 accepted stages;
- stage 30 ends the tower and prevents stage 31;
- tower footprint shrinks while remaining above the minimum footprint;
- player level and completed-tower count increment exactly once;
- NEXT TOWER removes old run geometry and starts a clean stage-0 run;
- deliberate MISS transitions to FAILED exactly once;
- post-failure drops are rejected;
- retry restores a clean ACTIVE run;
- HUD actions and minimum touch-target dimensions are present;
- Daily reward state is duplicate-safe;
- persisted Sunset block-theme ownership was restored;
- cosmetic equip is server-authoritative and changes live gameplay presentation;
- duplicate receipt simulation grants exactly once;
- revive restores the last valid stack and marks the run assisted;
- respawn restores the character and follow camera;
- progression best score/height and currency remain authoritative.

Representative final cloud run:
- starting profile: Player Level 4 / 3 towers completed;
- completion: 24 PERFECT, 6 GOOD, score 715;
- stage: 30/30;
- next level: Player Level 5;
- completed footprint remained within configured bounds;
- full journey ended with `[Perfect Drop E2E] COMPLETE full player journey passed`.

A subsequent run restored the persisted profile state before continuing, confirming real DataStore rejoin persistence.

## QA screenshots

- `/tmp/perfectdrop-qa/cloud-GAMEPLAY.jpg`
- `/tmp/perfectdrop-qa/final-completion-clean.jpg`
- `/tmp/perfectdrop-qa/final-shop-clean2.jpg`

## Static verification

- StyLua: pass
- Selene: 0 errors / 0 warnings / 0 parse errors
- Pure Luau: 10 test modules passed
- Release-readiness static rules: pass
- Rojo canonical build: pass

## External release gates

These are not gameplay defects:
1. Developer Product / Game Pass Roblox IDs are still configured as `0`; a real paid receipt + rejoin cannot be executed until IDs are created in Creator Dashboard.
2. Final public-store icon/thumbnails/metadata and Roblox content/privacy questionnaire require Creator Dashboard submission.
3. Physical phone/tablet/controller hardware smoke is still external to the automated Studio matrix; responsive/touch sizing and controller bindings are implemented and runtime-checked where Studio exposes them.

The gameplay V1 is feature-complete and the published private-place journey passes. Public launch remains blocked only by the external release items above.
