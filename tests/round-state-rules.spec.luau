local RoundStateRules = require("../src/shared/game/RoundStateRules")

assert(RoundStateRules.canDrop("ACTIVE"), "ACTIVE should accept a drop")
assert(not RoundStateRules.canDrop("FAILED"), "FAILED must reject a drop")
assert(RoundStateRules.canRetry("FAILED"), "FAILED should allow retry")
assert(RoundStateRules.canRetry("COMPLETED"), "COMPLETED should allow next tower")
assert(not RoundStateRules.canRetry("ACTIVE"), "ACTIVE must reject retry")
assert(RoundStateRules.canRevive("FAILED", 0, 1), "unused revive should be available")
assert(not RoundStateRules.canRevive("FAILED", 1, 1), "used revive must be unavailable")
assert(not RoundStateRules.canRevive("ACTIVE", 0, 1), "ACTIVE must reject revive")

return true
