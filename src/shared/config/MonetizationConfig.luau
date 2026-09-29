--!strict

local MonetizationConfig = {
	Products = {
		Revive = { ProductId = 0, FallbackRobux = 29 },
		PerfectShield = { ProductId = 0, FallbackRobux = 19 },
		CoinBoost15m = { ProductId = 0, FallbackRobux = 39 },
	},
	Passes = {
		PremiumThemes = { PassId = 0, FallbackRobux = 79 },
	},
	CompetitiveScorePurchasable = false,
}

return table.freeze(MonetizationConfig)
