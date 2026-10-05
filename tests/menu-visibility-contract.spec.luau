local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")
local art = fs.readFile("docs/ART-DIRECTION.md")

assert(string.find(hud, 'conceptShowcase.Visible = false', 1, true), "concept montage must stay hidden in gameplay")
assert(not string.find(hud, 'conceptShowcase.Visible = not compact', 1, true), "desktop must not force the concept montage visible")
assert(string.find(hud, 'shopPanel.Visible = false', 1, true), "shop must be hidden until opened")
assert(string.find(hud, 'dailyPanel.Name = "DailyRewardsPanel"', 1, true), "daily rewards need a dedicated panel")
assert(string.find(hud, 'dailyPanel.Visible = false', 1, true), "daily rewards must be hidden by default")
assert(string.find(hud, 'resultPanel.Visible = false', 1, true), "result/revive panel must be hidden until failure or completion")
assert(string.find(hud, 'setDailyVisible(true)', 1, true), "Daily quick action must open only the Daily panel")
assert(string.find(art, "feature montage", 1, true), "art direction must define the concept image as a feature montage")

return true
