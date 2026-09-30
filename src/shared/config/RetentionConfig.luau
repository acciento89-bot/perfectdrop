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
		first_tower = { DisplayName = "Tower Complete", RewardCoins = 50 },
		level_5 = { DisplayName = "Level Five", RewardCoins = 100 },
	},
}

return table.freeze(RetentionConfig)
