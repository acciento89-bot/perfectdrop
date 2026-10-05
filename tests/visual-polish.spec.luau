local fs = require("@lune/fs")
local Theme = require("../src/shared/ui/VisualTheme")

local function run()
	assert(Theme.Id == "perfect-drop", "visual theme id mismatch")
	assert(type(Theme.Colors) == "table", "visual theme colors missing")
	assert(type(Theme.Colors.Accent) == "string" and #Theme.Colors.Accent == 7, "accent must be #RRGGBB")
	assert(type(Theme.Colors.Accent2) == "string" and #Theme.Colors.Accent2 == 7, "secondary accent must be #RRGGBB")
	assert(type(Theme.Lighting) == "table" and Theme.Lighting.Brightness > 0, "lighting contract missing")
	assert(type(Theme.Effects) == "table" and Theme.Effects.BloomIntensity >= 0, "effects contract missing")
	assert(
		type(Theme.TargetGuis) == "table" and table.find(Theme.TargetGuis, "PerfectDropHUD") ~= nil,
		"target HUD missing"
	)

	local controller = fs.readFile("src/client/controllers/VisualPolishController.luau")
	assert(string.find(controller, "Atmosphere", 1, true), "visual controller must configure Atmosphere")
	assert(string.find(controller, "BloomEffect", 1, true), "visual controller must configure BloomEffect")
	assert(
		string.find(controller, "ColorCorrectionEffect", 1, true),
		"visual controller must configure color correction"
	)
	assert(string.find(controller, "UIGradient", 1, true), "visual controller must polish native Roblox GUI")
	assert(
		string.find(controller, 'Terrain:FindFirstChildOfClass("Clouds")', 1, true),
		"visual controller must reuse the existing terrain Clouds instance"
	)

	local initSource = fs.readFile("src/client/init.client.luau")
	assert(
		string.find(initSource, "VisualPolishController", 1, true),
		"client init must require visual polish controller"
	)
	assert(
		string.find(initSource, "VisualPolishController.start()", 1, true)
			or string.find(initSource, "VisualPolishController.Start()", 1, true),
		"client init must start visual polish controller"
	)
end

run()
return true
