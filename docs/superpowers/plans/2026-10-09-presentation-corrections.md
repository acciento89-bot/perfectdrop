# Perfect Drop Presentation Corrections Implementation Plan

> **For agentic workers:** Use superpowers:executing-plans to implement this plan task by task. The user already requested immediate execution of these corrections.

**Goal:** Keep the moving stack clear in both orientations, replace generic menus with a gold tower gallery, improve the earned city, and teach real stacking actions.

**Architecture:** StackPresentation defines HUD reservations and perspective fitting. Existing StackHud partials retain gameplay and commerce callbacks; a vector TowerPreviewGraphic supplies style previews. Tutorial is an observer of the current run, never a second reward system or a reset of paid progress.

**Tech Stack:** Unity 6000.6.4f1, C#, uGUI, native iOS/Android.

**Spec:** User's 9 October physical-device screenshots and corrective requests; docs/ART-DIRECTION.md (existing art direction).

## Global Constraints
- Preserve wallet, campaign, owned styles, commerce receipts, and interrupted run.
- Keep voluntary rewarded ads and genuine store prices/availability.
- No public store publication; internal testing only.
- At least 48 logical points for primary interactive controls.
- Build one Unity project at a time within available disk space.

## Review Focus
- Landscape iPhone safe areas: power controls never cover active drop geometry.
- Narrow portrait: all level, settings, shop and tutorial actions remain reachable.
- Rotation in an interrupted run: no camera easing through an obstructed pose.
- Tutorial replay on an existing profile: preserve live run and purchased rights.
- Completed city plus empty city: both have an architectural district, not orange slabs.

### Task 1: Reserve gameplay space
Files: StackPresentation.cs, StackHud.cs, ArcadeHud.cs, StackGame.cs, PresentationValidation.cs.
- [ ] Capture failing old landscape overlap and add projection tests for moving extents, axis/yaw and heights.
- [ ] Add Layout(aspect) reservations and FitDistance(bounds, rotation, fov, aspect, pane).
- [ ] Use independent side power rail in landscape and perspective fit of recent stack/motion extents in remaining pane.
- [ ] Run editor and native projected-corner checks.

### Task 2: Tower gallery and earned city
Files: TowerPreviewGraphic.cs, CampaignHud.cs, ArcadeHud.cs, CommerceHud.cs, WorldArt.cs.
- [ ] Replace generic home/footer hierarchy with tower hero, compact chapter/level cards and grouped secondary navigation.
- [ ] Use dimensional style specimens in earned and paid galleries; retain real prices and provider guards.
- [ ] Build roads, landscaping, facade windows and varied architectural crowns for earned towers; landscape empty plots.
- [ ] Inspect rendered portrait and landscape home/gallery/shop/city states.

### Task 3: Current-run tutorial
Files: TutorialHud.cs, StackSave.cs, StackHud.cs, StackGame.cs, PresentationValidation.cs.
- [ ] Add backward-compatible TutorialComplete; verify old profile rights and run survive serialization.
- [ ] Coach actual placement, alignment and repeated placement; skip/replay directly from home, without resetting active run or awarding tutorial coins.
- [ ] Verify fail/retry, skip and replay; compile all real provider packages.

### Task 4: Validate and deliver
- [ ] Review complete diff once with a fresh reviewer; fix important findings and regressions.
- [ ] Build native QA, iOS release and Android release serially, inspect actual surfaces and validate payloads.
- [ ] Commit/push source, retain archives and signed binaries in unpublished drafts, distribute only internal TestFlight.
