--!strict

local DifficultyRules = {}

function DifficultyRules.speedForHeight(
	height: number,
	initialSpeed: number,
	speedPerDrop: number,
	maxSpeed: number
): number
	assert(height >= 0, "height must be non-negative")
	return math.min(maxSpeed, initialSpeed + height * speedPerDrop)
end

function DifficultyRules.axisForHeight(height: number): "X" | "Z"
	return if height % 2 == 0 then "X" else "Z"
end

return DifficultyRules
