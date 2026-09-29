local GameConfig = require("../src/shared/config/GameConfig")
local MonetizationConfig = require("../src/shared/config/MonetizationConfig")

assert(GameConfig.Name == "Perfect Drop", "product name changed")
assert(GameConfig.Block.MinimumFootprint > 0, "minimum footprint must be positive")
assert(
	GameConfig.Block.PerfectTolerance < GameConfig.Block.MinimumFootprint,
	"Perfect tolerance must stay below minimum footprint"
)
assert(GameConfig.Block.InitialSpeed <= GameConfig.Block.MaxSpeed, "initial speed exceeds maximum")
assert(MonetizationConfig.CompetitiveScorePurchasable == false, "competitive score must not be purchasable")

print("Perfect Drop release-readiness static rules passed")
