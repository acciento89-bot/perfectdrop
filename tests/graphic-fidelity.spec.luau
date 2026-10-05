local fs = require("@lune/fs")

local world = fs.readFile("src/server/services/WorldService.luau")
local art = fs.readFile("docs/ART-DIRECTION.md")

assert(string.find(world, 'farSkyline.Name = "ParallaxSkylineFar"', 1, true), "world needs a far parallax skyline layer")
assert(string.find(world, '"GlassTower"', 1, true), "hero skyline needs authored glass towers")
assert(string.find(world, '"AntennaSpire"', 1, true), "skyline needs vertical antenna silhouettes")
assert(string.find(world, '"SunHalo"', 1, true), "sun focal point needs a layered halo")
assert(string.find(world, '"LandingHalo"', 1, true), "tower base needs a premium landing halo")
assert(string.find(world, '"RoofGardenPlanter"', 1, true), "rooftop needs authored foreground dressing")
assert(string.find(art, "parallax skyline", 1, true), "art direction must lock the layered city-depth treatment")

return true
