--!strict

local EconomyConfig = {
	CoinsPerGood = 1,
	CoinsPerPerfect = 2,
	HeightBonusEvery = 10,
	HeightBonusCoins = 5,
	TowerCompletionCoins = 75,
	ProfileVersion = 2,
	DataStoreVersion = 1,
	DataStoreBaseName = "PerfectDropPlayerData",
}

return table.freeze(EconomyConfig)
