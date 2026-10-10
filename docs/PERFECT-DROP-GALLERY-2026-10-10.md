# Approved October 10 architectural gallery — source handoff

Approved board: `/Volumes/SSK SSD/Kamilunavo/Artifacts/KamilunavoDelivery-20261009/concepts-20261010/perfect-drop-3-ansichten.png`. This implements the menu portion of `docs/superpowers/plans/2026-10-10-icon-gallery.md`; root owns live finish/meshes/specimen studio and heavy validation.

## Implemented source contract

The campaign menu is an opaque ivory architectural gallery. Its larger actual shared-mesh tower specimen sits over the existing owned chapter scene illustration. Three image-rich Wolkenwerk / Abendturm / Neonstadt cards carry chapter callbacks. A compact ten-level two-column gallery retains all original level callbacks and scrolls to the first and last full card. The old final-row trailing padding is removed, so clamping cannot hide the last row. There is one gold Continue/Play action; designs/city use navy surfaces and the other utilities use cream surfaces. All names used by QA remain unchanged.

Measured layout uses logical points, including readable text sizes, rather than assuming reference-canvas units equal native points. Compact portrait and landscape keep distinct scene/gallery arrangements. The gallery, style, optional-shop and city navigation share ivory/ink surfaces; style specimens remain the actual eight existing finishes through root's shared `TowerPreviewGraphic` API. The panel shader gives ivory a neutral paper response rather than the old dark-card cool gradient. Menu images are the already owned chapter artwork; active gameplay stays real 3D and no image is used as fake gameplay.

Economy, callbacks, save/paid ownership, tutorial replay/skip semantics, camera/world reservations and gameplay blocks are unchanged by this owner. The user deletion `docs/concepts/PRIMARY-CONCEPT.png` is preserved.

## Files owned by this pass

- `Assets/Scripts/UI/CampaignHud.cs`: specimen/chapter/gallery/action hierarchy, reachability, theme and point-based menu typography.
- `Assets/Scripts/UI/GalleryTheme.cs` plus meta: palette, final-surface button contrast, owned art crop and logical-point text sizing.
- `Assets/Scripts/UI/ArcadeHud.cs`: home/city navigation theme and readable labels; gameplay powers/reservations retained.
- `Assets/Scripts/UI/CommerceHud.cs`: ivory optional showroom/shop, owned/premium selection styling and readable labels; store callbacks retained.
- `Assets/Scripts/UI/TutorialHud.cs`: themed Learn entry; lesson behavior retained.
- `Assets/Resources/PerfectDropPanel.shader`: neutral ivory branch; gold/navy retained.
- `Assets/Editor/GalleryMenuValidation.cs` plus meta; one `ArcadeValidation.Validate` integration call preserving root's finish validation.

## Evidence and root acceptance

Fresh whole-source Roslyn compilation against current Unity6000.6.4f1 engine and matching current Library/Bee UI/commerce refs succeeds. Existing deprecated Unity API/external netstandard warnings only. Response and successful log: `.utmp/gallery-check/source.rsp` / `compile.log` (ignored). Focused diff whitespace passes. Whole-tree whitespace finds a pre-existing parent-owned `ProjectSettings.asset:303` blank build-target entry; this source owner does not alter that file.

`Kamilunavo.PerfectDrop.Editor.GalleryMenuValidation.Validate` is called by `ArcadeValidation.Validate`. It explicitly rejects the former clear/dark map identity (negative control), requires an opaque ivory gallery, real specimen hero, three distinct legitimate chapter pictures, exactly one gold primary, all-state >=4.5 contrast, DE/EN compact-safe targets >=48 points, and wholly visible first/last level cards after scroll. Fixtures cover375×627,430×850,627×355,850×394 and190×627 point panes. These engine checks are implemented but were not executed by this source owner.

Root should serially run `Kamilunavo.PerfectDrop.Editor.ArcadeValidation.Validate`, `BuildAutomation.BuildMacPreview`, then the actual development player's `-qaPresentation <fresh-dir>`, `-qaStyles <fresh-dir>` and `-qaArcade <fresh-dir>`. Existing presentation captures include gallery-home-portrait, gallery-home-430/932, gallery-last-levels-430/932, gallery-styles-430/932, gallery-shop/bottom-430/932, city and actual tutorial/gameplay framing. Inspect those against the approved board. Source compilation does not establish shader/render/native acceptance or physical performance. Main visual risks are actual specimen exposure over the bright scene, very narrow chapter labels, and native point-scaling/scroll bounds; the new checks and captures cover the corresponding gates.

No engine/build/native/version/commit/push/publication operations were performed by this owner.
