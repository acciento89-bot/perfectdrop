local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(
	string.find(hud, "status.Position = UDim2.new(0.5, 0, 0, 16)", 1, true),
	"desktop status must be centered instead of biased across the playfield"
)
assert(
	string.find(hud, "quickRail.Position = UDim2.new(1, -18, 0, 18)", 1, true),
	"desktop Shop/Daily rail must use the top-right safe zone"
)
assert(
	not string.find(hud, "quickRail.Position = UDim2.new(1, -266", 1, true),
	"desktop quick actions must not float hundreds of pixels away from the right safe edge"
)
assert(
	string.find(hud, "mobileSummary.Position = UDim2.fromOffset(12, 10)", 1, true),
	"compact status must use a dedicated top-left safe zone"
)
assert(
	string.find(hud, "shopButton.Position = UDim2.new(1, -12, 0, 10)", 1, true),
	"compact Shop button must occupy its own top-right slot"
)
assert(
	string.find(hud, "tutorial.Position = UDim2.new(0.5, 0, 1, -76)", 1, true),
	"compact tutorial must stay out of the top gameplay/readability zone"
)

return true
