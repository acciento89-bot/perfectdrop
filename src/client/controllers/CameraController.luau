--!strict

local Players = game:GetService("Players")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local CameraController = {}

local followConnection: RBXScriptConnection?

local function getTowerTop(player: Player): Vector3?
	local world = Workspace:FindFirstChild("PerfectDropWorld")
	if not world then
		return nil
	end
	local area = world:FindFirstChild(tostring(player.UserId))
	local run = area and area:FindFirstChild("Run")
	if not run then
		return nil
	end

	local topY = -math.huge
	local topPosition: Vector3? = nil
	for _, child in run:GetChildren() do
		if child:IsA("BasePart") and child.Name ~= "Overhang" and child.Position.Y > topY then
			topY = child.Position.Y
			topPosition = child.Position
		end
	end
	return topPosition
end

local function bindCharacter(character: Model)
	local root = character:WaitForChild("HumanoidRootPart", 8)
	local humanoid = character:WaitForChild("Humanoid", 8)
	if not root or not root:IsA("BasePart") or not humanoid or not humanoid:IsA("Humanoid") then
		return
	end

	if followConnection then
		followConnection:Disconnect()
	end

	followConnection = RunService.RenderStepped:Connect(function(dt)
		local camera = Workspace.CurrentCamera
		if not camera or not root.Parent then
			return
		end

		camera.CameraType = Enum.CameraType.Scriptable
		camera.CameraSubject = humanoid

		local tower = getTowerTop(Players.LocalPlayer)
		local towerHeight = math.max(0, Players.LocalPlayer:GetAttribute("PDHeight") or 0)
		local focus = root.Position + Vector3.new(0, 2.5, 0)
		if tower then
			local weight = math.clamp(0.35 + towerHeight * 0.012, 0.35, 0.62)
			focus = focus:Lerp(tower + Vector3.new(0, 1.5, 0), weight)
		end

		local distance = 28 + math.min(16, towerHeight * 0.55)
		local height = 15 + math.min(18, towerHeight * 0.5)
		local desired = CFrame.lookAt(focus + Vector3.new(22, height, distance), focus, Vector3.yAxis)
		local alpha = math.clamp(1 - math.exp(-8 * dt), 0, 1)
		camera.CFrame = camera.CFrame:Lerp(desired, alpha)
		camera.Focus = CFrame.new(focus)
	end)
end

function CameraController.Start()
	local player = Players.LocalPlayer
	if player.Character then
		task.defer(bindCharacter, player.Character)
	end
	player.CharacterAdded:Connect(bindCharacter)
end

return CameraController
