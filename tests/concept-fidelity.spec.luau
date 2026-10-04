local fs = require("@lune/fs")
local Theme = require("../src/shared/ui/VisualTheme")

local hud = fs.readFile("src/client/controllers/HudController.luau")
local world = fs.readFile("src/server/services/WorldService.luau")

assert(Theme.Lighting.ClockTime <= 18.5, "Perfect Drop concept needs a readable sunset skyline")
assert(Theme.Lighting.Brightness >= 2.5, "Perfect Drop skyline must remain bright enough for mobile")
assert(Theme.Lighting.ExposureCompensation >= 0.12, "Perfect Drop needs positive sunset exposure")
assert(string.find(hud, 'logo.Name = "PerfectDropLogo"', 1, true), "HUD needs a branded Perfect Drop logo")
assert(string.find(hud, 'quickRail.Name = "ConceptQuickRail"', 1, true), "HUD needs compact concept quick actions")
assert(string.find(world, 'heroLeft.Name = "HeroSkylineLeft"', 1, true), "world needs a left hero skyline cluster")
assert(string.find(world, 'heroRight.Name = "HeroSkylineRight"', 1, true), "world needs a right hero skyline cluster")
assert(string.find(world, '"SkySunDisc"', 1, true), "world needs the sunset focal disc")

return true
