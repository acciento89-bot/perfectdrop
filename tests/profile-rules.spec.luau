local ProfileRules = require("../src/shared/game/ProfileRules")

local fresh = ProfileRules.defaultProfile()
assert(fresh.Coins == 0, "fresh coins mismatch")
assert(fresh.OwnedBlockThemes.graphite == true, "default block theme missing")
assert(fresh.OwnedArenaThemes.rooftop == true, "default arena theme missing")

local migrated = ProfileRules.normalize({
	Coins = -5,
	BestScore = 123.9,
	OwnedBlockThemes = { sunset = true },
	EquippedBlockTheme = "missing",
	Consumables = { Revive = 2.8, PerfectShield = -4 },
})
assert(migrated.Coins == 0, "negative coins not clamped")
assert(migrated.BestScore == 123, "best score not normalized")
assert(migrated.EquippedBlockTheme == "graphite", "invalid equipped theme not repaired")
assert(migrated.Consumables.Revive == 2, "revive count not normalized")
assert(migrated.Consumables.PerfectShield == 0, "shield count not clamped")
assert(ProfileRules.dayKey(86400 * 3 + 1) == 3, "day key mismatch")

return true
