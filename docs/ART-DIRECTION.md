# Perfect Drop Art Direction

## Binding reference and scope

![Primary Perfect Drop concept](concepts/PRIMARY-CONCEPT.png)

`docs/concepts/PRIMARY-CONCEPT.png` is the user's fourth attached image, `image-gen-4(7).png`. Its SHA-256 is `edd9bf57abc305838473d20f3a1abe85cd8ad0f79f26ba9a49edb14b202c5f13`. It is the visual source of truth for Perfect Drop. This is a native Unity/C# mobile game for iOS and Android.

The target is a stylized runner making precise jumps between dark floating platforms above a sunset cloud city. Preserve the existing 30-floor course, optional boost, player-controlled orbit camera and immediate checkpoint recovery. Contextual screens appear only after interaction or at the appropriate run state.

This document specifies the required result. It does not certify that the current procedural presentation meets it. Final character, materials, animation and screenshot-quality acceptance remain open in the V1 ledger.

## Color palette

| Role | Target | Use |
| --- | --- | --- |
| Deep navy | #071426 | Platform cores and control backgrounds |
| Panel navy | #101C32 | HUD and contextual panels |
| Slate metal | #313642 | Platform surface plates and bevels |
| Signal gold | #FFC52E | Route edges, landing bays, primary actions |
| Gold highlight | #FFE86A | Small high-intensity accents and success cues |
| Dusk blue | #315A98 | Sky and distant skyline |
| Cloud lavender | #AFACC8 | Atmospheric cloud shadows |
| Sunset peach | #FFB97B | Horizon light and cloud highlights |
| Primary text | #F7FAFF | Values, icons and instructions |
| Secondary text | #ADB9D1 | Stat labels and secondary information |

Gold dominates route and landing-bay presentation as in the concept. Cyan is a restrained secondary accent for optional styles or feedback; permanent large cyan inner bays must not replace the concept's gold landing rectangles. Feedback must also use words, shape and sound so that color is not the only signal.

## Lighting and atmosphere

A low warm sun on the horizon gives platforms, hair and clothing a readable gold rim. Cool blue ambient fill preserves detail on dark faces. The sky transitions from deep blue overhead to peach at the horizon; clouds have soft lavender shadows and warm edges. The player and next landing surface must remain readable when the camera rotates away from the sun.

Use controlled emissive gold strips with a soft halo. Avoid white clipping, full-screen bloom haze or pulse effects that obscure platform boundaries. Atmospheric depth should separate foreground platforms, mid-distance towers and the far skyline. Any post-processing must be checked on actual mobile builds and within the 60 FPS target.

## Materials and platform construction

Visible decks need beveled corners, layered construction, inset dark cores and segmented metal top plates. Surface roughness, restrained wear, seam lines and small fasteners must remain coherent at gameplay distance. Gold trim follows the outer perimeter; the target rectangle sits slightly above the top plate without z-fighting.

Metal, cloth, skin, hair and shoe soles need distinct material responses. Cloth must not look like glossy metal, and platforms must not read as untextured cubes. Hidden box colliders may remain simple. Production geometry can be generated or authored, but its visible silhouette and finish must pass the same concept gate. A renamed primitive or a layer of trim alone does not satisfy that gate.

## World design

Create a clear ascending route of 30 floors above a substantial cloud layer. The next jump must be easy to read against the background. Distant monolithic towers vary in height, width and depth; sparse vertical gold lights connect them to the platform language. Keep decorative towers and clouds outside traversal collision.

Build foreground, middle and distant atmosphere instead of a single flat cloud band. A warm goal ring provides the terminal focal point. The active next bay is strongest; completed and distant platforms can retain subdued architectural markings without competing with the current objective. Avoid clutter over the upcoming landing surface.

All normal gaps must be reachable without boost, including practical takeoff and landing margins. Boost assists; it never unlocks a required jump. Preserve the existing course while testing rather than replacing it from the illustration alone.

## Camera and composition

Use a third-person camera behind and above the runner, with the character in the lower center and the next several platforms visible above. The concept's top HUD, middle route and bottom controls define the portrait hierarchy. Keep the character's feet and the current landing bay visible.

The player controls orbit yaw and pitch by swiping free world space. Do not continuously rotate behind the character. Joystick and button touches must not move the camera. Camera smoothing must follow promptly without lagging behind jumps or blocking the next landing. Rotation, window resize and Duo pose changes must preserve course state, character state and camera ownership.

## Character design and animation

The reference character is a stylized human runner with tousled brown hair, a black hoodie with yellow lining/cuffs, a gold triangular back emblem, dark cropped trousers and white shoes with black/gold accents. Preserve this rear-view silhouette; a helmet or visor is not the default concept design.

Use a cohesive shaped character with connected limbs, readable hands and shoes, and a stable grounded stance. Visible block torsos, detached hands, cube hair spikes or rigid shoe motion are not final production art. Animation needs idle, locomotion, takeoff, airborne anticipation, landing and recovery, with coherent arm/leg/foot motion. Movement direction and animation must agree. Landing compression and quick recovery must not lock input or delay the next jump.

Optional cosmetics may change accents while preserving the silhouette and dark/gold default. Do not rebuild the character or scene when selecting a style or changing layout.

## UI hierarchy and state

Active play exposes four stat cards (Floor, Best, Streak, Coins), one precision objective/progress card, brief landing feedback, joystick left, Boost center and Jump right. Keep the central route open. Settings, profile, daily claim, style selection and completion controls appear only when opened or triggered. A fall normally returns directly to the latest valid platform, without a blocking retry screen.

Portrait uses a top stat row and objective below. Wide portrait and landscape reflow the same objects. In Duo layouts, critical text and touch targets must avoid active reserved/division regions; the world may render across available space. Retain the existing native UIKit reserved-region bridge and responsive HUD. A layout change must not recreate gameplay or reset progress.

## Typography

Use a clean, bold sans serif. Large white numeric values carry the stat hierarchy; smaller cool-gray labels identify them. Objective titles are gold and bold, with white sentence-case instructions below. Avoid decorative fonts, thin weights and outlines that blur on compact phones.

Preserve room for DE/EN strings and large numeric values. German text must fit through layout and wrapping without shrinking action labels below comfortable reading size. Verify the actual rendered font on device; reference-canvas font sizes alone are not an accessibility check.

## Buttons and panels

Jump is a large dark circular control with a gold rim and white upward arrow. The joystick is a dark translucent circle with clear directional affordances and a contrasting thumb. Boost is a gold pill with a dark bolt and label. Provide immediate pressed-state and click feedback with no artificial delay. Use at least 48 logical-point touch targets, consistent spacing and separation from system gestures.

Stat and objective panels use rounded navy surfaces, subtle borders and restrained depth. They must retain contrast over the sunset and clouds. Contextual panels have a clear title, focused content and reachable close/action controls. Their backdrop must not permanently cover game action. Do not reproduce all menu states from reference art simultaneously.

## VFX, sound and haptics

Perfect: an immediate compact gold ring/burst, explicit PERFECT text, a distinct success cue and supported-device success haptic. Good: a smaller clean landing cue with GOOD text and a lighter sound/haptic. A missed jump: explicit MISS/recovery feedback, a short failure cue and immediate return to the latest safe floor. Avoid long death sequences and repeated per-frame landing cues.

Landings, checkpoints and the goal may have short sparks or rings that reinforce the surface rather than hide it. Keep transparent overdraw bounded. Reduced motion removes nonessential drift, pulse and camera disturbance while retaining scoring information. Audio includes jump, grade-specific landing, failure/recovery, UI click and a restrained sunset-city ambience/music bed. Audio and haptic preferences must persist and apply immediately.

## Game feel

Movement starts and stops promptly. Normal jumping, camera control and optional boost work together with simultaneous touch input. Perfect/Good/Miss must be recognizable immediately, including when sound or haptics are disabled. Recovery preserves valid floor progress and clears stale momentum/boost state. Settings and completion states must restore input correctly when closed or restarted.

## Visual acceptance and required evidence

For each candidate, record commit/build version and screenshots from actual execution. Compare composition, character silhouette, platform materials, cloud depth, lighting and HUD against the primary concept. A compile or generated image is not a visual acceptance result.

Required checks include a compact portrait iPhone, the official iPhone Duo open and divided layouts, relevant rotation/window states and representative Android phone/tablet ratios. Check long German text, large values, reachable controls, reserved-region avoidance and unchanged progress after resize. Capture gameplay, grading, fall recovery and contextual panels separately. Physical devices are required for performance, tactile haptics and the TestFlight installation/smoke-test gate.
