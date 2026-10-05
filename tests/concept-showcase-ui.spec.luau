local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(string.find(hud, 'conceptShowcase.Name = "ConceptShowcase"', 1, true), "concept showcase layer missing")
assert(string.find(hud, 'shopShowcase.Name = "DropShopShowcase"', 1, true), "drop shop showcase missing")
assert(string.find(hud, 'conceptProgressCard.Name = "ConceptProgressCard"', 1, true), "progress showcase missing")
assert(string.find(hud, 'conceptDailyRewards.Name = "ConceptDailyRewards"', 1, true), "daily showcase missing")
assert(string.find(hud, 'conceptVistaCard.Name = "ConceptVistaCard"', 1, true), "visual showcase missing")
assert(
	string.find(hud, 'local conceptViewport = Instance.new("ViewportFrame")', 1, true),
	"visual showcase must use native ViewportFrame"
)
assert(
	string.find(hud, "conceptShowcase.Visible = false", 1, true),
	"concept montage must remain hidden during normal gameplay"
)
assert(
	string.find(hud, "tutorial.Position = UDim2.new(0.5, 0, 1, -72)", 1, true),
	"non-compact tutorial must stay clear of the hero playfield"
)

return true
