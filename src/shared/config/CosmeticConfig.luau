--!strict

local CosmeticConfig = {
	DefaultBlockTheme = "graphite",
	DefaultArenaTheme = "rooftop",
	BlockThemes = {
		graphite = {
			DisplayName = "Graphite",
			PriceCoins = 0,
			BaseRGB = { 56, 66, 81 },
			MovingRGB = { 83, 99, 122 },
		},
		sunset = {
			DisplayName = "Sunset",
			PriceCoins = 250,
			BaseRGB = { 116, 59, 64 },
			MovingRGB = { 228, 114, 77 },
		},
		ocean = {
			DisplayName = "Ocean",
			PriceCoins = 450,
			BaseRGB = { 33, 78, 96 },
			MovingRGB = { 57, 159, 181 },
		},
	},
	ArenaThemes = {
		rooftop = {
			DisplayName = "Skyline Rooftop",
			PriceCoins = 0,
			PadRGB = { 49, 58, 72 },
			AccentRGB = { 255, 153, 51 },
		},
		night = {
			DisplayName = "Night Shift",
			PriceCoins = 700,
			PadRGB = { 33, 36, 52 },
			AccentRGB = { 118, 124, 255 },
		},
	},
}

return table.freeze(CosmeticConfig)
