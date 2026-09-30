--!strict

local Debris = game:GetService("Debris")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local CosmeticConfig = require(ReplicatedStorage.PerfectDrop.config.CosmeticConfig)
local GameConfig = require(ReplicatedStorage.PerfectDrop.config.GameConfig)
local DifficultyRules = require(ReplicatedStorage.PerfectDrop.game.DifficultyRules)
local DropRules = require(ReplicatedStorage.PerfectDrop.game.DropRules)
local RoundStateRules = require(ReplicatedStorage.PerfectDrop.game.RoundStateRules)
local ScoreRules = require(ReplicatedStorage.PerfectDrop.game.ScoreRules)
local StageRules = require(ReplicatedStorage.PerfectDrop.game.StageRules)
local RateLimiter = require(ReplicatedStorage.PerfectDrop.security.RateLimiter)

local ProfileService = require(script.Parent.ProfileService)
local WorldService = require(script.Parent.WorldService)

type Session = {
	player: Player,
	folder: Folder,
	origin: Vector3,
	state: string,
	height: number,
	score: number,
	combo: number,
	revivesUsed: number,
	assisted: boolean,
	topBlock: Part,
	movingBlock: Part?,
	moveStartedAt: number,
	axis: "X" | "Z",
}

local RoundService = {}
local sessions: { [Player]: Session } = {}
local stateChanged: RemoteEvent

local dropLimiter = RateLimiter.new(0.10)
local retryLimiter = RateLimiter.new(0.35)
local reviveLimiter = RateLimiter.new(0.35)

local function rgb(values): Color3
	return Color3.fromRGB(values[1], values[2], values[3])
end

local function themeFor(player: Player)
	local profile = ProfileService.Get(player)
	local themeId = if profile then profile.EquippedBlockTheme else CosmeticConfig.DefaultBlockTheme
	return CosmeticConfig.BlockThemes[themeId] or CosmeticConfig.BlockThemes[CosmeticConfig.DefaultBlockTheme]
end

local function colorForGrade(grade: string): Color3
	if grade == "PERFECT" then
		return Color3.fromRGB(255, 194, 66)
	elseif grade == "GOOD" then
		return Color3.fromRGB(69, 206, 159)
	elseif grade == "SHIELD" then
		return Color3.fromRGB(137, 121, 255)
	end
	return Color3.fromRGB(241, 86, 91)
end

local function snapshot(session: Session)
	local meta = ProfileService.GetSnapshot(session.player)
	return {
		state = session.state,
		height = session.height,
		score = session.score,
		combo = session.combo,
		revivesUsed = session.revivesUsed,
		assisted = session.assisted,
		axis = session.axis,
		speed = DifficultyRules.speedForHeight(
			session.height,
			GameConfig.Block.InitialSpeed,
			GameConfig.Block.SpeedPerDrop,
			GameConfig.Block.MaxSpeed
		),
		coins = if meta then meta.coins else 0,
		bestScore = if meta then meta.bestScore else 0,
		bestHeight = if meta then meta.bestHeight else 0,
		playerLevel = if meta then meta.playerLevel else 1,
		towersCompleted = if meta then meta.towersCompleted else 0,
		maxStages = GameConfig.Progression.MaxStages,
		revives = if meta then meta.revives else 0,
		perfectShields = if meta then meta.perfectShields else 0,
	}
end

local function emit(session: Session, grade: string?)
	stateChanged:FireClient(session.player, snapshot(session), grade)
end

local function makeBlock(
	name: string,
	size: Vector3,
	position: Vector3,
	color: Color3,
	parent: Instance,
	anchored: boolean
): Part
	local block = Instance.new("Part")
	block.Name = name
	block.Size = size
	block.Position = position
	block.Anchored = anchored
	block.Material = Enum.Material.SmoothPlastic
	block.Color = color
	block.TopSurface = Enum.SurfaceType.Smooth
	block.BottomSurface = Enum.SurfaceType.Smooth
	block.CustomPhysicalProperties = PhysicalProperties.new(0.7, 0.35, 0.1)
	block.CastShadow = true
	block.Parent = parent

	if name ~= "Overhang" then
		local surface = Instance.new("SurfaceGui")
		surface.Name = "TopFinish"
		surface.Face = Enum.NormalId.Top
		surface.SizingMode = Enum.SurfaceGuiSizingMode.FixedSize
		surface.CanvasSize = Vector2.new(320, 320)
		surface.LightInfluence = 0.45
		surface.Parent = block

		local inset = Instance.new("Frame")
		inset.Position = UDim2.fromOffset(10, 10)
		inset.Size = UDim2.new(1, -20, 1, -20)
		inset.BackgroundColor3 = color:Lerp(Color3.new(1, 1, 1), 0.08)
		inset.BackgroundTransparency = 0.55
		inset.BorderSizePixel = 0
		inset.Parent = surface

		local insetCorner = Instance.new("UICorner")
		insetCorner.CornerRadius = UDim.new(0, 18)
		insetCorner.Parent = inset

		local insetStroke = Instance.new("UIStroke")
		insetStroke.Color = Color3.fromRGB(235, 240, 248)
		insetStroke.Thickness = 3
		insetStroke.Transparency = 0.58
		insetStroke.Parent = inset

		local axisLineX = Instance.new("Frame")
		axisLineX.AnchorPoint = Vector2.new(0.5, 0.5)
		axisLineX.Position = UDim2.fromScale(0.5, 0.5)
		axisLineX.Size = UDim2.new(0, 2, 1, -56)
		axisLineX.BackgroundColor3 = Color3.fromRGB(239, 243, 249)
		axisLineX.BackgroundTransparency = 0.86
		axisLineX.BorderSizePixel = 0
		axisLineX.Parent = surface

		local axisLineZ = axisLineX:Clone()
		axisLineZ.Size = UDim2.new(1, -56, 0, 2)
		axisLineZ.Parent = surface
	end

	local edge = Instance.new("SelectionBox")
	edge.Name = "Edge"
	edge.Adornee = block
	edge.Color3 = Color3.fromRGB(225, 231, 240)
	edge.LineThickness = 0.035
	edge.SurfaceTransparency = 1
	edge.Parent = block
	return block
end

local function cleanRun(folder: Folder)
	local old = folder:FindFirstChild("Run")
	if old then
		old:Destroy()
	end
end

local function blockAxisValues(block: BasePart, axis: "X" | "Z"): (number, number)
	if axis == "X" then
		return block.Position.X, block.Size.X
	end
	return block.Position.Z, block.Size.Z
end

local function setAxis(position: Vector3, axis: "X" | "Z", value: number): Vector3
	if axis == "X" then
		return Vector3.new(value, position.Y, position.Z)
	end
	return Vector3.new(position.X, position.Y, value)
end

local function resizeAxis(size: Vector3, axis: "X" | "Z", value: number): Vector3
	if axis == "X" then
		return Vector3.new(value, size.Y, size.Z)
	end
	return Vector3.new(size.X, size.Y, value)
end

local function clearLandingGuide(session: Session)
	local guide = session.folder:FindFirstChild("LandingGuide")
	if guide then
		guide:Destroy()
	end
end

local function createLandingGuide(session: Session, axis: "X" | "Z")
	clearLandingGuide(session)
	local top = session.topBlock
	local size
	if axis == "X" then
		size = Vector3.new(GameConfig.Block.PerfectTolerance * 2, 0.06, math.max(0.6, top.Size.Z - 0.36))
	else
		size = Vector3.new(math.max(0.6, top.Size.X - 0.36), 0.06, GameConfig.Block.PerfectTolerance * 2)
	end

	local guide = Instance.new("Part")
	guide.Name = "LandingGuide"
	guide.Size = size
	guide.CFrame = top.CFrame + Vector3.new(0, top.Size.Y / 2 + 0.07, 0)
	guide.Anchored = true
	guide.CanCollide = false
	guide.CanTouch = false
	guide.CanQuery = false
	guide.Material = Enum.Material.Neon
	guide.Color = Color3.fromRGB(255, 194, 66)
	guide.Transparency = 0.30
	guide.CastShadow = false
	guide.Parent = session.folder

	local light = Instance.new("SurfaceLight")
	light.Face = Enum.NormalId.Top
	light.Color = guide.Color
	light.Brightness = 0.45
	light.Range = 4
	light.Angle = 90
	light.Shadows = false
	light.Parent = guide
end

local function createMoving(session: Session)
	local axis = DifficultyRules.axisForHeight(session.height)
	session.axis = axis
	createLandingGuide(session, axis)

	local top = session.topBlock
	local y = top.Position.Y + GameConfig.Block.Height
	local position = Vector3.new(top.Position.X, y, top.Position.Z)
	local start = if axis == "X"
		then top.Position.X - GameConfig.Block.TravelHalfDistance
		else top.Position.Z - GameConfig.Block.TravelHalfDistance
	position = setAxis(position, axis, start)

	local theme = themeFor(session.player)
	local moving = makeBlock("MovingBlock", top.Size, position, rgb(theme.MovingRGB), top.Parent, true)
	moving:SetAttribute("Active", true)

	local highlight = Instance.new("Highlight")
	highlight.Name = "ActiveGlow"
	highlight.Adornee = moving
	highlight.FillTransparency = 0.92
	highlight.OutlineColor = Color3.fromRGB(255, 194, 66)
	highlight.OutlineTransparency = 0.2
	highlight.DepthMode = Enum.HighlightDepthMode.Occluded
	highlight.Parent = moving

	session.movingBlock = moving
	session.moveStartedAt = os.clock()
end

local function startRun(player: Player)
	local profile = ProfileService.WaitForProfile(player)
	if not profile or not player.Parent then
		return
	end

	local area, origin = WorldService.EnsurePlayerArea(player)
	WorldService.ApplyArenaTheme(player)
	cleanRun(area)

	local run = Instance.new("Folder")
	run.Name = "Run"
	run.Parent = area

	local theme = themeFor(player)
	local base = makeBlock(
		"Stack_0",
		Vector3.new(GameConfig.Block.BaseSize, GameConfig.Block.Height, GameConfig.Block.BaseSize),
		origin + Vector3.new(0, 1.2 + GameConfig.Block.Height / 2, 0),
		rgb(theme.BaseRGB),
		run,
		true
	)

	local session: Session = {
		player = player,
		folder = run,
		origin = origin,
		state = "ACTIVE",
		height = 0,
		score = 0,
		combo = 0,
		revivesUsed = 0,
		assisted = false,
		topBlock = base,
		movingBlock = nil,
		moveStartedAt = 0,
		axis = "X",
	}
	sessions[player] = session

	player:SetAttribute("PDState", "ACTIVE")
	player:SetAttribute("PDHeight", 0)
	player:SetAttribute("PDScore", 0)
	player:SetAttribute("PDCombo", 0)
	player:SetAttribute("PDAssisted", false)

	createMoving(session)
	emit(session)
end

local function createOverhang(
	session: Session,
	original: Part,
	overlapCenter: number,
	overlapSize: number,
	offset: number
)
	local axis = session.axis
	local _, movingSize = blockAxisValues(original, axis)
	local lost = movingSize - overlapSize
	if lost <= 0.02 then
		return
	end

	local overlapEdge = if offset > 0 then overlapCenter + overlapSize / 2 else overlapCenter - overlapSize / 2
	local overhangCenter = if offset > 0 then overlapEdge + lost / 2 else overlapEdge - lost / 2
	local overhangPosition = setAxis(original.Position, axis, overhangCenter)
	local overhangSize = resizeAxis(original.Size, axis, lost)

	local falling =
		makeBlock("Overhang", overhangSize, overhangPosition, Color3.fromRGB(112, 123, 143), session.folder, false)
	falling.AssemblyLinearVelocity =
		Vector3.new(if axis == "X" then offset * 0.65 else 0, -3, if axis == "Z" then offset * 0.65 else 0)
	falling.AssemblyAngularVelocity = Vector3.new(0.3, 0.15, -0.25)
	Debris:AddItem(falling, 3)
end

local function updatePlayerAttributes(session: Session)
	local player = session.player
	player:SetAttribute("PDState", session.state)
	player:SetAttribute("PDHeight", session.height)
	player:SetAttribute("PDScore", session.score)
	player:SetAttribute("PDCombo", session.combo)
	player:SetAttribute("PDAssisted", session.assisted)
end

local function motionCenterAt(session: Session, sampleTime: number): number
	local speed = DifficultyRules.speedForHeight(
		session.height,
		GameConfig.Block.InitialSpeed,
		GameConfig.Block.SpeedPerDrop,
		GameConfig.Block.MaxSpeed
	)
	local half = GameConfig.Block.TravelHalfDistance
	local elapsed = math.max(0, sampleTime - session.moveStartedAt)
	local cyclePosition = (elapsed * speed / (2 * half)) % 2
	local alpha = if cyclePosition <= 1 then cyclePosition else 2 - cyclePosition
	local delta = -half + alpha * (2 * half)
	local topCenter = if session.axis == "X" then session.topBlock.Position.X else session.topBlock.Position.Z
	return topCenter + delta
end

local function gradeRank(grade: string): number
	if grade == "PERFECT" then
		return 3
	elseif grade == "GOOD" then
		return 2
	end
	return 1
end

local function handleDrop(player: Player)
	local session = sessions[player]
	if not session or not RoundStateRules.canDrop(session.state) then
		return { ok = false, reason = "invalid_state" }
	end
	if not dropLimiter:Allow(player) then
		return { ok = false, reason = "rate_limited" }
	end

	local moving = session.movingBlock
	if not moving or not moving.Parent then
		return { ok = false, reason = "moving_block_missing" }
	end

	local previousCenter, previousSize = blockAxisValues(session.topBlock, session.axis)
	local _, movingSize = blockAxisValues(moving, session.axis)
	local now = os.clock()
	local currentCenter = motionCenterAt(session, now)
	local networkGrace = math.clamp(0.035 + player:GetNetworkPing() * 0.5, 0.035, 0.13)
	local rewindCenter = motionCenterAt(session, now - networkGrace)

	local currentResult = DropRules.evaluate(
		previousCenter,
		previousSize,
		currentCenter,
		movingSize,
		GameConfig.Block.PerfectTolerance,
		GameConfig.Block.MinimumFootprint
	)
	local rewindResult = DropRules.evaluate(
		previousCenter,
		previousSize,
		rewindCenter,
		movingSize,
		GameConfig.Block.PerfectTolerance,
		GameConfig.Block.MinimumFootprint
	)

	local result = currentResult
	local evaluatedCenter = currentCenter
	if
		gradeRank(rewindResult.grade) > gradeRank(currentResult.grade)
		or (
			gradeRank(rewindResult.grade) == gradeRank(currentResult.grade)
			and math.abs(rewindResult.offset) < math.abs(currentResult.offset)
		)
	then
		result = rewindResult
		evaluatedCenter = rewindCenter
	end

	-- Use the server-selected compensated position for all cut geometry. The client
	-- still never submits overlap, grade or score.
	moving.Position = setAxis(moving.Position, session.axis, evaluatedCenter)
	moving:SetAttribute("Active", false)
	session.movingBlock = nil
	clearLandingGuide(session)

	local shielded = false
	if result.grade == "MISS" and ProfileService.ConsumePerfectShield(player) then
		shielded = true
		session.assisted = true
		result = {
			grade = "GOOD",
			offset = 0,
			overlapSize = previousSize,
			overlapCenter = previousCenter,
			lostSize = 0,
		}
	end

	if result.grade == "MISS" then
		session.state = "FAILED"
		session.combo = 0
		moving.Name = "MissedBlock"
		moving.Anchored = false
		moving.Color = colorForGrade("MISS")
		moving.AssemblyLinearVelocity = Vector3.new(0, -4, 0)
		Debris:AddItem(moving, 4)
		updatePlayerAttributes(session)
		emit(session, "MISS")
		return { ok = true, grade = "MISS", state = snapshot(session) }
	end

	if not shielded then
		createOverhang(session, moving, result.overlapCenter, result.overlapSize, result.offset)
	end

	local acceptedCenter = if result.grade == "PERFECT" or shielded then previousCenter else result.overlapCenter
	moving.Position = setAxis(moving.Position, session.axis, acceptedCenter)
	moving.Size = resizeAxis(moving.Size, session.axis, result.overlapSize)
	moving.Size =
		StageRules.shrinkFootprint(moving.Size, GameConfig.Block.StageShrinkPerDrop, GameConfig.Block.MinimumFootprint)
	local acceptedStage = session.height + 1
	moving.Name = ("Stack_%d"):format(acceptedStage)
	local theme = themeFor(player)
	local baseColor = rgb(theme.BaseRGB)
	local movingColor = rgb(theme.MovingRGB)
	local towerAlpha = math.clamp(0.18 + acceptedStage / GameConfig.Progression.MaxStages * 0.52, 0.18, 0.70)
	moving.Color = baseColor:Lerp(movingColor, towerAlpha)
	moving.Anchored = true

	local activeGlow = moving:FindFirstChild("ActiveGlow")
	if activeGlow then
		activeGlow:Destroy()
	end
	local topFinish = moving:FindFirstChild("TopFinish")
	if topFinish and topFinish:IsA("SurfaceGui") then
		local inset = topFinish:FindFirstChildOfClass("Frame")
		if inset then
			inset.BackgroundColor3 = moving.Color:Lerp(Color3.new(1, 1, 1), 0.08)
		end
	end

	local gradeEdge = moving:FindFirstChild("Edge")
	if gradeEdge and gradeEdge:IsA("SelectionBox") then
		gradeEdge.Color3 = colorForGrade(if shielded then "SHIELD" else result.grade)
		gradeEdge.LineThickness = if result.grade == "PERFECT" then 0.06 else 0.04
	end

	local points, nextCombo = ScoreRules.pointsForDrop(
		result.grade,
		session.combo,
		GameConfig.Scoring.GoodBase,
		GameConfig.Scoring.PerfectBase,
		GameConfig.Scoring.PerfectComboCap,
		GameConfig.Scoring.ComboStep
	)

	session.height += 1
	session.score += points
	session.combo = nextCombo
	session.topBlock = moving
	local rewardCoins = ProfileService.RecordDrop(
		player,
		result.grade,
		session.score,
		session.height,
		session.combo,
		not session.assisted
	)

	local completed = StageRules.isTowerComplete(session.height, GameConfig.Progression.MaxStages)
	local completionCoins = 0
	if completed then
		session.state = "COMPLETED"
		completionCoins = ProfileService.CompleteTower(player, not session.assisted)
	end

	updatePlayerAttributes(session)
	emit(session, if completed then "TOWER COMPLETE" elseif shielded then "SHIELD" else result.grade)

	if not completed then
		task.delay(0.1, function()
			if sessions[player] == session and session.state == "ACTIVE" then
				createMoving(session)
				emit(session)
			end
		end)
	end

	return {
		ok = true,
		grade = if shielded then "SHIELD" else result.grade,
		completed = completed,
		points = points,
		rewardCoins = rewardCoins,
		completionCoins = completionCoins,
		state = snapshot(session),
	}
end

local function handleRevive(player: Player)
	local session = sessions[player]
	if
		not session or not RoundStateRules.canRevive(session.state, session.revivesUsed, GameConfig.Round.MaxRevives)
	then
		return { ok = false, reason = "invalid_state" }
	end
	if not reviveLimiter:Allow(player) then
		return { ok = false, reason = "rate_limited" }
	end
	if not ProfileService.ConsumeRevive(player) then
		return { ok = false, reason = "no_revive" }
	end

	session.revivesUsed += 1
	session.assisted = true
	session.state = "ACTIVE"
	updatePlayerAttributes(session)
	createMoving(session)
	emit(session, "REVIVE")
	return { ok = true, state = snapshot(session) }
end

local function updateMoving(session: Session)
	local moving = session.movingBlock
	if session.state ~= "ACTIVE" or not moving or not moving.Parent then
		return
	end

	local center = motionCenterAt(session, os.clock())
	moving.Position = setAxis(moving.Position, session.axis, center)
end

function RoundService.Start(remotes: Folder)
	stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	local dropRequest = remotes:WaitForChild("DropRequest") :: RemoteFunction
	local retryRun = remotes:WaitForChild("RetryRun") :: RemoteFunction
	local useRevive = remotes:WaitForChild("UseRevive") :: RemoteFunction
	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction

	dropRequest.OnServerInvoke = handleDrop
	retryRun.OnServerInvoke = function(player)
		local session = sessions[player]
		if session and not RoundStateRules.canRetry(session.state) then
			return { ok = false, reason = "invalid_state" }
		end
		if not retryLimiter:Allow(player) then
			return { ok = false, reason = "rate_limited" }
		end
		startRun(player)
		local fresh = sessions[player]
		if not fresh then
			return { ok = false, reason = "profile_unavailable" }
		end
		return { ok = true, state = snapshot(fresh) }
	end
	useRevive.OnServerInvoke = handleRevive
	requestState.OnServerInvoke = function(player)
		local session = sessions[player]
		if not session then
			startRun(player)
			session = sessions[player]
		end
		if not session then
			return { ok = false, reason = "profile_unavailable" }
		end
		return { ok = true, state = snapshot(session) }
	end

	Players.PlayerAdded:Connect(function(player)
		task.defer(startRun, player)
	end)
	for _, player in Players:GetPlayers() do
		task.defer(startRun, player)
	end

	RunService.Heartbeat:Connect(function()
		for _, session in sessions do
			updateMoving(session)
		end
	end)
end

function RoundService.RemovePlayer(player: Player)
	sessions[player] = nil
	dropLimiter:Forget(player)
	retryLimiter:Forget(player)
	reviveLimiter:Forget(player)
end

return RoundService
