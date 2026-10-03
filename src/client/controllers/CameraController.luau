--!strict

local Players = game:GetService("Players")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local CameraController = {}

local followConnection: RBXScriptConnection?
local descendantAddedConnection: RBXScriptConnection?
local descendantRemovingConnection: RBXScriptConnection?
local visibleParts: { BasePart } = {}

local function getRunTargets(player: Player): (Vector3?, Vector3?)
	local world = Workspace:FindFirstChild("PerfectDropWorld")
	if not world then
		return nil, nil
	end
	local area = world:FindFirstChild(tostring(player.UserId))
	local run = area and area:FindFirstChild("Run")
	if not run then
		return nil, nil
	end

	local topY = -math.huge
	local topPosition: Vector3? = nil
	local movingPosition: Vector3? = nil
	for _, child in run:GetChildren() do
		if child:IsA("BasePart") then
			if child.Name == "MovingBlock" then
				movingPosition = child.Position
			elseif child.Name ~= "Overhang" and child.Position.Y > topY then
				topY = child.Position.Y
				topPosition = child.Position
			end
		end
	end
	return topPosition, movingPosition
end

local function trackVisiblePart(instance: Instance)
	if instance:IsA("BasePart") then
		instance.LocalTransparencyModifier = 0
		table.insert(visibleParts, instance)
	end
end

local function untrackVisiblePart(instance: Instance)
	if not instance:IsA("BasePart") then
		return
	end
	local index = table.find(visibleParts, instance)
	if index then
		table.remove(visibleParts, index)
	end
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
	if descendantAddedConnection then
		descendantAddedConnection:Disconnect()
	end
	if descendantRemovingConnection then
		descendantRemovingConnection:Disconnect()
	end

	table.clear(visibleParts)
	for _, descendant in character:GetDescendants() do
		trackVisiblePart(descendant)
	end
	descendantAddedConnection = character.DescendantAdded:Connect(trackVisiblePart)
	descendantRemovingConnection = character.DescendantRemoving:Connect(untrackVisiblePart)

	followConnection = RunService.RenderStepped:Connect(function(dt)
		local camera = Workspace.CurrentCamera
		if not camera or not root.Parent then
			return
		end

		camera.CameraType = Enum.CameraType.Scriptable
		camera.CameraSubject = humanoid
		for _, bodyPart in visibleParts do
			if bodyPart.Parent and bodyPart.LocalTransparencyModifier ~= 0 then
				bodyPart.LocalTransparencyModifier = 0
			end
		end

		local tower, moving = getRunTargets(Players.LocalPlayer)
		local lowTarget = root.Position + Vector3.new(0, 2.4, 0)
		local highTarget = moving or tower or lowTarget
		local focus = (lowTarget + highTarget) * 0.5
		local verticalSpan = math.abs(highTarget.Y - lowTarget.Y)
		local horizontalSpan = (Vector3.new(highTarget.X, 0, highTarget.Z) - Vector3.new(lowTarget.X, 0, lowTarget.Z)).Magnitude

		-- Frame the avatar and the active moving block together. This scales with
		-- the actual run geometry instead of stage count, so late-stage blocks do
		-- not disappear above/behind the mobile HUD.
		local compact = math.min(camera.ViewportSize.X, camera.ViewportSize.Y) <= 600
		camera.FieldOfView = if compact then 74 else 70
		local distance = (if compact then 15 else 17)
			+ verticalSpan * (if compact then 0.58 else 0.52)
			+ math.min(7, horizontalSpan * 0.28)
		local height = (if compact then 7 else 8.5) + verticalSpan * 0.20
		local side = if compact then 9.5 else 12.5
		local desired = CFrame.lookAt(focus + Vector3.new(side, height, distance), focus, Vector3.yAxis)
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
