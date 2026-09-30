local DifficultyRules = require("../src/shared/game/DifficultyRules")

assert(DifficultyRules.speedForHeight(0, 10.5, 0.25, 17.5) == 10.5, "initial speed mismatch")
assert(math.abs(DifficultyRules.speedForHeight(10, 10.5, 0.25, 17.5) - 13) < 0.0001, "speed ramp mismatch")
assert(DifficultyRules.speedForHeight(999, 10.5, 0.25, 17.5) == 17.5, "speed cap mismatch")
assert(DifficultyRules.axisForHeight(0) == "X", "height 0 axis mismatch")
assert(DifficultyRules.axisForHeight(1) == "Z", "height 1 axis mismatch")
assert(DifficultyRules.axisForHeight(2) == "X", "height 2 axis mismatch")

return true
