# Perfect Drop — Public Release Record

Date: 2026-10-02

## Roblox identity

- Universe ID: `10768685669`
- Start Place ID: `118957776621075`
- Canonical private-place runtime acceptance was already completed before exposure.
- Public access is now enabled in Creator Dashboard.

## Release configuration completed

- Creator Dashboard content questionnaire completed and published.
- Roblox content maturity result: **Minimal**.
- No content labels, age restriction, or non-conforming regions were reported by the questionnaire result.
- Custom game icon uploaded from accepted gameplay presentation.
- Custom gameplay/store thumbnails uploaded.
- Description configured:
  - `Time the drop. Build a 30-stage tower, chain PERFECT landings, beat your best score, unlock block themes, and keep climbing. Fast retries, daily rewards, cosmetics, and a simple one-input skill loop.`
- Genre: `Party & Casual`
- Subgenre: `Minispiel`
- Audience/access: Public.

## Monetization configuration

Live Roblox IDs are present in `src/shared/config/MonetizationConfig.luau`:

- Revive Developer Product: `3715864717`
- Perfect Shield Developer Product: `3715864864`
- Coin Boost 15m Developer Product: `3715865030`
- Premium Themes Game Pass: `2002172961`

The previous ledger text saying those IDs were still zero is obsolete.

## Repository verification

CI was repaired by removing the duplicate interactive `rokit install` trust step. GitHub Actions run for commit
`3106b71066698566f1c7884d48a3764cd2d3daf1` completed successfully.

## Residual post-launch verification

The real paid Developer Product receipt + subsequent new-session rejoin has not been executed. Receipt idempotency and duplicate-grant behavior were already verified in the runtime QA harness, but the real Robux transaction remains a post-launch operational verification rather than a blocker to the owner-directed public release.

Physical controller/phone/tablet smoke items also remain recorded as residual device QA rather than being falsely marked as verified.
