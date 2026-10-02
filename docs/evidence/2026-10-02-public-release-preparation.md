# Perfect Drop — Public-release preparation 2026-10-02

## Canonical platform IDs

- Universe: `10768685669`
- Place: `118957776621075`
- accepted private-place version previously verified: v17

## Monetization IDs

- Revive: `3715864717`
- Perfect Shield: `3715864864`
- Coin Boost 15m: `3715865030`
- Premium Themes pass: `2002172961`

## Repository verification

The GitHub Actions failure on head `699967511754d4257c51eb0756f9789b61c0eed4` was infrastructure-only: setup-rokit already installed tools with `--no-trust-check`, then the workflow redundantly ran interactive `rokit install`, which failed on StyLua trust.

Commit `3106b71066698566f1c7884d48a3764cd2d3daf1` changes that redundant command to `rokit install --no-trust-check`. CI run `36961489748` completed successfully.

Local source verification also passed:
- StyLua
- Selene: 0 errors / 0 warnings / 0 parse errors
- 10 pure-Luau test files
- release-readiness
- Rojo build

## Store art

Final prepared local assets:
- `icon-512.jpg` 512x512
- `thumbnail-1-gameplay.jpg` 1920x1080
- `thumbnail-2-stack.jpg` 1920x1080
- `thumbnail-3-shop.jpg` 1920x1080

They are cropped only from already accepted real runtime evidence.

## Dashboard blocker

The Creator Dashboard currently presents an account-level **Updated Agreements** modal covering revised Roblox Terms of Service and Privacy Policy effective 2026-11-01. Accepting that agreement is a legal/account-owner action and was deliberately not performed by automation.

Until the owner accepts it, final store uploads, questionnaire submission and public-access save cannot be completed through the dashboard.

The owner has explicitly instructed that completed Roblox projects should be published directly. Accordingly, the real paid Developer Product receipt/rejoin remains documented as post-launch verification rather than a pre-public blocker.
