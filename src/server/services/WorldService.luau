--!strict

local Lighting = game:GetService("Lighting")
local Players = game:GetService("Players")
local Workspace = game:GetService("Workspace")

local WorldService = {}

local worldFolder: Folder? = nil
local laneByPlayer: { [Player]: number } = {}
local nextLane = 0

local function part(name: string, size: Vector3, cframe: CFrame, material: Enum.Material, color: Color3, parent: Instance): Part
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
		Vector3.new(180, 2, 110),
		CFrame.new(20, -1, 0),
		Enum.Material.Concrete,
		Color3.fromRGB(31, 37, 46),
		folder
	)

	for x = -60, 100, 20 do
		local height = 14 + ((x + 60) % 40)
		part(
			"Skyline",
			Vector3.new(14, height, 12),
			CFrame.new(x, height / 2 - 3, -48),
			Enum.Material.Concrete,
			Color3.fromRGB(43, 50, 62),
			folder
		)
	end

	for _, z in { -53, 53 } do
		part(
			"Rail",
			Vector3.new(180, 2, 0.8),
			CFrame.new(20, 2, z),
			Enum.Material.Metal,
			Color3.fromRGB(78, 89, 104),
			folder
		)
	end

	Lighting.Brightness = 2.2
	Lighting.ClockTime = 17.2
	Lighting.EnvironmentDiffuseScale = 0.55
	Lighting.EnvironmentSpecularScale = 0.45
	Lighting.Ambient = Color3.fromRGB(82, 89, 103)
	Lighting.OutdoorAmbient = Color3.fromRGB(112, 120, 136)

	local atmosphere = Lighting:FindFirstChildOfClass("Atmosphere") or Instance.new("Atmosphere")
	atmosphere.Density = 0.22
	atmosphere.Haze = 1.1
	atmosphere.Color = Color3.fromRGB(205, 215, 232)
	atmosphere.Decay = Color3.fromRGB(105, 119, 143)
	atmosphere.Parent = Lighting
end

function WorldService.Start()
	ensureWorld()
end

function WorldService.GetLane(player: Player): (number, Vector3)
	local current = laneByPlayer[player]
	if current == nil then
		current = nextLane
		nextLane += 1
		laneByPlayer[player] = current
	end
	return current, Vector3.new(current * 38, 0, 0)
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
		Vector3.new(12.4, 0.25, 12.4),
		CFrame.new(offset + Vector3.new(15, 1.12, 10)),
		Enum.Material.Neon,
		Color3.fromRGB(255, 153, 51),
		folder
	)
	trim.CanCollide = false

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
	ring.LineThickness = 0.06
	ring.SurfaceTransparency = 1
	ring.Parent = towerPad

	if player.RespawnLocation ~= spawn then
		player.RespawnLocation = spawn
	end

	return folder, offset
end

function WorldService.PlaceCharacter(player: Player, character: Model)
	local _, offset = WorldService.EnsurePlayerArea(player)
	local root = character:WaitForChild("HumanoidRootPart", 8)
	if root and root:IsA("BasePart") then
		character:PivotTo(CFrame.new(offset + Vector3.new(15, 4, 10), offset + Vector3.new(0, 4, 0)))
	end
end

function WorldService.ReleaseLane(player: Player)
	laneByPlayer[player] = nil
end

Players.PlayerAdded:Connect(function(player)
	WorldService.EnsurePlayerArea(player)
	player.CharacterAdded:Connect(function(character)
		task.defer(WorldService.PlaceCharacter, player, character)
	end)
	if player.Character then
		task.defer(WorldService.PlaceCharacter, player, player.Character)
	end
end)

return WorldService
