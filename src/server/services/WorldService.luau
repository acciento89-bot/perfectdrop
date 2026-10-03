--!strict

local Lighting = game:GetService("Lighting")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local CosmeticConfig = require(ReplicatedStorage.PerfectDrop.config.CosmeticConfig)
local GameConfig = require(ReplicatedStorage.PerfectDrop.config.GameConfig)

local WorldService = {}

local worldFolder: Folder? = nil
local laneByPlayer: { [Player]: number } = {}
local usedLanes: { [number]: boolean } = {}

local function rgb(values): Color3
	return Color3.fromRGB(values[1], values[2], values[3])
end

local function part(
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3,
	parent: Instance
): Part
	local item = Instance.new("Part")
	item.Name = name
	item.Size = size
	item.CFrame = cframe
	item.Anchored = true
	item.Material = material
	item.Color = color
	item.TopSurface = Enum.SurfaceType.Smooth
	item.BottomSurface = Enum.SurfaceType.Smooth
	item.Parent = parent
	return item
end

local function decorativeHvac(position: Vector3, parent: Instance)
	local body = part(
		"HVACBody",
		Vector3.new(8, 3.4, 6),
		CFrame.new(position + Vector3.new(0, 1.7, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(67, 76, 88),
		parent
	)
	body.CanCollide = true
	local top = part(
		"HVACTop",
		Vector3.new(8.4, 0.35, 6.4),
		CFrame.new(position + Vector3.new(0, 3.55, 0)),
		Enum.Material.DiamondPlate,
		Color3.fromRGB(91, 101, 115),
		parent
	)
	top.CanCollide = true
	for index = -2, 2 do
		local slat = part(
			"VentSlat",
			Vector3.new(0.18, 1.6, 4.8),
			CFrame.new(position + Vector3.new(index * 1.2, 1.8, -3.04)),
			Enum.Material.Metal,
			Color3.fromRGB(33, 39, 47),
			parent
		)
		slat.CanCollide = false
	end
end

local function makeDecorative(item: BasePart)
	item.CanCollide = false
	item.CanTouch = false
	item.CanQuery = false
end

local function padTrim(
	parent: Instance,
	prefix: string,
	center: Vector3,
	width: number,
	depth: number,
	y: number,
	accent: Color3
)
	local thickness = 0.18
	local inset = 0.30
	for _, zSign in { -1, 1 } do
		local edge = part(
			prefix .. "Trim",
			Vector3.new(width - inset * 2, 0.12, thickness),
			CFrame.new(center + Vector3.new(0, y, zSign * (depth / 2 - inset))),
			Enum.Material.Neon,
			accent,
			parent
		)
		edge.Transparency = 0.06
		edge:SetAttribute("ArenaAccent", true)
		makeDecorative(edge)
	end
	for _, xSign in { -1, 1 } do
		local edge = part(
			prefix .. "Trim",
			Vector3.new(thickness, 0.12, depth - inset * 2),
			CFrame.new(center + Vector3.new(xSign * (width / 2 - inset), y, 0)),
			Enum.Material.Neon,
			accent,
			parent
		)
		edge.Transparency = 0.06
		edge:SetAttribute("ArenaAccent", true)
		makeDecorative(edge)
	end
end

local function rooftopLight(position: Vector3, accent: Color3, parent: Instance)
	local base = part(
		"RoofLightBase",
		Vector3.new(0.8, 0.7, 0.8),
		CFrame.new(position + Vector3.new(0, 0.35, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(44, 51, 63),
		parent
	)
	makeDecorative(base)

	local cap = part(
		"RoofLight",
		Vector3.new(0.42, 0.22, 0.42),
		CFrame.new(position + Vector3.new(0, 0.82, 0)),
		Enum.Material.Neon,
		accent,
		parent
	)
	makeDecorative(cap)

	local light = Instance.new("PointLight")
	light.Color = accent
	light.Brightness = 1.3
	light.Range = 8
	light.Shadows = false
	light.Parent = cap
end

local function roofPipe(position: Vector3, length: number, parent: Instance)
	local body = part(
		"RoofPipe",
		Vector3.new(length, 0.55, 0.55),
		CFrame.new(position + Vector3.new(0, 0.8, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(91, 96, 103),
		parent
	)
	makeDecorative(body)
	for _, x in { -length / 2 + 0.5, length / 2 - 0.5 } do
		local support = part(
			"PipeSupport",
			Vector3.new(0.4, 1.0, 1.0),
			CFrame.new(position + Vector3.new(x, 0.45, 0)),
			Enum.Material.Metal,
			Color3.fromRGB(54, 60, 69),
			parent
		)
		makeDecorative(support)
	end
end

local function cityBuilding(position: Vector3, size: Vector3, shade: number, parent: Instance)
	local building = part(
		"SkylineBuilding",
		size,
		CFrame.new(position + Vector3.new(0, size.Y / 2, 0)),
		Enum.Material.Concrete,
		Color3.fromRGB(shade, shade + 5, shade + 12),
		parent
	)
	makeDecorative(building)

	local windowRows = math.clamp(math.floor(size.Y / 9), 2, 6)
	for row = 1, windowRows do
		local band = part(
			"WindowBand",
			Vector3.new(math.max(4, size.X - 2.2), 0.32, 0.08),
			CFrame.new(position + Vector3.new(0, row * (size.Y / (windowRows + 1)), -size.Z / 2 - 0.05)),
			Enum.Material.Neon,
			if row % 2 == 0 then Color3.fromRGB(255, 190, 109) else Color3.fromRGB(122, 164, 196),
			parent
		)
		band.Transparency = 0.34
		makeDecorative(band)
	end
end

local function milestoneMarker(offset: Vector3, stage: number, parent: Instance)
	local y = 1.8 + stage * GameConfig.Block.Height
	local bar = part(
		("StageMilestone_%d"):format(stage),
		Vector3.new(3.8, 0.13, 0.22),
		CFrame.new(offset + Vector3.new(-7.2, y, 0)),
		Enum.Material.Neon,
		if stage == GameConfig.Progression.MaxStages then Color3.fromRGB(255, 194, 66) else Color3.fromRGB(89, 180, 198),
		parent
	)
	makeDecorative(bar)

	local billboard = Instance.new("BillboardGui")
	billboard.Name = "StageLabel"
	billboard.AlwaysOnTop = true
	billboard.Size = UDim2.fromOffset(58, 28)
	billboard.StudsOffset = Vector3.new(-1.8, 0.4, 0)
	billboard.Adornee = bar
	billboard.MaxDistance = 90
	billboard.Parent = bar

	local label = Instance.new("TextLabel")
	label.Size = UDim2.fromScale(1, 1)
	label.BackgroundTransparency = 1
	label.Font = Enum.Font.GothamBold
	label.TextScaled = true
	label.TextColor3 = Color3.fromRGB(237, 242, 248)
	label.TextStrokeTransparency = 0.72
	label.Text = tostring(stage)
	label.Parent = billboard
end

local function finishCrown(offset: Vector3, parent: Instance)
	local y = 1.8 + GameConfig.Progression.MaxStages * GameConfig.Block.Height + 1.25
	local accent = Color3.fromRGB(255, 194, 66)

	for _, data in
		{
			{ Vector3.new(12.5, 0.18, 0.28), Vector3.new(0, 0, -6.1) },
			{ Vector3.new(12.5, 0.18, 0.28), Vector3.new(0, 0, 6.1) },
			{ Vector3.new(0.28, 0.18, 12.5), Vector3.new(-6.1, 0, 0) },
			{ Vector3.new(0.28, 0.18, 12.5), Vector3.new(6.1, 0, 0) },
		}
	do
		local bar = part(
			"FinishCrown",
			data[1],
			CFrame.new(offset + Vector3.new(0, y, 0) + data[2]),
			Enum.Material.Neon,
			accent,
			parent
		)
		bar.Transparency = 0.08
		makeDecorative(bar)
	end

	local anchor = part(
		"FinishLabelAnchor",
		Vector3.new(0.2, 0.2, 0.2),
		CFrame.new(offset + Vector3.new(0, y + 2.2, 0)),
		Enum.Material.Neon,
		accent,
		parent
	)
	anchor.Transparency = 1
	makeDecorative(anchor)

	local gui = Instance.new("BillboardGui")
	gui.Name = "FinishLabel"
	gui.Adornee = anchor
	gui.AlwaysOnTop = true
	gui.MaxDistance = 120
	gui.Size = UDim2.fromOffset(150, 54)
	gui.Parent = anchor

	local label = Instance.new("TextLabel")
	label.Size = UDim2.fromScale(1, 1)
	label.BackgroundColor3 = Color3.fromRGB(20, 24, 31)
	label.BackgroundTransparency = 0.12
	label.BorderSizePixel = 0
	label.Font = Enum.Font.GothamBlack
	label.TextScaled = true
	label.TextColor3 = accent
	label.Text = "30 / FINISH"
	label.Parent = gui

	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, 10)
	corner.Parent = label

	local stroke = Instance.new("UIStroke")
	stroke.Color = accent
	stroke.Thickness = 1.4
	stroke.Transparency = 0.18
	stroke.Parent = label
end

local function configureLighting()
	Lighting.Brightness = 2.1
	Lighting.ClockTime = 17.35
	Lighting.EnvironmentDiffuseScale = 0.55
	Lighting.EnvironmentSpecularScale = 0.45
	Lighting.Ambient = Color3.fromRGB(86, 91, 110)
	Lighting.OutdoorAmbient = Color3.fromRGB(123, 126, 145)
	Lighting.ColorShift_Top = Color3.fromRGB(255, 218, 187)
	Lighting.ColorShift_Bottom = Color3.fromRGB(188, 205, 230)
	Lighting.FogColor = Color3.fromRGB(149, 164, 190)
	Lighting.FogStart = 145
	Lighting.FogEnd = 430

	local atmosphere = Lighting:FindFirstChildOfClass("Atmosphere") or Instance.new("Atmosphere")
	atmosphere.Density = 0.22
	atmosphere.Haze = 1.15
	atmosphere.Color = Color3.fromRGB(205, 215, 232)
	atmosphere.Decay = Color3.fromRGB(105, 119, 143)
	atmosphere.Parent = Lighting

	local bloom = Lighting:FindFirstChildOfClass("BloomEffect") or Instance.new("BloomEffect")
	bloom.Intensity = 0.22
	bloom.Size = 18
	bloom.Threshold = 1.6
	bloom.Parent = Lighting

	local colorCorrection = Lighting:FindFirstChildOfClass("ColorCorrectionEffect")
		or Instance.new("ColorCorrectionEffect")
	colorCorrection.Brightness = 0.01
	colorCorrection.Contrast = 0.08
	colorCorrection.Saturation = -0.05
	colorCorrection.TintColor = Color3.fromRGB(255, 238, 223)
	colorCorrection.Parent = Lighting

	local sunRays = Lighting:FindFirstChildOfClass("SunRaysEffect") or Instance.new("SunRaysEffect")
	sunRays.Intensity = 0.035
	sunRays.Spread = 0.6
	sunRays.Parent = Lighting

	Lighting.GlobalShadows = true
	Lighting.ShadowSoftness = 0.28
	Lighting.ExposureCompensation = 0.05

	local clouds = Workspace.Terrain:FindFirstChildOfClass("Clouds") or Instance.new("Clouds")
	clouds.Cover = 0.28
	clouds.Density = 0.34
	clouds.Color = Color3.fromRGB(235, 228, 220)
	clouds.Parent = Workspace.Terrain
end

local function ensureWorld()
	if worldFolder and worldFolder.Parent then
		return
	end

	local existing = Workspace:FindFirstChild("PerfectDropWorld")
	if existing then
		existing:Destroy()
	end

	local folder = Instance.new("Folder")
	folder.Name = "PerfectDropWorld"
	folder.Parent = Workspace
	worldFolder = folder

	part(
		"Rooftop",
		Vector3.new(210, 2, 180),
		CFrame.new(0, -1, 22),
		Enum.Material.Concrete,
		Color3.fromRGB(31, 37, 46),
		folder
	)

	for _, z in { -68, 112 } do
		part(
			"Rail",
			Vector3.new(210, 2.4, 0.8),
			CFrame.new(0, 2, z),
			Enum.Material.Metal,
			Color3.fromRGB(78, 89, 104),
			folder
		)
	end
	for _, x in { -105, 105 } do
		part(
			"Rail",
			Vector3.new(0.8, 2.4, 180),
			CFrame.new(x, 2, 22),
			Enum.Material.Metal,
			Color3.fromRGB(78, 89, 104),
			folder
		)
	end

	for x = -92, 92, 18 do
		local seam = part(
			"RoofSeam",
			Vector3.new(0.12, 0.025, 168),
			CFrame.new(x, 0.02, 22),
			Enum.Material.Metal,
			Color3.fromRGB(45, 51, 61),
			folder
		)
		seam.Transparency = 0.28
		makeDecorative(seam)
	end

	for z = -54, 98, 19 do
		local seam = part(
			"RoofSeam",
			Vector3.new(198, 0.025, 0.12),
			CFrame.new(0, 0.025, z),
			Enum.Material.Metal,
			Color3.fromRGB(45, 51, 61),
			folder
		)
		seam.Transparency = 0.28
		makeDecorative(seam)
	end

	local skylineHeights = { 25, 36, 20, 45, 29, 34, 52, 27, 41, 23, 32 }
	for index, height in skylineHeights do
		local x = -96 + (index - 1) * 19
		cityBuilding(
			Vector3.new(x, -6, -88 - (index % 3) * 5),
			Vector3.new(12 + (index % 3) * 2, height, 12 + (index % 2) * 3),
			38 + index,
			folder
		)
	end

	local sun = part(
		"SunDisc",
		Vector3.new(19, 19, 19),
		CFrame.new(56, 50, -145),
		Enum.Material.Neon,
		Color3.fromRGB(255, 180, 92),
		folder
	)
	sun.Shape = Enum.PartType.Ball
	sun.Transparency = 0.08
	makeDecorative(sun)

	local sunGlow = part(
		"SunGlow",
		Vector3.new(27, 27, 27),
		CFrame.new(56, 50, -144.5),
		Enum.Material.Neon,
		Color3.fromRGB(255, 196, 121),
		folder
	)
	sunGlow.Shape = Enum.PartType.Ball
	sunGlow.Transparency = 0.78
	makeDecorative(sunGlow)

	for index = 1, 6 do
		local z = -42 + (index - 1) * 25
		cityBuilding(
			Vector3.new(-122 - (index % 2) * 5, -8, z),
			Vector3.new(14, 24 + index * 4, 14 + (index % 2) * 3),
			37 + index,
			folder
		)
	end

	for index = 1, 5 do
		local z = -35 + (index - 1) * 28
		cityBuilding(
			Vector3.new(124 + (index % 2) * 4, -10, z),
			Vector3.new(15, 26 + index * 3, 13),
			40 + index,
			folder
		)
	end

	decorativeHvac(Vector3.new(-88, 0, 88), folder)
	decorativeHvac(Vector3.new(88, 0, 86), folder)
	roofPipe(Vector3.new(-72, 0, 64), 22, folder)
	roofPipe(Vector3.new(63, 0, 70), 26, folder)

	for _, lightPosition in
		{
			Vector3.new(-96, 0, -58),
			Vector3.new(96, 0, -58),
			Vector3.new(-96, 0, 102),
			Vector3.new(96, 0, 102),
		}
	do
		rooftopLight(lightPosition, Color3.fromRGB(255, 172, 74), folder)
	end

	local signPost = part(
		"PerfectDropSignPost",
		Vector3.new(1, 9, 1),
		CFrame.new(-92, 4.5, -55),
		Enum.Material.Metal,
		Color3.fromRGB(58, 66, 78),
		folder
	)
	signPost.CanCollide = false
	local sign = part(
		"PerfectDropSign",
		Vector3.new(18, 5, 0.7),
		CFrame.new(-92, 9, -55),
		Enum.Material.Metal,
		Color3.fromRGB(24, 29, 37),
		folder
	)
	sign.CanCollide = false
	local surface = Instance.new("SurfaceGui")
	surface.Face = Enum.NormalId.Front
	surface.SizingMode = Enum.SurfaceGuiSizingMode.PixelsPerStud
	surface.PixelsPerStud = 40
	surface.Parent = sign
	local label = Instance.new("TextLabel")
	label.Size = UDim2.fromScale(1, 1)
	label.BackgroundTransparency = 1
	label.Font = Enum.Font.GothamBlack
	label.Text = "PERFECT\nDROP"
	label.TextColor3 = Color3.fromRGB(255, 171, 64)
	label.TextScaled = true
	label.Parent = surface

	configureLighting()
end

local function offsetForLane(lane: number): Vector3
	local column = lane % 3
	local row = math.floor(lane / 3)
	return Vector3.new((column - 1) * 54, 0, row * 54 - 4)
end

function WorldService.Start()
	ensureWorld()

	local function attachPlayer(player: Player)
		WorldService.EnsurePlayerArea(player)
		player.CharacterAdded:Connect(function(character)
			task.defer(WorldService.PlaceCharacter, player, character)
		end)
		if player.Character then
			task.defer(WorldService.PlaceCharacter, player, player.Character)
		end
	end

	Players.PlayerAdded:Connect(attachPlayer)
	for _, player in Players:GetPlayers() do
		task.defer(attachPlayer, player)
	end
end

function WorldService.GetLane(player: Player): (number, Vector3)
	local current = laneByPlayer[player]
	if current ~= nil then
		return current, offsetForLane(current)
	end

	local lane = 0
	while usedLanes[lane] do
		lane += 1
	end
	usedLanes[lane] = true
	laneByPlayer[player] = lane
	return lane, offsetForLane(lane)
end

function WorldService.EnsurePlayerArea(player: Player): (Folder, Vector3)
	ensureWorld()
	local lane, offset = WorldService.GetLane(player)
	local folderName = tostring(player.UserId)
	local existing = worldFolder and worldFolder:FindFirstChild(folderName)
	if existing and existing:IsA("Folder") then
		return existing, offset
	end

	assert(worldFolder, "world folder unavailable")
	local folder = Instance.new("Folder")
	folder.Name = folderName
	folder:SetAttribute("Lane", lane)
	folder.Parent = worldFolder

	part(
		"OperatorPad",
		Vector3.new(12, 1, 12),
		CFrame.new(offset + Vector3.new(15, 0.5, 10)),
		Enum.Material.DiamondPlate,
		Color3.fromRGB(49, 58, 72),
		folder
	)

	padTrim(folder, "OperatorPad", offset + Vector3.new(15, 0, 10), 12, 12, 1.04, Color3.fromRGB(255, 153, 51))

	local operatorInset = part(
		"OperatorInset",
		Vector3.new(10.4, 0.10, 10.4),
		CFrame.new(offset + Vector3.new(15, 1.05, 10)),
		Enum.Material.Slate,
		Color3.fromRGB(37, 45, 57),
		folder
	)
	makeDecorative(operatorInset)

	local spawn = Instance.new("SpawnLocation")
	spawn.Name = "PlayerSpawn"
	spawn.Size = Vector3.new(6, 1, 6)
	spawn.CFrame = CFrame.new(offset + Vector3.new(15, 1.7, 10))
	spawn.Anchored = true
	spawn.CanCollide = true
	spawn.Neutral = true
	spawn.Transparency = 1
	spawn.Duration = 0
	spawn.Parent = folder

	part(
		"TowerPad",
		Vector3.new(18, 1.2, 18),
		CFrame.new(offset + Vector3.new(0, 0.6, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(25, 29, 36),
		folder
	)

	padTrim(folder, "TowerPad", offset, 18, 18, 1.22, Color3.fromRGB(255, 153, 51))

	local inset = part(
		"TowerInset",
		Vector3.new(15.6, 0.12, 15.6),
		CFrame.new(offset + Vector3.new(0, 1.24, 0)),
		Enum.Material.Slate,
		Color3.fromRGB(17, 21, 28),
		folder
	)
	makeDecorative(inset)

	for _, corner in
		{
			Vector3.new(-8.0, 0, -8.0),
			Vector3.new(8.0, 0, -8.0),
			Vector3.new(-8.0, 0, 8.0),
			Vector3.new(8.0, 0, 8.0),
		}
	do
		rooftopLight(offset + corner, Color3.fromRGB(255, 153, 51), folder)
	end

	for _, stage in { 10, 20, 30 } do
		milestoneMarker(offset, stage, folder)
	end
	finishCrown(offset, folder)

	local towerSign = part(
		"TowerSignAnchor",
		Vector3.new(0.2, 0.2, 0.2),
		CFrame.new(offset + Vector3.new(8.5, 4.8, -6.8)),
		Enum.Material.Metal,
		Color3.fromRGB(42, 48, 58),
		folder
	)
	towerSign.Transparency = 1
	makeDecorative(towerSign)

	local towerGui = Instance.new("BillboardGui")
	towerGui.Name = "TowerInfo"
	towerGui.Adornee = towerSign
	towerGui.AlwaysOnTop = false
	towerGui.MaxDistance = 70
	towerGui.Size = UDim2.fromOffset(150, 62)
	towerGui.Parent = towerSign

	local towerLabel = Instance.new("TextLabel")
	towerLabel.Size = UDim2.fromScale(1, 1)
	towerLabel.BackgroundColor3 = Color3.fromRGB(22, 27, 34)
	towerLabel.BackgroundTransparency = 0.08
	towerLabel.BorderSizePixel = 0
	towerLabel.Font = Enum.Font.GothamBold
	towerLabel.TextScaled = true
	towerLabel.TextWrapped = true
	towerLabel.TextColor3 = Color3.fromRGB(235, 240, 248)
	towerLabel.Text = "30 STAGES\nREACH THE CROWN"
	towerLabel.Parent = towerGui

	local towerCorner = Instance.new("UICorner")
	towerCorner.CornerRadius = UDim.new(0, 10)
	towerCorner.Parent = towerLabel

	local towerStroke = Instance.new("UIStroke")
	towerStroke.Color = Color3.fromRGB(255, 153, 51)
	towerStroke.Thickness = 1.2
	towerStroke.Transparency = 0.25
	towerStroke.Parent = towerLabel

	if player.RespawnLocation ~= spawn then
		player.RespawnLocation = spawn
	end

	return folder, offset
end

function WorldService.ApplyArenaTheme(player: Player)
	local area = worldFolder and worldFolder:FindFirstChild(tostring(player.UserId))
	if not area then
		return
	end
	local themeId = player:GetAttribute("PDArenaTheme")
	if type(themeId) ~= "string" then
		themeId = CosmeticConfig.DefaultArenaTheme
	end
	local theme = CosmeticConfig.ArenaThemes[themeId] or CosmeticConfig.ArenaThemes[CosmeticConfig.DefaultArenaTheme]
	local operatorPad = area:FindFirstChild("OperatorPad")
	if operatorPad and operatorPad:IsA("BasePart") then
		operatorPad.Color = rgb(theme.PadRGB)
	end
	local accent = rgb(theme.AccentRGB)
	for _, descendant in area:GetDescendants() do
		if descendant:IsA("BasePart") and descendant:GetAttribute("ArenaAccent") == true then
			descendant.Color = accent
		end
	end
end

function WorldService.PlaceCharacter(player: Player, character: Model)
	local _, offset = WorldService.EnsurePlayerArea(player)
	local root = character:WaitForChild("HumanoidRootPart", 8)
	if root and root:IsA("BasePart") then
		character:PivotTo(CFrame.new(offset + Vector3.new(15, 4, 10), offset + Vector3.new(0, 4, 0)))
	end
end

function WorldService.ReleaseLane(player: Player)
	local lane = laneByPlayer[player]
	if lane ~= nil then
		usedLanes[lane] = nil
	end
	laneByPlayer[player] = nil
end

return WorldService
