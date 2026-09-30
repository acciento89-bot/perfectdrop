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
ping.SoundId = "rbxasset://sounds/electronicpingshort.wav"
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

local function pulse(color: Color3)
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
	plate.Transparency = 0.25
	plate.Size = Vector3.new(math.max(2, top.Size.X), 0.08, math.max(2, top.Size.Z))
	plate.CFrame = top.CFrame + Vector3.new(0, top.Size.Y / 2 + 0.08, 0)
	plate.Parent = Workspace

	local goalSize = Vector3.new(plate.Size.X + 4, 0.08, plate.Size.Z + 4)
	TweenService:Create(
		plate,
		TweenInfo.new(0.42, Enum.EasingStyle.Quint, Enum.EasingDirection.Out),
		{ Size = goalSize, Transparency = 1 }
	):Play()
	Debris:AddItem(plate, 0.5)
end

local function feedback(grade: string?)
	if not grade then
		return
	end

	if grade == "PERFECT" then
		ping.PlaybackSpeed = 1.18
		ping:Play()
		pulse(Color3.fromRGB(255, 194, 66))
	elseif grade == "GOOD" then
		ping.PlaybackSpeed = 0.94
		ping:Play()
		pulse(Color3.fromRGB(69, 206, 159))
	elseif grade == "SHIELD" or grade == "REVIVE" then
		ping.PlaybackSpeed = 0.78
		ping:Play()
		pulse(Color3.fromRGB(137, 121, 255))
	elseif grade == "MISS" then
		ping.PlaybackSpeed = 0.52
		ping:Play()
	end
end

function FeedbackController.Start(remotes: Folder)
	local stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	stateChanged.OnClientEvent:Connect(function(_, grade)
		feedback(grade)
	end)
end

return FeedbackController
