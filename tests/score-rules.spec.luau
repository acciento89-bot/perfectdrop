local ScoreRules = require("../src/shared/game/ScoreRules")

local points, combo = ScoreRules.pointsForDrop("PERFECT", 0, 10, 15, 10, 0.10)
assert(points == 17, "first Perfect points mismatch")
assert(combo == 1, "first Perfect combo mismatch")

local points2, combo2 = ScoreRules.pointsForDrop("PERFECT", 10, 10, 15, 10, 0.10)
assert(points2 == 30, "capped Perfect points mismatch")
assert(combo2 == 10, "combo cap mismatch")

local goodPoints, goodCombo = ScoreRules.pointsForDrop("GOOD", 7, 10, 15, 10, 0.10)
assert(goodPoints == 10, "Good points mismatch")
assert(goodCombo == 0, "Good must reset combo")

return true
