--!strict

local Lighting = game:GetService("Lighting")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Terrain = workspace.Terrain
local TweenService = game:GetService("TweenService")

local sharedRoot = ReplicatedStorage:WaitForChild("PerfectDrop")
local VisualTheme = require(sharedRoot:WaitForChild("ui"):WaitForChild("VisualTheme"))

local VisualPolishController = {}
local player = Players.LocalPlayer
local playerGui = player:WaitForChild("PlayerGui")
local watched: { [ScreenGui]: RBXScriptConnection } = {}

local function hex(value: string): Color3
	local raw = string.gsub(value, "#", "")
	local r = tonumber(string.sub(raw, 1, 2), 16) or 255
	local g = tonumber(string.sub(raw, 3, 4), 16) or 255
	local b = tonumber(string.sub(raw, 5, 6), 16) or 255
	return Color3.fromRGB(r, g, b)
end

local function ensure(className: string, name: string): Instance
	local current = Lighting:FindFirstChild(name)
	if current and current.ClassName == className then
		return current
	end
	if current then
		current:Destroy()
	end
	local instance = Instance.new(className)
	instance.Name = name
	instance.Parent = Lighting
	return instance
end

local function configureLighting()
	local cfg = VisualTheme.Lighting
	Lighting.Brightness = cfg.Brightness
	Lighting.ClockTime = cfg.ClockTime
	Lighting.ExposureCompensation = cfg.ExposureCompensation
	Lighting.EnvironmentDiffuseScale = cfg.EnvironmentDiffuseScale
	Lighting.EnvironmentSpecularScale = cfg.EnvironmentSpecularScale
	Lighting.FogStart = cfg.FogStart
	Lighting.FogEnd = cfg.FogEnd
	Lighting.Ambient = hex(cfg.Ambient)
	Lighting.OutdoorAmbient = hex(cfg.OutdoorAmbient)
	Lighting.FogColor = hex(cfg.FogColor)

	local atmosphere = ensure("Atmosphere", "KNVisualAtmosphere") :: Atmosphere
	atmosphere.Density = VisualTheme.Atmosphere.Density
	atmosphere.Offset = VisualTheme.Atmosphere.Offset
	atmosphere.Color = hex(VisualTheme.Atmosphere.Color)
	atmosphere.Decay = hex(VisualTheme.Atmosphere.Decay)
	atmosphere.Glare = VisualTheme.Atmosphere.Glare
	atmosphere.Haze = VisualTheme.Atmosphere.Haze

	local bloom = ensure("BloomEffect", "KNVisualBloom") :: BloomEffect
	bloom.Enabled = true
	bloom.Intensity = VisualTheme.Effects.BloomIntensity
	bloom.Size = VisualTheme.Effects.BloomSize
	bloom.Threshold = VisualTheme.Effects.BloomThreshold

	local correction = ensure("ColorCorrectionEffect", "KNVisualColor") :: ColorCorrectionEffect
	correction.Enabled = true
	correction.Saturation = VisualTheme.Effects.Saturation
	correction.Contrast = VisualTheme.Effects.Contrast
	correction.TintColor = hex(VisualTheme.Effects.TintColor)

	local rays = ensure("SunRaysEffect", "KNVisualSunRays") :: SunRaysEffect
	rays.Enabled = VisualTheme.Effects.SunRaysIntensity > 0
	rays.Intensity = VisualTheme.Effects.SunRaysIntensity
	rays.Spread = VisualTheme.Effects.SunRaysSpread

	local clouds = Terrain:FindFirstChildOfClass("Clouds")
	if not clouds then
		clouds = Instance.new("Clouds")
		clouds.Name = "KNVisualClouds"
		clouds.Parent = Terrain
	end
	local typedClouds = clouds :: Clouds
	typedClouds.Cover = VisualTheme.Clouds.Cover
	typedClouds.Density = VisualTheme.Clouds.Density
	typedClouds.Color = hex(VisualTheme.Clouds.Color)
end

local function hasGuiName(screen: ScreenGui): boolean
	for _, name in VisualTheme.TargetGuis do
		if screen.Name == name then
			return true
		end
	end
	for _, prefix in VisualTheme.TargetPrefixes do
		if string.sub(screen.Name, 1, #prefix) == prefix then
			return true
		end
	end
	return false
end

local function addCorner(gui: GuiObject, radius: number)
	if gui:FindFirstChildOfClass("UICorner") then
		return
	end
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = gui
end

local function addStroke(gui: GuiObject, color: Color3, transparency: number, thickness: number)
	local stroke = gui:FindFirstChild("KNVisualStroke")
	if not stroke then
		stroke = Instance.new("UIStroke")
		stroke.Name = "KNVisualStroke"
		stroke.Parent = gui
	end
	local typed = stroke :: UIStroke
	typed.Color = color
	typed.Transparency = transparency
	typed.Thickness = thickness
end

local function addGradient(gui: GuiObject, top: Color3, bottom: Color3, rotation: number)
	local gradient = gui:FindFirstChild("KNVisualGradient")
	if not gradient then
		gradient = Instance.new("UIGradient")
		gradient.Name = "KNVisualGradient"
		gradient.Parent = gui
	end
	local typed = gradient :: UIGradient
	typed.Color = ColorSequence.new(top, bottom)
	typed.Rotation = rotation
end

local function buttonColor(name: string): Color3
	local lower = string.lower(name)
	if
		string.find(lower, "revive", 1, true)
		or string.find(lower, "retry", 1, true)
		or string.find(lower, "claim", 1, true)
	then
		return hex(VisualTheme.Colors.Accent3)
	end
	if
		string.find(lower, "shop", 1, true)
		or string.find(lower, "style", 1, true)
		or string.find(lower, "supply", 1, true)
	then
		return hex(VisualTheme.Colors.Accent2)
	end
	return hex(VisualTheme.Colors.Accent)
end

local function shouldPolishFrame(name: string): boolean
	local lower = string.lower(name)
	for _, token in { "status", "panel", "card", "metric", "objective", "result", "tutorial", "warning", "pill" } do
		if string.find(lower, token, 1, true) then
			return true
		end
	end
	return false
end

local function polish(instance: Instance)
	if instance:GetAttribute("KNVisualPolished") == true then
		return
	end
	if instance:IsA("TextButton") then
		local accent = buttonColor(instance.Name)
		instance.BackgroundColor3 = accent
		instance.TextColor3 = hex(VisualTheme.Colors.Text)
		instance.AutoButtonColor = false
		addCorner(instance, 12)
		addStroke(instance, accent:Lerp(Color3.new(1, 1, 1), 0.42), 0.32, 1.35)
		addGradient(instance, accent:Lerp(Color3.new(1, 1, 1), 0.11), accent:Lerp(Color3.new(0, 0, 0), 0.11), 90)
		instance.MouseEnter:Connect(function()
			TweenService
				:Create(instance, TweenInfo.new(0.12), { BackgroundColor3 = accent:Lerp(Color3.new(1, 1, 1), 0.13) })
				:Play()
		end)
		instance.MouseLeave:Connect(function()
			TweenService:Create(instance, TweenInfo.new(0.12), { BackgroundColor3 = accent }):Play()
		end)
		instance:SetAttribute("KNVisualPolished", true)
	elseif instance:IsA("Frame") and shouldPolishFrame(instance.Name) and instance.BackgroundTransparency < 0.96 then
		local panel = hex(VisualTheme.Colors.Panel)
		local panel2 = hex(VisualTheme.Colors.Panel2)
		instance.BackgroundColor3 = panel
		addCorner(instance, 14)
		addStroke(instance, hex(VisualTheme.Colors.Accent2), 0.7, 1.05)
		addGradient(instance, panel2, panel, 115)
		instance:SetAttribute("KNVisualPolished", true)
	elseif instance:IsA("ScrollingFrame") and instance.BackgroundTransparency < 0.96 then
		instance.BackgroundColor3 = hex(VisualTheme.Colors.Panel)
		instance.ScrollBarImageColor3 = hex(VisualTheme.Colors.Accent)
		addCorner(instance, 14)
		addStroke(instance, hex(VisualTheme.Colors.Accent2), 0.76, 1)
		instance:SetAttribute("KNVisualPolished", true)
	end
end

local function polishScreen(screen: ScreenGui)
	if not hasGuiName(screen) then
		return
	end
	for _, descendant in screen:GetDescendants() do
		polish(descendant)
	end
	if not watched[screen] then
		watched[screen] = screen.DescendantAdded:Connect(polish)
	end
end

function VisualPolishController.Start()
	configureLighting()
	for _, child in playerGui:GetChildren() do
		if child:IsA("ScreenGui") then
			polishScreen(child)
		end
	end
	playerGui.ChildAdded:Connect(function(child)
		if child:IsA("ScreenGui") then
			task.defer(polishScreen, child)
		end
	end)
end

return VisualPolishController
