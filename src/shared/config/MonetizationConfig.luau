--!strict

local MonetizationConfig = {
	Products = {
		Revive = { ProductId = 3715864717, FallbackRobux = 29 },
		PerfectShield = { ProductId = 3715864864, FallbackRobux = 19 },
		CoinBoost15m = { ProductId = 3715865030, FallbackRobux = 39 },
	},
	Passes = {
		PremiumThemes = { PassId = 2002172961, FallbackRobux = 79 },
	},
	CompetitiveScorePurchasable = false,
}

return table.freeze(MonetizationConfig)
