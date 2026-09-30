local StageRules = require("../src/shared/game/StageRules")

local x = 10
local z = 10
for _ = 1, 30 do
	x = StageRules.shrinkDimension(x, 0.10, 1.75)
	z = StageRules.shrinkDimension(z, 0.10, 1.75)
end

assert(math.abs(x - 7.0) < 0.0001, "30-stage X shrink mismatch")
assert(math.abs(z - 7.0) < 0.0001, "30-stage Z shrink mismatch")
assert(not StageRules.isTowerComplete(29, 30), "stage 29 must not complete tower")
assert(StageRules.isTowerComplete(30, 30), "stage 30 must complete tower")
assert(StageRules.isTowerComplete(31, 30), "stages above cap remain complete")

local minimum = StageRules.shrinkDimension(1.78, 0.10, 1.75)
assert(minimum == 1.75, "minimum footprint clamp failed")

return true
