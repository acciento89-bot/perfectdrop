# Icon finish and architectural gallery implementation plan

> Execute with Unity feature implementation and a final independent review. The user approved the rendered three-view board and explicitly requested execution.

**Goal:** Bring the actual stacking game and menu toward the October 10 gold/navy icon-quality gallery direction.
**Architecture:** Keep StackGame, saves, timing, powers and ownership intact. Improve shared live block geometry/materials; render menu specimens from those same actual meshes into bounded cached textures. Keep chapter/level callbacks and the measured layout reservations.
**Tech:** Unity 6000.6.4f1, built-in Standard surface shader, uGUI, C#.
**Spec:** `../../ART-DIRECTION.md`, approved board in `/Volumes/SSK SSD/Kamilunavo/Artifacts/KamilunavoDelivery-20261009/concepts-20261010/perfect-drop-3-ansichten.png`.

## Constraints and review focus
- No economy, save/receipt, collider or gameplay changes; DE/EN; portrait/landscape; targets >=48 logical points.
- Preserve six renderers per live block and bounded endless geometry; no per-frame offscreen camera render.
- Selected whole-body style and special cue must survive scene reload, including pedestal.
- Use actual mesh/material previews, never screenshots pretending to be 3D gameplay.
- Existing uncommitted deletion `docs/concepts/PRIMARY-CONCEPT.png` is user state and must not be staged/restored.

## Task 1 — rounded metal-and-gold live blocks
- [ ] Add `Assets/Editor/ConceptFinishValidation.cs`; prove the current dark finish/plate sizing fails the approved-direction contract.
- [ ] Improve `Visuals/WorldArt.cs` shared mesh, broad inset plate, controlled reflective materials; `StackStylePalette.cs` navy/gold, pearl ceramic, jade lacquer material contrast.
- [ ] Use UV-bound fine brush variation without panel seams on the gold top; verify finite mesh normals, thin offcuts, actual shader properties and renderer budget with the editor entrypoint.

## Task 2 — real specimens and illustrated gallery menu
- [ ] Add `Visuals/StackSpecimenStudio.cs`, isolated camera/layer, bounded textures, explicit cache ownership and rendering only when a finish/floor key first needs a specimen.
- [ ] Keep `UI/TowerPreviewGraphic.cs` public palette/preview API while drawing actual cached mesh specimens. No side effects on world camera, lighting, RenderTexture.active or player profile.
- [ ] Refine `UI/CampaignHud.cs` and existing ArcadeHud layout into opaque ivory/navy architectural gallery with image-rich chapter selection and a larger real tower hero; keep all level/actions accessible in compact landscape and portrait.
- [ ] Verify standard/owned/premium skin callbacks, disabled contrast, card masking, targets, reserved game area, long labels, tutorial, exact-once ownership and reload.

## Task 3 — rendered acceptance and source backup
- [ ] Execute ArcadeValidation, new finish validation and source shaders in Unity; build a Development desktop player on the SSD.
- [ ] Run actual `-qaStyles`, `-qaPresentation`, `-qaArcade`; inspect fresh gameplay/menu/design/city portrait and landscape captures against the board and fix visual gaps.
- [ ] Independent review; commit coherent verified source to canonical main and retain evidence. Native/TestFlight distribution follows only after visual acceptance; no public release.
