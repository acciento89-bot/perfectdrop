--!strict

local Lighting = game:GetService("Lighting")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local CosmeticConfig = require(ReplicatedStorage.PerfectDrop.config.CosmeticConfig)

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

local function configureLighting()
	Lighting.Brightness = 2.1
	Lighting.ClockTime = 17.35
	Lighting.EnvironmentDiffuseScale = 0.55
	Lighting.EnvironmentSpecularScale = 0.45
	Lighting.Ambient = Color3.fromRGB(82, 89, 103)
	Lighting.OutdoorAmbient = Color3.fromRGB(112, 120, 136)

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

	local skylineHeights = { 22, 34, 18, 42, 27, 31, 48, 24, 38, 20 }
	for index, height in skylineHeights do
		local x = -92 + (index - 1) * 20
		part(
			"Skyline",
			Vector3.new(14, height, 14),
			CFrame.new(x, height / 2 - 4, -82),
			Enum.Material.Concrete,
			Color3.fromRGB(44 + index, 50 + index, 62 + index),
			folder
		)
	end

	decorativeHvac(Vector3.new(-88, 0, 88), folder)
	decorativeHvac(Vector3.new(88, 0, 86), folder)

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
		Enum.Material.Metal,
		Color3.fromRGB(49, 58, 72),
		folder
	)

	local trim = part(
		"OperatorPadTrim",
		Vector3.new(12.4, 0.18, 12.4),
		CFrame.new(offset + Vector3.new(15, 1.08, 10)),
		Enum.Material.Neon,
		Color3.fromRGB(255, 153, 51),
		folder
	)
	trim.CanCollide = false
	trim.CanQuery = false

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

	local towerPad = part(
		"TowerPad",
		Vector3.new(18, 1.2, 18),
		CFrame.new(offset + Vector3.new(0, 0.6, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(25, 29, 36),
		folder
	)

	local ring = Instance.new("SelectionBox")
	ring.Name = "TowerPadAccent"
	ring.Adornee = towerPad
	ring.Color3 = Color3.fromRGB(255, 153, 51)
	ring.LineThickness = 0.065
	ring.SurfaceTransparency = 1
	ring.Parent = towerPad

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
	local trim = area:FindFirstChild("OperatorPadTrim")
	local towerPad = area:FindFirstChild("TowerPad")
	if operatorPad and operatorPad:IsA("BasePart") then
		operatorPad.Color = rgb(theme.PadRGB)
	end
	if trim and trim:IsA("BasePart") then
		trim.Color = rgb(theme.AccentRGB)
	end
	if towerPad and towerPad:IsA("BasePart") then
		local accent = towerPad:FindFirstChild("TowerPadAccent")
		if accent and accent:IsA("SelectionBox") then
			accent.Color3 = rgb(theme.AccentRGB)
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
