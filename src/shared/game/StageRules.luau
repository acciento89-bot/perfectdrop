--!strict

local StageRules = {}

function StageRules.shrinkDimension(size: number, shrinkPerStage: number, minimumFootprint: number): number
	assert(size > 0, "size must be positive")
	assert(shrinkPerStage >= 0, "stage shrink must be non-negative")
	assert(minimumFootprint > 0, "minimum footprint must be positive")
	return math.max(minimumFootprint, size - shrinkPerStage)
end

function StageRules.shrinkFootprint(size: Vector3, shrinkPerStage: number, minimumFootprint: number): Vector3
	return Vector3.new(
		StageRules.shrinkDimension(size.X, shrinkPerStage, minimumFootprint),
		size.Y,
		StageRules.shrinkDimension(size.Z, shrinkPerStage, minimumFootprint)
	)
end

function StageRules.isTowerComplete(stage: number, maxStages: number): boolean
	assert(maxStages > 0, "maxStages must be positive")
	return stage >= maxStages
end

return StageRules
