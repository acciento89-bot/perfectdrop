--!strict

export type DropResult = {
	grade: "PERFECT" | "GOOD" | "MISS",
	offset: number,
	overlapSize: number,
	overlapCenter: number,
	lostSize: number,
}

local DropRules = {}

local function finite(value: number): boolean
	return value == value and value > -math.huge and value < math.huge
end

function DropRules.evaluate(
	previousCenter: number,
	previousSize: number,
	movingCenter: number,
	movingSize: number,
	perfectTolerance: number,
	minimumFootprint: number
): DropResult
	assert(
		finite(previousCenter)
			and finite(previousSize)
			and finite(movingCenter)
			and finite(movingSize)
			and finite(perfectTolerance)
			and finite(minimumFootprint),
		"drop inputs must be finite"
	)
	assert(previousSize > 0 and movingSize > 0, "block sizes must be positive")
	assert(perfectTolerance >= 0 and minimumFootprint > 0, "tolerances must be valid")

	local offset = movingCenter - previousCenter
	if math.abs(offset) <= perfectTolerance then
		return {
			grade = "PERFECT",
			offset = offset,
			overlapSize = previousSize,
			overlapCenter = previousCenter,
			lostSize = math.max(0, movingSize - previousSize),
		}
	end

	local previousMin = previousCenter - previousSize / 2
	local previousMax = previousCenter + previousSize / 2
	local movingMin = movingCenter - movingSize / 2
	local movingMax = movingCenter + movingSize / 2

	local overlapMin = math.max(previousMin, movingMin)
	local overlapMax = math.min(previousMax, movingMax)
	local overlapSize = math.max(0, overlapMax - overlapMin)

	if overlapSize < minimumFootprint then
		return {
			grade = "MISS",
			offset = offset,
			overlapSize = overlapSize,
			overlapCenter = (overlapMin + overlapMax) / 2,
			lostSize = movingSize,
		}
	end

	return {
		grade = "GOOD",
		offset = offset,
		overlapSize = overlapSize,
		overlapCenter = (overlapMin + overlapMax) / 2,
		lostSize = math.max(0, movingSize - overlapSize),
	}
end

return DropRules
