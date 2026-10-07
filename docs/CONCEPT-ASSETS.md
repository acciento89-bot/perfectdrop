# Perfect Drop production art

These textures were generated with the built-in ImageGen tool on 2026-10-07 and copied into the game repository. The user's fourth attached concept is the binding modern gunmetal/gold/cloud-city reference. The abandoned fantasy architecture variant is not used. No background contains gameplay slabs, runner, controls or text: stacking, trimming and earned buildings are rendered as live geometry.

## Assets and production specifications

All files are under `Assets/Resources/Art/`.

- `CloudCityPortrait.png` / `CloudCityLandscape.png`: premium modern geometric navy/gunmetal towers with beveled roofs and vertical golden light strips, sunset from the right, cobalt sky and peach/lavender volumetric cloud sea. Towers frame both edges; centre remains open for the live tower. Portrait 9:16 and a separately recomposed landscape 16:9.
- `CloudCityDayPortrait.png` / `CloudCityDayLandscape.png`: same architecture/composition, clear azure daylight, fluffy white/lilac clouds, warm sunlight, restrained visible gold strips. Chapter 1 Cloud Works.
- `CloudCityNightPortrait.png` / `CloudCityNightLandscape.png`: same architecture/composition, cobalt/violet twilight, moonlight, luminous lavender/pink clouds, cyan/magenta/gold building strips. Keep the centre bright enough for navy gameplay geometry. Chapter 3 Neon City.
- `GunmetalPanels.png`: square tileable, straight orthographic albedo for metal decks. Cool neutral slate-grey brushed steel, restrained scratches/wear, four engineered panels with thin seams and tiny recessed fasteners. No perspective, bevels, baked directional lighting, shadows, highlights, glow, gold markings or text. The actual shader tints this texture and supplies lighting.

## Final variant prompt set

Day portrait: “Change only the time of day and atmospheric lighting to a beautiful clear azure-blue daytime sky with bright fluffy white and pale lilac clouds, soft sunlit golden edges, subtle warm sunlight from the right. Preserve exactly the same modern navy gunmetal geometric skyscrapers, roof shapes and vertical gold light strips, positions, perspective and empty central gameplay zone. Keep the premium stylized 3D rendering, realistic metal surfaces and believable soft cloud volume. Gold architecture lights remain tasteful and clearly visible. Do not add objects, vegetation, characters, game platforms, UI, text or logos. Portrait 9:16.”

Night portrait: “Change only atmospheric time of day and illumination: deep cobalt blue and violet twilight sky, luminous soft violet/pink cloud sea, moonlight from the right and vivid cyan, magenta and gold building light strips reflecting subtly on gunmetal. Preserve the exact modern rectangular geometric skyscrapers, roof shapes, positions, perspective and clear central gameplay zone. Brighter cinematic twilight, not near-black darkness, so a navy stack silhouette will read clearly against the lavender centre clouds. No sunset sun; subtle moon glow high on the right. No characters, game platforms, UI, text or logos. Portrait 9:16.”

Landscape variants: “Recompose the attached portrait cloud city into a wide 16:9 landscape view by extending the world naturally to the sides. Preserve the matching chapter's colour palette, time of day, exact modern rectangular gunmetal towers with beveled roofs, materials and detailed soft cloud volume. Architectural framing at the left and right edges, leaving the middle third clear for the actual live game stack. No character, gameplay blocks or platforms, UI, text or logo. This is a distant environment background asset, not a gameplay screenshot.”

The font files in `Assets/Resources/Fonts/` are Barlow regular and bold from the official Google Fonts repository. Their SIL Open Font License is retained in `docs/licenses/Barlow-OFL.txt` and shipped in `Assets/StreamingAssets/Barlow-OFL.txt`.

## Runtime use and acceptance

`WorldArt` selects the chapter/orientation backdrop; the sky shader preserves image aspect through a cover crop. Backdrops use clamped wrapping and preserve NPOT aspect. Shared beveled meshes, instanced material tint/emission and a cached studio reflection shade the actual decks. A separate additive ring and small sparks provide Perfect feedback; reduced motion suppresses these transient effects. Bloom runs at quarter resolution and releases its temporary targets.

Generated imagery and build success are not the visual acceptance gate. Inspect actual map, gameplay, cut/result, powers, styles and city captures; exercise compact/landscape/division layouts; check shader/runtime errors and rendering/stability measurements. Native device input, performance, haptic/audio and TestFlight checks remain separate from desktop evidence.
