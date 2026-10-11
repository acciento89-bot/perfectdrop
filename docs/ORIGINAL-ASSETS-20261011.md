# Perfect Drop original visual assets — 2026-10-11

The complete rebuild uses real Unity meshes and surface assets. The approved Concept remains the unchanged comparison reference. No Concept city, illustrated tower or generated mockup replaces a gameplay framebuffer.

## Original bitmap assets

All five imagegen outputs are copied unchanged into `Assets/Resources/Art`. Unity import settings control resolution, mipmaps and sampling. Metal and limestone repeat; the mineral atlas and sky clamp. The sky includes only sky/clouds: all visible architecture is actual geometry. The title uses transparent alpha over an actual scene render.

| Asset | Original dimensions | Import maximum | SHA-256 of original PNG |
| --- | --- | --- | --- |
| AuthoredBrushedMetalAlbedo.png | 1254 × 1254 | 256 | 96eb9d25f5daec3109415157221903271507abaca96d76fb97d11517c2970fb9 |
| AuthoredPerfectDropTitle.png | 1620 × 971 | 512 | 9e25dc4069c304ba76037ea7b560f33232ed5916f77bab73044e50885c50e7a2 |
| AuthoredGoldenCloudPanorama.png | 1774 × 887 | 2048 | c0257629ee8626c6d31a674ecbd0f1c5aea3efecd7f3bae0d7ef6324742e1ed7 |
| AuthoredMineralAtlas.png | 1774 × 887 | 1024 | 7cb999f3c3bb8fa558b267b41a557821b07d725c208d77f04dd2f766c3b736da |
| AuthoredIvoryLimestone.png | 1254 × 1254 | 512 | fe5c5aa7dcff24ec715dbd6c61bf6c52a3d85e856c9173eaea8d5b40909cbcde |

Exact generation prompts/source-output paths: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/complete-rebuild-original-provenance.json`. Original dimensions/hashes: `complete-rebuild-original-assets.json` in the same directory. These files distinguish reusable bitmap assets from genuine Unity captures.

## Original runtime models and rendering

- Closed rounded lacquer shoulder and turned metal rim meshes, continuous proportional perimeter UVs, geometric normals and finite tangent bases; actual retained/cut footprints use this same six-renderer block kit.
- Modeled circular structural pedestal, split compass petals, engraved ticks, turned balustrades, lanterns, steps and planters. The stone top aligns with the actual physical base beneath the y=0 placed slab; hero previews include that same base. The near support contracts in X/Z only, keeping the skyline and slab footprint unchanged.
- Two detailed architectural rings, twelve distant houses and six landmarks per chapter, open arcades/canal bridges, recessed window tiers, cornices, porticos, turned columns and ribbed domes. Day, sunset and neon have distinct physical landmarks. Three chapters total 249,137 owned vertices and 14 active scenery renderers in engine validation; this is not a physical mobile performance measurement.
- A modeled 32-segment showroom arch surrounds actual scene previews. Hero and gallery towers use live block models and selected material definitions, with retained/released preview ownership.
- Original material shader for brushed metal, UV-bound jade/pearl and limestone, a cached reflection cubemap, a continuous mirrored sky mapping and depth/normal contact shadows. Perspective and orthographic reconstruction are independently rendered and checked, including shifted principal points and 4× MSAA.
- Compact Roman-type menu, owned transparent title, genuine chapter previews, minimum 49-point navigation, gold pill actions, distinct jade/pearl gallery panels, explicit parent Menu and visible Shop.

## Licensed font

Cinzel is a third-party font, not an original asset. `Assets/Resources/Fonts/Cinzel-Regular.ttf` is a weight-400 instance from the official Google Fonts variable font. Its SIL Open Font License and attribution are retained in `Cinzel-OFL.txt`.

Font source: https://raw.githubusercontent.com/google/fonts/main/ofl/cinzel/Cinzel%5Bwght%5D.ttf. Font SHA-256: `9a7471de0ce382a7ea510779fa685e78d187cd60e2929d5d502b216ab27b20b2`; license SHA-256: `f2b3029aba64c378bf0963b62945eee15e564fe4330b934c8f2eb058282b5e83`.

Validation and actual Concept comparison: `GRAPHICS-COMPARISON-20261011.md`. Functional checks cannot establish Concept similarity. No TestFlight until the exact-source visual gate passes.
