local GameConfig = require("../src/shared/config/GameConfig")
local MonetizationConfig = require("../src/shared/config/MonetizationConfig")

assert(GameConfig.Name == "Perfect Drop", "product name changed")
assert(GameConfig.Block.MinimumFootprint > 0, "minimum footprint must be positive")
assert(
	GameConfig.Block.PerfectTolerance < GameConfig.Block.MinimumFootprint,
	"Perfect tolerance must stay below minimum footprint"
)
assert(GameConfig.Block.InitialSpeed <= GameConfig.Block.MaxSpeed, "initial speed exceeds maximum")
assert(GameConfig.Progression.MaxStages == 30, "V1 tower must have exactly 30 stages")
assert(GameConfig.Block.StageShrinkPerDrop > 0, "stage footprint must narrow as the tower rises")
assert(GameConfig.Block.TravelHalfDistance <= 12, "mobile travel span drifted beyond the V1 framing limit")
assert(MonetizationConfig.CompetitiveScorePurchasable == false, "competitive score must not be purchasable")

print("Perfect Drop release-readiness static rules passed")
