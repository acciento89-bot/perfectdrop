# Perfect Drop Art Direction

## Target
Premium but buildable: dark floating architecture above clouds, warm gold edge lighting, readable marked bays and a clean portrait HUD.

## Palette
- Background navy: #071426
- Panel navy: #0D1E33
- Gold primary: #FFBD2E
- Gold highlight: #FFD85B
- Cool sky: #5D8DCE
- Primary text: #F7FAFF
- Secondary text: #A9B8CA

## World
- Dark modular platforms with restrained gold emissive trim.
- Marked landing bay on every scoring platform.
- Distant city monoliths and one warm horizon focal point.
- Simple collision; authored final meshes.

## UI
- Four compact top cards: Floor, Best, Streak, Coins.
- One objective card below.
- Joystick bottom-left.
- Boosts bottom-center.
- Jump bottom-right.
- No permanent menu clutter during play.

## Primary visual reference

![Primary Perfect Drop concept](concepts/PRIMARY-CONCEPT.png)

This image is the binding visual target for composition, atmosphere, lighting, platform language, character silhouette and HUD hierarchy. It is not a single simultaneous UI state: gameplay must remain visually open, and contextual panels appear only when invoked or when the run state requires them.

### Non-negotiable visual gates
- The world must read as dark floating architecture above a cloud layer, not as an exposed Unity prototype.
- Platform silhouettes need authored depth, layered trim and believable material response; plain cubes are acceptable only as hidden/simple collision.
- Gold emissive edges guide the route; cyan is reserved for precision/target state.
- The player character must read as a designed runner at gameplay distance, with a clear helmet/visor/body silhouette and motion feedback.
- Skyline, cloud depth and a warm horizon focal point must create depth behind the route without obscuring the next landing.
- HUD spacing and touch controls must preserve the concept hierarchy across classic iPhone, iPad/Android tablet and iPhone Duo reserved regions.
