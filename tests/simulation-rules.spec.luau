local SimulationRules = require("../src/shared/game/SimulationRules")

local sample = SimulationRules.sample(500)
assert(sample.minSpeed == 10.5, "simulation minimum speed mismatch")
assert(sample.maxSpeed == 17.5, "simulation maximum speed mismatch")
assert(sample.xCount == 250 and sample.zCount == 250, "axis distribution mismatch")

return true
