--!strict

local Players = game:GetService("Players")
local TweenService = game:GetService("TweenService")

local HudController = {}

local gui: ScreenGui
local status: TextLabel
local tutorial: TextLabel
local feedback: TextLabel
local retry: TextButton
local dropButton: TextButton
local currentState = "ACTIVE"

local function round(object: GuiObject, radius: number)
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = object
end

local function stroke(object: GuiObject, color: Color3)
	local line = Instance.new("UIStroke")
	line.Color = color
	line.Thickness = 1.5
	line.Transparency = 0.2
	line.Parent = object
end

local function updateStatus()
	local player = Players.LocalPlayer
	local score = player:GetAttribute("PDScore") or 0
	local height = player:GetAttribute("PDHeight") or 0
	local combo = player:GetAttribute("PDCombo") or 0
	status.Text = ("SCORE  %d     HEIGHT  %d     PERFECT x%d"):format(score, height, combo)
end

local function showGrade(grade: string?)
	if not grade then
		return
	end
	feedback.Text = grade
	feedback.TextTransparency = 0
	feedback.Size = UDim2.fromOffset(300, 72)
	feedback.Visible = true
	if grade == "PERFECT" then
		feedback.TextColor3 = Color3.fromRGB(255, 194, 66)
	elseif grade == "GOOD" then
		feedback.TextColor3 = Color3.fromRGB(69, 206, 159)
	else
		feedback.TextColor3 = Color3.fromRGB(241, 86, 91)
	end
	TweenService:Create(
		feedback,
		TweenInfo.new(0.55, Enum.EasingStyle.Quint, Enum.EasingDirection.Out),
		{ TextTransparency = 1, Size = UDim2.fromOffset(360, 86) }
	):Play()
end

function HudController.ApplyState(state, grade: string?)
	currentState = state.state
	updateStatus()
	showGrade(grade)
	local failed = currentState == "FAILED"
	retry.Visible = failed
	dropButton.Visible = not failed
	tutorial.Visible = (state.height or 0) < 2 and not failed
	if failed then
		tutorial.Visible = false
	end
end

function HudController.GetState(): string
	return currentState
end

function HudController.Start(remotes: Folder)
	local playerGui = Players.LocalPlayer:WaitForChild("PlayerGui")
	gui = Instance.new("ScreenGui")
	gui.Name = "PerfectDropHUD"
	gui.ResetOnSpawn = false
	gui.IgnoreGuiInset = false
	gui.DisplayOrder = 20
	gui.Parent = playerGui

	status = Instance.new("TextLabel")
	status.AnchorPoint = Vector2.new(0.5, 0)
	status.Position = UDim2.new(0.5, 0, 0, 18)
	status.Size = UDim2.fromOffset(520, 48)
	status.BackgroundColor3 = Color3.fromRGB(22, 27, 34)
	status.BackgroundTransparency = 0.06
	status.BorderSizePixel = 0
	status.Font = Enum.Font.GothamBold
	status.TextSize = 17
	status.TextColor3 = Color3.fromRGB(239, 243, 249)
	status.Parent = gui
	round(status, 14)
	stroke(status, Color3.fromRGB(255, 153, 51))

	tutorial = Instance.new("TextLabel")
	tutorial.AnchorPoint = Vector2.new(0.5, 0)
	tutorial.Position = UDim2.new(0.5, 0, 0, 78)
	tutorial.Size = UDim2.fromOffset(490, 54)
	tutorial.BackgroundColor3 = Color3.fromRGB(29, 35, 44)
	tutorial.BackgroundTransparency = 0.08
	tutorial.BorderSizePixel = 0
	tutorial.Font = Enum.Font.GothamMedium
	tutorial.TextSize = 16
	tutorial.TextWrapped = true
	tutorial.TextColor3 = Color3.fromRGB(231, 236, 244)
	tutorial.Text = "DROP when the moving block lines up. Perfect timing keeps the full platform."
	tutorial.Parent = gui
	round(tutorial, 12)

	feedback = Instance.new("TextLabel")
	feedback.AnchorPoint = Vector2.new(0.5, 0.5)
	feedback.Position = UDim2.fromScale(0.5, 0.34)
	feedback.Size = UDim2.fromOffset(300, 72)
	feedback.BackgroundTransparency = 1
	feedback.Font = Enum.Font.GothamBlack
	feedback.TextSize = 38
	feedback.TextStrokeTransparency = 0.75
	feedback.Visible = false
	feedback.Parent = gui

	dropButton = Instance.new("TextButton")
	dropButton.Name = "DropButton"
	dropButton.AnchorPoint = Vector2.new(1, 1)
	dropButton.Position = UDim2.new(1, -26, 1, -26)
	dropButton.Size = UDim2.fromOffset(170, 64)
	dropButton.BackgroundColor3 = Color3.fromRGB(255, 153, 51)
	dropButton.BorderSizePixel = 0
	dropButton.Font = Enum.Font.GothamBold
	dropButton.TextSize = 20
	dropButton.TextColor3 = Color3.fromRGB(24, 28, 34)
	dropButton.Text = "DROP"
	dropButton.Parent = gui
	round(dropButton, 16)

	retry = Instance.new("TextButton")
	retry.Name = "RetryButton"
	retry.AnchorPoint = Vector2.new(0.5, 0.5)
	retry.Position = UDim2.fromScale(0.5, 0.58)
	retry.Size = UDim2.fromOffset(240, 64)
	retry.BackgroundColor3 = Color3.fromRGB(255, 153, 51)
	retry.BorderSizePixel = 0
	retry.Font = Enum.Font.GothamBold
	retry.TextSize = 21
	retry.TextColor3 = Color3.fromRGB(24, 28, 34)
	retry.Text = "RETRY"
	retry.Visible = false
	retry.Parent = gui
	round(retry, 16)

	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction
	local stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	local result = requestState:InvokeServer()
	if result and result.ok then
		HudController.ApplyState(result.state)
	end

	stateChanged.OnClientEvent:Connect(function(state, grade)
		HudController.ApplyState(state, grade)
	end)

	updateStatus()
end

function HudController.GetDropButton(): TextButton
	return dropButton
end

function HudController.GetRetryButton(): TextButton
	return retry
end

return HudController
