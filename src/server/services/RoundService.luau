--!strict

local Debris = game:GetService("Debris")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local GameConfig = require(ReplicatedStorage.PerfectDrop.config.GameConfig)
local DifficultyRules = require(ReplicatedStorage.PerfectDrop.game.DifficultyRules)
local DropRules = require(ReplicatedStorage.PerfectDrop.game.DropRules)
local RoundStateRules = require(ReplicatedStorage.PerfectDrop.game.RoundStateRules)
local ScoreRules = require(ReplicatedStorage.PerfectDrop.game.ScoreRules)
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
	topBlock: Part,
	movingBlock: Part?,
	moveStartedAt: number,
	lastDropAt: number,
	axis: "X" | "Z",
}

local RoundService = {}
local sessions: { [Player]: Session } = {}
local stateChanged: RemoteEvent

local function colorForGrade(grade: string): Color3
	if grade == "PERFECT" then
		return Color3.fromRGB(255, 194, 66)
	elseif grade == "GOOD" then
		return Color3.fromRGB(69, 206, 159)
	end
	return Color3.fromRGB(241, 86, 91)
end

local function snapshot(session: Session)
	return {
		state = session.state,
		height = session.height,
		score = session.score,
		combo = session.combo,
		revivesUsed = session.revivesUsed,
		axis = session.axis,
		speed = DifficultyRules.speedForHeight(
			session.height,
			GameConfig.Block.InitialSpeed,
			GameConfig.Block.SpeedPerDrop,
			GameConfig.Block.MaxSpeed
		),
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
	block.Parent = parent

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

local function createMoving(session: Session)
	local axis = DifficultyRules.axisForHeight(session.height)
	session.axis = axis

	local top = session.topBlock
	local y = top.Position.Y + GameConfig.Block.Height
	local position = Vector3.new(top.Position.X, y, top.Position.Z)
	local start = if axis == "X"
		then top.Position.X - GameConfig.Block.TravelHalfDistance
		else top.Position.Z - GameConfig.Block.TravelHalfDistance
	position = setAxis(position, axis, start)

	local moving = makeBlock("MovingBlock", top.Size, position, Color3.fromRGB(83, 99, 122), top.Parent, true)
	moving:SetAttribute("Active", true)
	session.movingBlock = moving
	session.moveStartedAt = os.clock()
end

local function startRun(player: Player)
	local area, origin = WorldService.EnsurePlayerArea(player)
	cleanRun(area)

	local run = Instance.new("Folder")
	run.Name = "Run"
	run.Parent = area

	local base = makeBlock(
		"Stack_0",
		Vector3.new(GameConfig.Block.BaseSize, GameConfig.Block.Height, GameConfig.Block.BaseSize),
		origin + Vector3.new(0, 1.2 + GameConfig.Block.Height / 2, 0),
		Color3.fromRGB(56, 66, 81),
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
		topBlock = base,
		movingBlock = nil,
		moveStartedAt = 0,
		lastDropAt = -math.huge,
		axis = "X",
	}
	sessions[player] = session

	player:SetAttribute("PDState", "ACTIVE")
	player:SetAttribute("PDHeight", 0)
	player:SetAttribute("PDScore", 0)
	player:SetAttribute("PDCombo", 0)

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
end

local function handleDrop(player: Player)
	local session = sessions[player]
	if not session or not RoundStateRules.canDrop(session.state) then
		return { ok = false, reason = "invalid_state" }
	end

	local now = os.clock()
	if now - session.lastDropAt < 0.12 then
		return { ok = false, reason = "rate_limited" }
	end
	session.lastDropAt = now

	local moving = session.movingBlock
	if not moving or not moving.Parent then
		return { ok = false, reason = "moving_block_missing" }
	end

	local previousCenter, previousSize = blockAxisValues(session.topBlock, session.axis)
	local movingCenter, movingSize = blockAxisValues(moving, session.axis)
	local result = DropRules.evaluate(
		previousCenter,
		previousSize,
		movingCenter,
		movingSize,
		GameConfig.Block.PerfectTolerance,
		GameConfig.Block.MinimumFootprint
	)

	moving:SetAttribute("Active", false)
	session.movingBlock = nil

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

	createOverhang(session, moving, result.overlapCenter, result.overlapSize, result.offset)

	local acceptedCenter = if result.grade == "PERFECT" then previousCenter else result.overlapCenter
	moving.Position = setAxis(moving.Position, session.axis, acceptedCenter)
	moving.Size = resizeAxis(moving.Size, session.axis, result.overlapSize)
	moving.Name = ("Stack_%d"):format(session.height + 1)
	moving.Color = colorForGrade(result.grade)
	moving.Anchored = true

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
	updatePlayerAttributes(session)
	emit(session, result.grade)

	task.delay(0.1, function()
		if sessions[player] == session and session.state == "ACTIVE" then
			createMoving(session)
			emit(session)
		end
	end)

	return { ok = true, grade = result.grade, points = points, state = snapshot(session) }
end

local function updateMoving(session: Session)
	local moving = session.movingBlock
	if session.state ~= "ACTIVE" or not moving or not moving.Parent then
		return
	end

	local top = session.topBlock
	local speed = DifficultyRules.speedForHeight(
		session.height,
		GameConfig.Block.InitialSpeed,
		GameConfig.Block.SpeedPerDrop,
		GameConfig.Block.MaxSpeed
	)
	local half = GameConfig.Block.TravelHalfDistance
	local elapsed = os.clock() - session.moveStartedAt
	local cyclePosition = (elapsed * speed / (2 * half)) % 2
	local alpha = if cyclePosition <= 1 then cyclePosition else 2 - cyclePosition
	local delta = -half + alpha * (2 * half)
	local baseCenter = if session.axis == "X" then top.Position.X else top.Position.Z
	moving.Position = setAxis(moving.Position, session.axis, baseCenter + delta)
end

function RoundService.Start(remotes: Folder)
	stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	local dropRequest = remotes:WaitForChild("DropRequest") :: RemoteFunction
	local retryRun = remotes:WaitForChild("RetryRun") :: RemoteFunction
	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction

	dropRequest.OnServerInvoke = handleDrop
	retryRun.OnServerInvoke = function(player)
		local session = sessions[player]
		if session and not RoundStateRules.canRetry(session.state) then
			return { ok = false, reason = "invalid_state" }
		end
		startRun(player)
		return { ok = true, state = snapshot(sessions[player]) }
	end
	requestState.OnServerInvoke = function(player)
		local session = sessions[player]
		if not session then
			startRun(player)
			session = sessions[player]
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
end

return RoundService
