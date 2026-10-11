# Perfect Drop navigation revision — 2026-10-11

The user rejects the shared ivory collector-style menu and requires actual Unity-rendered comparison with the approved Perfect Drop board before any new TestFlight delivery. Perfect Drop's identity is navy metal, warm gold, an architectural night skyline and its actual dimensional stacking tower. It must remain distinct from Floor's cream/copper lift gallery and Rising's atlas.

## Baseline source evidence

`CampaignHud.BuildCampaignMenus` constructs an opaque full ivory LevelMap with the specimen hero, three chapter cards, ten level cards and six utilities active together. `LayoutCampaign` subtracts two utility rows and the play action from the remaining level view, leaving a short scrolling strip. `CommerceHud.LayoutShopRows` imposes 154 canvas units for every shop row independent of content. This explains the simultaneous density and bulky repeated controls reported by the user.

The old `GalleryMenuValidation` explicitly requires ivory and all ten levels on default home. That design assertion is superseded by this user request; target, contrast, style and first/last-level reachability checks must remain.

## New discriminating regression

Entrypoint: `Kamilunavo.PerfectDrop.Editor.DropNavigationRegression.Validate`. The real existing UI is constructed across DE/EN portrait, landscape and narrow reserved-pane fixtures. Actual Unity baseline exited 1 with all 18 expected failures across six original DE/EN/pane cases: default ivory instead of navy, level gallery active on landing, no separate CampaignNavigation. There were no C# compilation errors. Receipt: `/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/baseline-drop-menu.log`. The completed fixture follows real campaign/navigation/back/design callbacks, rotates the retained campaign, checks full first/last-level reachability and >=49 logical point/all-state4.5 contrast, and confirms the selected shared-mesh specimen plus unchanged wallet, paid ownership and progression.

## Implemented UI source; engine/render verification pending

Source syntax passes the bundled Roslyn parser with zero errors across the four UI files and both scoped editor checks. This is syntax evidence only.

The landing emphasizes the next campaign level and selected tower over an owned skyline, with one gold Play/Continue. Campaign, designs and city open separately. Compact practice/settings actions remain available. Campaign shows its three chapters, ten reachable level choices, daily challenge and unlockable endless in dedicated space. Designs remain dimensional tower specimens, never a generic inventory table. Shop rows vary by content with all original commerce guards, ownership and IDs preserved.

Only authorized UI files and narrowly scoped editor checks are owned here. StackGame, rules, save, physics, world art, tower meshes, shaders, versions, runtime QA, Git and native exports are root-owned or out of scope. Existing RuntimeSmoke/RenderSoak flows must open CampaignNavigation before looking up active chapter/level-gallery controls; the root worker coordinates that harness update and serial engine validation.

Source, actual RED/GREEN receipts and rendered acceptance must be distinguished. Native delivery is gated on visual comparison, and no new TestFlight upload is authorized by a compile-only pass.
