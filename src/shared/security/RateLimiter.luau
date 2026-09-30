--!strict

local RateLimiter = {}
RateLimiter.__index = RateLimiter

function RateLimiter.new(intervalSeconds: number)
	assert(intervalSeconds > 0, "interval must be positive")
	return setmetatable({
		_interval = intervalSeconds,
		_last = {} :: { [Player]: number },
	}, RateLimiter)
end

function RateLimiter:Allow(player: Player): boolean
	local now = os.clock()
	local previous = self._last[player] or -math.huge
	if now - previous < self._interval then
		return false
	end
	self._last[player] = now
	return true
end

function RateLimiter:Forget(player: Player)
	self._last[player] = nil
end

return RateLimiter
