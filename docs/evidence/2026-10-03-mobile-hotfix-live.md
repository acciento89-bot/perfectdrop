# Perfect Drop mobile hotfix live — 2026-10-03

- Canonical source: `main` at `0f88cd2` (includes compact-summary commit `7d375af`).
- Universe: `10768685669` (`Perfect Drop` verified directly in Creator Dashboard).
- Existing production place: `118957776621075` (`Perfekter Drop`).
- Mobile iPhone XR Studio smoke: correct Perfect Drop world, active block/tower visible, single compact `STAGE / SCORE / PERF` summary pill, Shop and Drop controls clear of playfield.
- Full Studio E2E immediately before final layout cleanup passed 30/30 stages, stage-30 camera/readability, Retry, next tower, cosmetics, daily persistence, duplicate receipt idempotency, Revive, respawn and progression meta.
- Static gate after final layout: StyLua, Selene `0 errors / 0 warnings`, Rojo build, 10 pure-Luau test files, release-readiness all passed.
- Cloud-place contamination was removed by explicitly managing ReplicatedStorage, ServerScriptService and StarterPlayerScripts with `$ignoreUnknownInstances: false` in `default.project.json`.
- Final Studio publish returned `PublishSuccessful` and `Published new changes in "Perfekter Drop" to Roblox.`
