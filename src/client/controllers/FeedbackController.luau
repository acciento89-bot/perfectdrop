--!strict

local Debris = game:GetService("Debris")
local Players = game:GetService("Players")
local SoundService = game:GetService("SoundService")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local FeedbackController = {}

local player = Players.LocalPlayer

local ping = Instance.new("Sound")
ping.Name = "PerfectDropPing"
ping.SoundId = "rbxasset://sounds/action_jump_land.mp3"
ping.Volume = 0.34
ping.Parent = SoundService

local function getTop(): BasePart?
	local world = Workspace:FindFirstChild("PerfectDropWorld")
	local area = world and world:FindFirstChild(tostring(player.UserId))
	local run = area and area:FindFirstChild("Run")
	if not run then
		return nil
	end

	local top: BasePart? = nil
	local y = -math.huge
	for _, child in run:GetChildren() do
		if child:IsA("BasePart") and string.match(child.Name, "^Stack_") and child.Position.Y > y then
			top = child
			y = child.Position.Y
		end
	end
	return top
end

local function playPing(speed: number, volume: number?)
	if player:GetAttribute("PDAudioMuted") == true then
		return
	end
	ping.PlaybackSpeed = speed
	ping.Volume = volume or 0.34
	ping.TimePosition = 0
	ping:Play()
end

local function pulse(color: Color3, expansion: number?, duration: number?)
	if player:GetAttribute("PDReducedMotion") == true then
		return
	end
	local top = getTop()
	if not top then
		return
	end

	local plate = Instance.new("Part")
	plate.Name = "DropPulse"
	plate.Anchored = true
	plate.CanCollide = false
	plate.CanQuery = false
	plate.CanTouch = false
	plate.Material = Enum.Material.Neon
	plate.Color = color
	plate.Transparency = 0.22
	plate.Size = Vector3.new(math.max(2, top.Size.X), 0.08, math.max(2, top.Size.Z))
	plate.CFrame = top.CFrame + Vector3.new(0, top.Size.Y / 2 + 0.08, 0)
	plate.Parent = Workspace

	local grow = expansion or 4
	local time = duration or 0.42
	local goalSize = Vector3.new(plate.Size.X + grow, 0.08, plate.Size.Z + grow)
	TweenService:Create(
		plate,
		TweenInfo.new(time, Enum.EasingStyle.Quint, Enum.EasingDirection.Out),
		{ Size = goalSize, Transparency = 1 }
	):Play()
	Debris:AddItem(plate, time + 0.1)
end

local function completionBurst()
	local accent = Color3.fromRGB(255, 194, 66)
	pulse(accent, 5, 0.45)
	task.delay(0.10, function()
		pulse(Color3.fromRGB(89, 180, 198), 8, 0.55)
	end)
	task.delay(0.22, function()
		pulse(Color3.fromRGB(255, 153, 51), 11, 0.65)
	end)

	if player:GetAttribute("PDReducedMotion") ~= true then
		local top = getTop()
		if top then
			for index = 1, 16 do
				local angle = index / 16 * math.pi * 2
				local shard = Instance.new("Part")
				shard.Name = "TowerCompleteShard"
				shard.Size = Vector3.new(0.22, 0.44, 0.14)
				shard.CFrame = CFrame.new(top.Position + Vector3.new(0, top.Size.Y / 2 + 0.45, 0))
					* CFrame.Angles(angle * 0.3, angle, angle * 0.5)
				shard.Anchored = false
				shard.CanCollide = false
				shard.CanTouch = false
				shard.CanQuery = false
				shard.CastShadow = false
				shard.Material = Enum.Material.Neon
				shard.Color = if index % 3 == 0 then Color3.fromRGB(89, 180, 198) else Color3.fromRGB(255, 194, 66)
				shard.AssemblyLinearVelocity =
					Vector3.new(math.cos(angle) * 8.5, 11 + (index % 5), math.sin(angle) * 8.5)
				shard.AssemblyAngularVelocity = Vector3.new(2.3, 4.1, 1.7)
				shard.Parent = Workspace
				Debris:AddItem(shard, 2.4)
			end
		end
	end

	playPing(1.30, 0.42)
	task.delay(0.12, function()
		playPing(1.48, 0.38)
	end)
	task.delay(0.24, function()
		playPing(1.68, 0.34)
	end)
end

local function feedback(state, grade: string?)
	if not grade then
		return
	end

	if grade == "PERFECT" then
		local combo = math.clamp(tonumber(state and state.combo) or 0, 0, 10)
		playPing(1.06 + combo * 0.045)
		pulse(Color3.fromRGB(255, 194, 66), 4.5, 0.42)
	elseif grade == "GOOD" then
		playPing(0.94)
		pulse(Color3.fromRGB(69, 206, 159), 3.5, 0.38)
	elseif grade == "SHIELD" or grade == "REVIVE" then
		playPing(0.78)
		pulse(Color3.fromRGB(137, 121, 255), 4.5, 0.46)
	elseif grade == "TOWER COMPLETE" then
		completionBurst()
	elseif grade == "MISS" then
		playPing(0.52, 0.30)
	end
end

function FeedbackController.Start(remotes: Folder)
	local stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	stateChanged.OnClientEvent:Connect(function(state, grade)
		feedback(state, grade)
	end)
end

return FeedbackController
