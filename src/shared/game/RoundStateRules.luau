--!strict

local RoundStateRules = {}

function RoundStateRules.canDrop(state: string): boolean
	return state == "ACTIVE"
end

function RoundStateRules.canRetry(state: string): boolean
	return state == "FAILED"
end

function RoundStateRules.canRevive(state: string, revivesUsed: number, maxRevives: number): boolean
	return state == "FAILED" and revivesUsed < maxRevives
end

return RoundStateRules
