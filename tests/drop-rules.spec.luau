local DropRules = require("../src/shared/game/DropRules")

local perfect = DropRules.evaluate(0, 10, 0.2, 10, 0.35, 1.25)
assert(perfect.grade == "PERFECT", "Perfect grade mismatch")
assert(perfect.overlapSize == 10, "Perfect should preserve footprint")
assert(perfect.overlapCenter == 0, "Perfect should snap to previous center")

local right = DropRules.evaluate(0, 10, 3, 10, 0.35, 1.25)
assert(right.grade == "GOOD", "right overlap should be Good")
assert(math.abs(right.overlapSize - 7) < 0.0001, "right overlap size mismatch")
assert(math.abs(right.overlapCenter - 1.5) < 0.0001, "right overlap center mismatch")

local left = DropRules.evaluate(0, 10, -3, 10, 0.35, 1.25)
assert(left.grade == "GOOD", "left overlap should be Good")
assert(math.abs(left.overlapSize - 7) < 0.0001, "left overlap size mismatch")
assert(math.abs(left.overlapCenter + 1.5) < 0.0001, "left overlap center mismatch")

local miss = DropRules.evaluate(0, 10, 9.2, 10, 0.35, 1.25)
assert(miss.grade == "MISS", "sub-minimum overlap must miss")

local edge = DropRules.evaluate(0, 10, 8.7, 10, 0.35, 1.25)
assert(edge.grade == "GOOD", "minimum accepted overlap should be Good")
assert(math.abs(edge.overlapSize - 1.3) < 0.0001, "edge overlap size mismatch")

local ok = pcall(function()
	DropRules.evaluate(0 / 0, 10, 0, 10, 0.35, 1.25)
end)
assert(not ok, "NaN must be rejected")

return true
