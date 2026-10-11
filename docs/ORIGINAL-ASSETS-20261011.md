# Perfect Drop original visual assets — 2026-10-11

The complete rebuild uses real Unity meshes and surface assets. The approved Concept remains the unchanged comparison reference. No Concept city, illustrated tower or generated mockup replaces a gameplay framebuffer.

## Original bitmap assets

All eleven imagegen outputs are copied unchanged into `Assets/Resources/Art`. Unity import settings control resolution, mipmaps and sampling. Metal and limestone repeat; the mineral atlas and sky clamp. The original cloud panorama contains only sky/clouds. The three newer city panoramas contain original painted distant architecture on a camera-following, depth-tested environment shell. The playable stack, pedestal, near terrace, canal structures and earned city remain real meshes; distant paintings do not replace the gameplay framebuffer. The title uses transparent alpha over an actual scene render.

| Asset | Original dimensions | Import maximum | SHA-256 of original PNG |
| --- | --- | --- | --- |
| AuthoredBrushedMetalAlbedo.png | 1254 × 1254 | 256 | 96eb9d25f5daec3109415157221903271507abaca96d76fb97d11517c2970fb9 |
| AuthoredPerfectDropTitle.png | 1620 × 971 | 512 | 9e25dc4069c304ba76037ea7b560f33232ed5916f77bab73044e50885c50e7a2 |
| AuthoredGoldenCloudPanorama.png | 1774 × 887 | 2048 | c0257629ee8626c6d31a674ecbd0f1c5aea3efecd7f3bae0d7ef6324742e1ed7 |
| AuthoredMineralAtlas.png | 1774 × 887 | 1024 | 7cb999f3c3bb8fa558b267b41a557821b07d725c208d77f04dd2f766c3b736da |
| AuthoredIvoryLimestone.png | 1254 × 1254 | 512 | fe5c5aa7dcff24ec715dbd6c61bf6c52a3d85e856c9173eaea8d5b40909cbcde |
| AuthoredDistantCityDay.png | 1774 × 887 | 2048 | 826d21530f45d62b3c89aca8195eeb97a999738d9ef34ee96ef75c445f46e3f2 |
| AuthoredDistantCitySunset.png | 1774 × 887 | 2048 | f048116dc36f0ca2a3f0a4963a992bbfe89ea5f10768f24eb1c7bfe075765f53 |
| AuthoredDistantCityNight.png | 1774 × 887 | 2048 | 93bbeec3617f3bf188ab94f69519cd64b6f15baa2ab3fec3b401f84db25532b0 |
| AuthoredCompassInlay.png | 1254 × 1254 | 1024 | 4b3d222e43fa904d9e6a282105f0ecb1575943d24bfc1de75e97234f596035bd |
| AuthoredCypressCutout.png | 1024 × 1536 | 1024 | 9ac4d54d0ab565f7d46ab0c5d4aaf1a663e3f3a60019f5b036dc808e3f781217 |
| AuthoredCloudCutout.png | 1774 × 887 | 1024 | 1c20a7dc9892d89de6c64b7a838ca4c01ea8ac6641f98a748b59eac9afcb3456 |

The original cypress and cloud cutouts retain generated alpha. Cypress alpha is tested for foliage shadows; soft cloud alpha is blended on12 world-anchored cards per chapter. The cards face each rendering camera, retain parallax/depth and are checked at three orbit headings. They replace the earlier smooth opaque sphere clouds. Asset source/prompts/hashes: `cypress-original-provenance.json` and `cloud-cutout-original-provenance.json` in the external evidence directory.

Exact generation prompts/source-output paths: `/Volumes/SSK SSD/Kamilunavo/Builds/GraphicsRefinement-20261011/Drop/complete-rebuild-original-provenance.json`. City generation prompts/source-output paths: `distant-city-original-provenance.json` in the same external evidence directory. Original dimensions/hashes: `complete-rebuild-original-assets.json` in the same directory. The original compass albedo has its full prompt/source in `compass-original-provenance.json` there. These files distinguish reusable bitmap assets from genuine Unity captures.

## Original runtime models and rendering

- Closed rounded lacquer shoulder and turned metal rim meshes, continuous proportional perimeter UVs, geometric normals and finite tangent bases; actual retained/cut footprints use this same six-renderer block kit.
- Original engraved compass albedo with bounds-normalized planar cap UV mapping on a dedicated navy/gold surface material, independent of the shared block materials.
- Modeled circular structural pedestal, split compass petals, engraved ticks, turned balustrades, lanterns, steps and planters. The stone top aligns with the actual physical base beneath the y=0 placed slab; hero previews include that same base. The actual unit-radius Cylinder.fbx now uses the correct horizontal scale; regression checks bind the supporting stone radius to3.57 world units and keep every cap UV inside [0,1]. All lantern stone feet fit the actual drum support and share anchors with their glass. The near support contracts in X/Z only, keeping the skyline and slab footprint unchanged.
- Two detailed architectural rings, three distant houses and six landmarks per chapter, open arcades/canal bridges, recessed window tiers, cornices, porticos, turned columns and ribbed domes. Day, sunset and neon have distinct physical landmarks. Three chapters currently total 236,731 owned vertices and 15 active scenery renderers in engine validation; this is not a physical mobile performance measurement.
- Near cypresses and urn foliage use original transparent botanical albedo on three crossed planes per plant in one owned alpha-tested garden batch, replacing opaque ellipsoid foliage. Distant geometry is reduced to three skyline markers per chapter while the original painted city carries its detail.
- Original day, sunset and futuristic neon distant city paintings on a closed 258-vertex camera-following shell. After-skybox drawing and far-plane depth preserve actual geometry in close, narrow city orbit and tall endless views. Actual render regression catches overwritten sky, missing cap/extent coverage, and near/far opaque overdraw.
- A modeled 32-segment showroom arch with layered moldings, fluted columns and gilding surrounds actual scene previews. Column specimens stand on original modeled stone plinths. Hero and gallery towers use live block models and selected material definitions, with retained/released preview ownership.
- Original material shader for brushed metal, UV-bound jade/pearl and limestone, a cached reflection cubemap, a continuous mirrored sky mapping and depth/normal contact shadows. Perspective and orthographic reconstruction are independently rendered and checked, including shifted principal points and 4× MSAA.
- Landscape placement uses a fixed five-floor camera window rather than fitting the growing entire tower. A tall left world reservation and right action rail preserve usable landing scale; shallow safe panes reflow controls into three rows with minimum49-point targets. Actual UI placement regressions compare8,30,66-floor landing pixel widths.
- Compact Roman-type menu, owned transparent title, genuine chapter previews, minimum 49-point navigation, gold pill actions,49-point visible design selection plates, selected-card highlighting, distinct jade/pearl gallery panels, explicit parent Menu and visible Shop.

## Licensed font

Cinzel is a third-party font, not an original asset. `Assets/Resources/Fonts/Cinzel-Regular.ttf` is a weight-400 instance from the official Google Fonts variable font. Its SIL Open Font License and attribution are retained in `Cinzel-OFL.txt`.

Font source: https://raw.githubusercontent.com/google/fonts/main/ofl/cinzel/Cinzel%5Bwght%5D.ttf. Font SHA-256: `9a7471de0ce382a7ea510779fa685e78d187cd60e2929d5d502b216ab27b20b2`; license SHA-256: `f2b3029aba64c378bf0963b62945eee15e564fe4330b934c8f2eb058282b5e83`.

Validation and actual Concept comparison: `GRAPHICS-COMPARISON-20261011.md`. Functional checks cannot establish Concept similarity. No TestFlight until the exact-source visual gate passes.
