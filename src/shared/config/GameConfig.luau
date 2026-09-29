--!strict

local GameConfig = {
	Name = "Perfect Drop",
	Block = {
		BaseSize = 10,
		Height = 1.2,
		TravelHalfDistance = 14,
		InitialSpeed = 12,
		SpeedPerDrop = 0.45,
		MaxSpeed = 22,
		PerfectTolerance = 0.35,
		MinimumFootprint = 1.25,
	},
	Scoring = {
		GoodBase = 10,
		PerfectBase = 15,
		PerfectComboCap = 10,
		ComboStep = 0.10,
	},
	Round = {
		MaxRevives = 1,
		RetryTargetSeconds = 1.0,
	},
}

return table.freeze(GameConfig)
