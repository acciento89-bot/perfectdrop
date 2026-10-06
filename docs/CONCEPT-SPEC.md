# Perfect Drop Approved Implementation Concept

Portrait 9:16.

Top: Floor, Best, Streak, Coins.
Below: PRECISION JUMP / Land inside the marked bay / progress.
Gameplay: character centered lower-middle, next bay + 2-4 future platforms visible, warm goal ring in distance.
Bottom: joystick left, Boosts center, Jump right.

All UI is real interactive Unity UI; all gameplay geometry is real 3D content. The concept is a production target, not a raster background.


## iPhone Duo / adaptive display contract

The primary art direction remains portrait-first at 9:16, but the runtime must not assume a fixed phone aspect ratio.

- Interactive foreground UI stays inside `Screen.safeArea`; background/world rendering remains edge-to-edge.
- Classic portrait phones use the compact layout.
- Wider portrait windows, including the iPhone Duo inner display, use a wide-portrait layout with smaller edge controls and no critical UI through the middle interaction band.
- A landscape/resizable fallback keeps the center 12% free of critical HUD/controls so the physical fold/division region cannot cover required actions.
- Opening/closing/resizing must not recreate the gameplay scene or reset progress.
- No fixed pixel widths for HUD placement; anchors are recomputed when safe-area or window dimensions change.
- Xcode 27.1 / iOS 27.1 Device Hub validation is a release gate before claiming full iPhone Duo verification.
