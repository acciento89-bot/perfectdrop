--!strict

local CosmeticConfig = {
	DefaultBlockTheme = "graphite",
	DefaultArenaTheme = "rooftop",
	BlockThemes = {
		graphite = { DisplayName = "Graphite", PriceCoins = 0 },
		sunset = { DisplayName = "Sunset", PriceCoins = 250 },
		ocean = { DisplayName = "Ocean", PriceCoins = 450 },
	},
	ArenaThemes = {
		rooftop = { DisplayName = "Skyline Rooftop", PriceCoins = 0 },
		night = { DisplayName = "Night Shift", PriceCoins = 700 },
	},
}

return table.freeze(CosmeticConfig)
