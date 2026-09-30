--!strict

local DifficultyRules = require("./DifficultyRules")
local GameConfig = require("../config/GameConfig")

local SimulationRules = {}

function SimulationRules.sample(count: number)
	local minSpeed = math.huge
	local maxSpeed = -math.huge
	local xCount = 0
	local zCount = 0
	for height = 0, count - 1 do
		local speed = DifficultyRules.speedForHeight(
			height,
			GameConfig.Block.InitialSpeed,
			GameConfig.Block.SpeedPerDrop,
			GameConfig.Block.MaxSpeed
		)
		minSpeed = math.min(minSpeed, speed)
		maxSpeed = math.max(maxSpeed, speed)
		if DifficultyRules.axisForHeight(height) == "X" then
			xCount += 1
		else
			zCount += 1
		end
	end
	return {
		minSpeed = minSpeed,
		maxSpeed = maxSpeed,
		xCount = xCount,
		zCount = zCount,
	}
end

return SimulationRules
