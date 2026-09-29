--!strict

local ScoreRules = {}

function ScoreRules.pointsForDrop(
	grade: "PERFECT" | "GOOD",
	currentCombo: number,
	goodBase: number,
	perfectBase: number,
	comboCap: number,
	comboStep: number
): (number, number)
	local nextCombo = if grade == "PERFECT" then math.min(comboCap, currentCombo + 1) else 0
	local base = if grade == "PERFECT" then perfectBase else goodBase
	local multiplier = 1 + nextCombo * comboStep
	return math.floor(base * multiplier + 0.5), nextCombo
end

return ScoreRules
