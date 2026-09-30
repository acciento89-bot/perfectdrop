--!strict

local RetentionRules = {}

function RetentionRules.canClaimDaily(lastClaimDay: number, currentDay: number): boolean
	return currentDay > lastClaimDay
end

function RetentionRules.normalizeChallengeDay(
	storedDay: number,
	currentDay: number,
	perfects: number,
	rewarded: boolean
): (number, number, boolean)
	if storedDay ~= currentDay then
		return currentDay, 0, false
	end
	return storedDay, math.max(0, perfects), rewarded
end

return RetentionRules
