local DifficultyRules = require("../src/shared/game/DifficultyRules")

assert(DifficultyRules.speedForHeight(0, 12, 0.45, 22) == 12, "initial speed mismatch")
assert(math.abs(DifficultyRules.speedForHeight(10, 12, 0.45, 22) - 16.5) < 0.0001, "speed ramp mismatch")
assert(DifficultyRules.speedForHeight(999, 12, 0.45, 22) == 22, "speed cap mismatch")
assert(DifficultyRules.axisForHeight(0) == "X", "height 0 axis mismatch")
assert(DifficultyRules.axisForHeight(1) == "Z", "height 1 axis mismatch")
assert(DifficultyRules.axisForHeight(2) == "X", "height 2 axis mismatch")

return true
