# Perfect Drop optional monetization design — 2026-10-08

## Intent and authorization
The user physically tested internal TestFlight1.0(5) on iPhone16ProMax: sound/haptics good, no perceived stutter; landscape Drop awkward. They explicitly requested revenue-generating monetization and selected voluntary rewarded videos + design/starter purchases. Prior instructions authorize continuous autonomous implementation, main commits/push and internal TestFlight; no public store publication. Existing free campaign/endless/earned designs/progress remain intact. Genuine Duo poses remain unverified, with no Duo available to the user.

## Independent UI correction
Landscape Drop overlaps Risk in build5. Reproduced with actual runtime bounds regression and failure capture. Move Drop to normalized safe-pane (.69,.09,.27,.24), powers to left (.04,.07,.60,.25). Portrait unchanged. Test landscape4:3 and iPhone16ProMax956x440 aspect, all interactive raycasts, full30-drop/save/resume. These captures are desktop layout evidence, not physical revalidation.

## Purchases
Use pinned Unity IAP5.4.4 from official Unity registry for Apple/Google native stores. No subscription or external checkout. Product IDs: com.kamilunavo.perfectdrop.starter (non-consumable), com.kamilunavo.perfectdrop.neoncollection (non-consumable). Starter includes exclusive Copper style4 and 500 coins once on a new pending purchase; restoring confirmed entitlement only restores its style. Collection includes exclusive styles5–7 (Pearl, Violet, Solar), without changing existing styles0–3 or gameplay difficulty. Prices come from fetched localized store metadata, never hardcoded into player purchase buttons; actual catalog prices require store configuration and are not published by this design.

A separate serialized commerce record preserves entitlement bits and fulfilled transaction IDs within StackProfile; schema2 remains compatible with additive fields and validation. Pending purchases validate supported known product/transaction identifiers, atomically persist reward+transaction marker, then confirm. Replay cannot grant twice; confirmed restoration grants only non-consumable cosmetics. Handle cancellation/failure/deferred/offline with clear status and no reward. Fetch/restore active store entitlements; provide Restore Purchases. StoreKit2 verified store transactions are the Apple trust boundary; Google purchases originate native Billing SDK. Client-owned offline wallets remain tamperable; do not claim server-authoritative receipt/security verification. No backend is silently provisioned.

## Rewarded advertising
Provider-independent service boundary, default Google Mobile Ads11.5.0 SDK candidate following verified official documentation. Directory search for mobile rewarded ads, AdMob and UnityAds returned no entries. AdMob account/app IDs/unit IDs remain required; user asked whether an account already exists. No credentials in source. Platform app/unit IDs are non-secret configuration. Real release ads require real configured IDs; internal development uses official test units only. Never initialize live ads during editor/desktop QA. No interstitial/banner/app-open ads. Coins+50 only on genuine reward callback, with single-use attempt token, maximum5/day UTC and60-second post-success cooldown. Close/error/no-fill/declined consent awards nothing. Busy state prevents concurrent show; pause gameplay while native overlay active and resume/save safely. SDK consent gathering and privacy-options entry must precede ads requests. Configure required native app IDs/SDK build integrations and privacy declarations for actual SDK behavior.

## UI
A separate contextual premium shop and reward offer, accessible through Designs, includes two purchase cards, localized price/status, restore action, video+50 and privacy options. Existing earned-design selection remains available. Layout supports portrait, landscape and reserved safe pane; minimum48 logical point hit targets, no permanent shop overlays during play. Owned products show Owned, unavailable store/ads disabled. No fake buy/watch callbacks in release. QA fakes are confined to development/editor tests.

## Acceptance
Pure tests: additive old-profile loading; product grant/replay/unknown/deferred/reward failure; restore without starter coin replay; UTC caps/cooldown; failed persistence does not acknowledge; multiple callback protection. Actual runtime shop layout/button raycasts + old arcade/save/style regression. Fresh iOS/Android compile/build, StoreKit sandbox purchase/cancel/restore and rewarded test-ad completion/no-fill/device lifecycle. Production revenue is NOT claimed until real store catalog, paid agreement/account status and AdMob configuration are verified. Public publishing remains unauthorized.

## Sources
- https://docs.unity.com/en-us/iap/set-up-in-app-purchasing
- https://docs.unity.com/en-us/iap/purchases
- https://developers.google.com/admob/unity/quick-start
- https://developers.google.com/admob/unity/rewarded
