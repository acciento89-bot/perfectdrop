--!strict

local GameConfig = {
	Name = "Perfect Drop",
	Block = {
		BaseSize = 10,
		Height = 1.2,
		TravelHalfDistance = 12,
		InitialSpeed = 10.5,
		SpeedPerDrop = 0.25,
		MaxSpeed = 17.5,
		PerfectTolerance = 0.45,
		MinimumFootprint = 1.75,
		StageShrinkPerDrop = 0.10,
	},
	Progression = {
		MaxStages = 30,
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
