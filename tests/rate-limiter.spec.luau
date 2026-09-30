local RateLimiter = require("../src/shared/security/RateLimiter")

local limiter = RateLimiter.new(0.05)
local actor = {} :: any

assert(limiter:Allow(actor), "first action should be allowed")
assert(not limiter:Allow(actor), "immediate duplicate action should be rate-limited")
limiter:Forget(actor)
assert(limiter:Allow(actor), "forgotten actor should be allowed again")

return true
