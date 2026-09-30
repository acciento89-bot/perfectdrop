--!strict

local RetentionConfig = {
	DailyRewardCoins = 25,
	DailyChallenge = {
		PerfectsRequired = 3,
		RewardCoins = 30,
	},
	Achievements = {
		first_perfect = { DisplayName = "First Perfect", RewardCoins = 15 },
		combo_5 = { DisplayName = "Perfect Five", RewardCoins = 25 },
		height_10 = { DisplayName = "Ten High", RewardCoins = 35 },
		height_25 = { DisplayName = "Sky Builder", RewardCoins = 60 },
	},
}

return table.freeze(RetentionConfig)
