--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local GameConfig = require(ReplicatedStorage.PerfectDrop.config.GameConfig)
local UserInputService = game:GetService("UserInputService")

local HudController = {}

local player = Players.LocalPlayer
local gui: ScreenGui
local status: Frame
local stageProgressFill: Frame
local levelStatus: TextLabel
local stageStatus: TextLabel
local scoreStatus: TextLabel
local comboStatus: TextLabel
local coinsStatus: TextLabel
local statusStroke: UIStroke
local tutorial: TextLabel
local feedback: TextLabel
local dropButton: TextButton
local shopButton: TextButton
local resultPanel: Frame
local resultTitle: TextLabel
local resultSummary: TextLabel
local retryButton: TextButton
local reviveButton: TextButton
local shopPanel: Frame
local shopCoins: TextLabel
local dailyButton: TextButton
local challengeLabel: TextLabel
local reducedMotionButton: TextButton
local audioButton: TextButton
local currentState = "ACTIVE"
local currentMeta = nil
local themeButtons: { [TextButton]: { category: string, id: string } } = {}

local ACCENT = Color3.fromRGB(255, 153, 51)
local SURFACE = Color3.fromRGB(22, 27, 34)
local SURFACE_RAISED = Color3.fromRGB(31, 38, 48)
local TEXT = Color3.fromRGB(239, 243, 249)
local MUTED = Color3.fromRGB(170, 180, 194)

local function round(object: GuiObject, radius: number)
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = object
end

local function stroke(object: GuiObject, color: Color3, transparency: number?): UIStroke
	local line = Instance.new("UIStroke")
	line.Color = color
	line.Thickness = 1.4
	line.Transparency = transparency or 0.2
	line.Parent = object
	return line
end

local function textConstraint(object: TextLabel | TextButton, minSize: number, maxSize: number)
	local constraint = Instance.new("UITextSizeConstraint")
	constraint.MinTextSize = minSize
	constraint.MaxTextSize = maxSize
	constraint.Parent = object
end

local function makeLabel(parent: Instance, text: string, height: number): TextLabel
	local label = Instance.new("TextLabel")
	label.Size = UDim2.new(1, 0, 0, height)
	label.BackgroundTransparency = 1
	label.Font = Enum.Font.GothamMedium
	label.TextSize = 15
	label.TextColor3 = TEXT
	label.TextXAlignment = Enum.TextXAlignment.Left
	label.Text = text
	label.Parent = parent
	return label
end

local function makeActionButton(parent: Instance, text: string, color: Color3?): TextButton
	local button = Instance.new("TextButton")
	button.Size = UDim2.new(1, 0, 0, 48)
	button.BackgroundColor3 = color or SURFACE_RAISED
	button.BorderSizePixel = 0
	button.Font = Enum.Font.GothamBold
	button.TextSize = 15
	button.TextColor3 = TEXT
	button.Text = text
	button.AutoButtonColor = true
	button.Selectable = true
	button.Parent = parent
	round(button, 12)
	stroke(button, Color3.fromRGB(78, 88, 104), 0.35)
	return button
end

local function metaValue(key: string, fallback)
	if currentMeta and currentMeta[key] ~= nil then
		return currentMeta[key]
	end
	return fallback
end

local function makeStatusMetric(parent: Instance, name: string, titleText: string): TextLabel
	local card = Instance.new("Frame")
	card.Name = name
	card.BackgroundColor3 = SURFACE_RAISED
	card.BackgroundTransparency = 0.05
	card.BorderSizePixel = 0
	card.Parent = parent
	round(card, 9)
	stroke(card, Color3.fromRGB(72, 82, 98), 0.48)

	local title = Instance.new("TextLabel")
	title.Name = "Title"
	title.Position = UDim2.fromOffset(8, 4)
	title.Size = UDim2.new(1, -16, 0, 12)
	title.BackgroundTransparency = 1
	title.Font = Enum.Font.GothamBold
	title.TextSize = 9
	title.TextColor3 = MUTED
	title.TextXAlignment = Enum.TextXAlignment.Left
	title.Text = titleText
	title.Parent = card

	local value = Instance.new("TextLabel")
	value.Name = "Value"
	value.Position = UDim2.fromOffset(8, 16)
	value.Size = UDim2.new(1, -16, 1, -19)
	value.BackgroundTransparency = 1
	value.Font = Enum.Font.GothamBlack
	value.TextScaled = true
	value.TextColor3 = TEXT
	value.TextXAlignment = Enum.TextXAlignment.Left
	value.Text = "—"
	value.Parent = card
	textConstraint(value, 10, 16)
	return value
end

local function updateTopStatus()
	local score = player:GetAttribute("PDScore") or 0
	local height = player:GetAttribute("PDHeight") or 0
	local combo = player:GetAttribute("PDCombo") or 0
	local level = metaValue("playerLevel", player:GetAttribute("PDPlayerLevel") or 1)
	local coins = metaValue("coins", player:GetAttribute("PDCoins") or 0)
	local assisted = player:GetAttribute("PDAssisted") == true
	levelStatus.Text = ("LV %d"):format(level)
	stageStatus.Text = ("%d / %d"):format(height, GameConfig.Progression.MaxStages)
	scoreStatus.Text = tostring(score)
	comboStatus.Text = if assisted then ("x%d  •  A"):format(combo) else ("x%d"):format(combo)
	comboStatus.TextColor3 = if assisted then Color3.fromRGB(219, 211, 255) else Color3.fromRGB(255, 194, 66)
	coinsStatus.Text = ("◈ %d"):format(coins)
	statusStroke.Color = if assisted then Color3.fromRGB(137, 121, 255) else ACCENT
	if stageProgressFill then
		local progress = math.clamp(height / GameConfig.Progression.MaxStages, 0, 1)
		stageProgressFill.Size = UDim2.fromScale(progress, 1)
	end
end

local function showGrade(grade: string?)
	if not grade then
		return
	end
	feedback.Text = grade
	feedback.TextTransparency = 0
	feedback.Size = UDim2.fromOffset(300, 72)
	feedback.Visible = true
	if grade == "PERFECT" or grade == "TOWER COMPLETE" then
		feedback.TextColor3 = Color3.fromRGB(255, 194, 66)
	elseif grade == "GOOD" then
		feedback.TextColor3 = Color3.fromRGB(69, 206, 159)
	elseif grade == "SHIELD" or grade == "REVIVE" then
		feedback.TextColor3 = Color3.fromRGB(137, 121, 255)
	else
		feedback.TextColor3 = Color3.fromRGB(241, 86, 91)
	end

	if player:GetAttribute("PDReducedMotion") == true then
		task.delay(0.45, function()
			feedback.Visible = false
		end)
	else
		local tween = TweenService:Create(
			feedback,
			TweenInfo.new(0.55, Enum.EasingStyle.Quint, Enum.EasingDirection.Out),
			{ TextTransparency = 1, Size = UDim2.fromOffset(360, 86) }
		)
		tween:Play()
		tween.Completed:Once(function()
			feedback.Visible = false
		end)
	end
end

local function updateResult(state)
	local failed = state.state == "FAILED"
	local completed = state.state == "COMPLETED"
	resultPanel.Visible = failed or completed
	if not failed and not completed then
		return
	end

	local best = metaValue("bestScore", state.bestScore or 0)
	if completed then
		local level = metaValue("playerLevel", state.playerLevel or 1)
		resultTitle.Text = "TOWER COMPLETE!"
		resultTitle.TextColor3 = Color3.fromRGB(255, 194, 66)
		resultSummary.Text = ("30/30 stages cleared  •  Score %d  •  Player Level %d\n+75 ◈ tower bonus%s"):format(
			state.score or 0,
			level,
			if state.assisted then "  •  Assisted run" else ""
		)
		retryButton.Text = "NEXT TOWER"
		reviveButton.Visible = false
	else
		resultTitle.Text = "RUN OVER"
		resultTitle.TextColor3 = Color3.fromRGB(241, 86, 91)
		resultSummary.Text = ("Score %d   •   Stage %d/%d   •   Best %d%s"):format(
			state.score or 0,
			state.height or 0,
			GameConfig.Progression.MaxStages,
			best,
			if state.assisted then "\nAssisted run — PB protected" else ""
		)
		retryButton.Text = "RETRY"
		local revives = metaValue("revives", state.revives or 0)
		reviveButton.Visible = (state.revivesUsed or 0) < 1 and revives > 0
		reviveButton.Text = ("USE REVIVE  (%d)"):format(revives)
	end
end

local function ownedFor(category: string, id: string): boolean
	if not currentMeta then
		return false
	end
	local map = if category == "block" then currentMeta.ownedBlockThemes else currentMeta.ownedArenaThemes
	return map and map[id] == true
end

local function equippedFor(category: string, id: string): boolean
	if not currentMeta then
		return false
	end
	if category == "block" then
		return currentMeta.equippedBlockTheme == id
	end
	return currentMeta.equippedArenaTheme == id
end

local themePrices = {
	["block:graphite"] = 0,
	["block:sunset"] = 250,
	["block:ocean"] = 450,
	["arena:rooftop"] = 0,
	["arena:night"] = 700,
}

local themeNames = {
	["block:graphite"] = "BLOCK • GRAPHITE",
	["block:sunset"] = "BLOCK • SUNSET",
	["block:ocean"] = "BLOCK • OCEAN",
	["arena:rooftop"] = "ARENA • SKYLINE",
	["arena:night"] = "ARENA • NIGHT SHIFT",
}

local function refreshShop()
	local coins = metaValue("coins", player:GetAttribute("PDCoins") or 0)
	shopCoins.Text = ("COINS  ◈ %d"):format(coins)

	local dailyClaimable = metaValue("dailyClaimable", false)
	dailyButton.Text = if dailyClaimable then "CLAIM DAILY REWARD  +25 ◈" else "DAILY REWARD CLAIMED"
	dailyButton.Active = dailyClaimable
	dailyButton.AutoButtonColor = dailyClaimable
	dailyButton.BackgroundColor3 = if dailyClaimable then ACCENT else Color3.fromRGB(52, 60, 72)
	dailyButton.TextColor3 = if dailyClaimable then Color3.fromRGB(24, 28, 34) else MUTED

	local perfects = metaValue("dailyPerfects", 0)
	local required = metaValue("dailyRequired", 3)
	local rewarded = metaValue("dailyChallengeRewarded", false)
	challengeLabel.Text = if rewarded
		then ("DAILY CHALLENGE  ✓  %d/%d PERFECTS"):format(math.min(perfects, required), required)
		else ("DAILY CHALLENGE  %d/%d PERFECTS  •  +30 ◈"):format(math.min(perfects, required), required)

	for button, info in themeButtons do
		local key = info.category .. ":" .. info.id
		local name = themeNames[key] or info.id
		if equippedFor(info.category, info.id) then
			button.Text = name .. "    ✓ EQUIPPED"
			button.BackgroundColor3 = Color3.fromRGB(48, 82, 69)
		elseif ownedFor(info.category, info.id) then
			button.Text = name .. "    EQUIP"
			button.BackgroundColor3 = SURFACE_RAISED
		else
			local price = themePrices[key] or 0
			button.Text = ("%s    BUY %d ◈"):format(name, price)
			button.BackgroundColor3 = SURFACE_RAISED
		end
	end

	reducedMotionButton.Text = if player:GetAttribute("PDReducedMotion") == true
		then "REDUCED MOTION  ON"
		else "REDUCED MOTION  OFF"
	audioButton.Text = if player:GetAttribute("PDAudioMuted") == true then "AUDIO  OFF" else "AUDIO  ON"
end

local function setShopVisible(visible: boolean)
	shopPanel.Visible = visible
	if visible then
		refreshShop()
	end
end

function HudController.ToggleShop()
	setShopVisible(not shopPanel.Visible)
end

function HudController.IsOverlayOpen(): boolean
	return shopPanel.Visible
end

function HudController.ApplyMeta(meta)
	currentMeta = meta
	updateTopStatus()
	refreshShop()
end

function HudController.ApplyState(state, grade: string?)
	currentState = state.state
	updateTopStatus()
	showGrade(grade)
	updateResult(state)

	local blocked = currentState == "FAILED" or currentState == "COMPLETED"
	dropButton.Visible = not blocked and not shopPanel.Visible

	local bestHeight = metaValue("bestHeight", player:GetAttribute("PDBestHeight") or 0)
	tutorial.Visible = not blocked and bestHeight == 0 and (state.height or 0) < 2 and not shopPanel.Visible
	comboStatus.TextColor3 = if state.assisted then Color3.fromRGB(219, 211, 255) else Color3.fromRGB(255, 194, 66)
end

function HudController.GetState(): string
	return currentState
end

function HudController.Start(remotes: Folder)
	local playerGui = player:WaitForChild("PlayerGui")

	gui = Instance.new("ScreenGui")
	gui.Name = "PerfectDropHUD"
	gui.ResetOnSpawn = false
	gui.IgnoreGuiInset = false
	gui.DisplayOrder = 20
	gui.Parent = playerGui

	status = Instance.new("Frame")
	status.AnchorPoint = Vector2.new(0.5, 0)
	status.Position = UDim2.new(0.5, 0, 0, 16)
	status.Size = UDim2.new(1, -36, 0, 72)
	status.BackgroundColor3 = SURFACE
	status.BackgroundTransparency = 0.04
	status.BorderSizePixel = 0
	status.Parent = gui
	round(status, 14)
	statusStroke = stroke(status, ACCENT)
	local statusSize = Instance.new("UISizeConstraint")
	statusSize.MaxSize = Vector2.new(760, 72)
	statusSize.MinSize = Vector2.new(300, 56)
	statusSize.Parent = status

	local metricRow = Instance.new("Frame")
	metricRow.Name = "Metrics"
	metricRow.Position = UDim2.fromOffset(8, 7)
	metricRow.Size = UDim2.new(1, -16, 1, -18)
	metricRow.BackgroundTransparency = 1
	metricRow.Parent = status

	local metricGrid = Instance.new("UIGridLayout")
	metricGrid.FillDirection = Enum.FillDirection.Horizontal
	metricGrid.FillDirectionMaxCells = 5
	metricGrid.SortOrder = Enum.SortOrder.LayoutOrder
	metricGrid.CellPadding = UDim2.fromOffset(5, 0)
	metricGrid.CellSize = UDim2.new(0.2, -4, 1, 0)
	metricGrid.Parent = metricRow

	levelStatus = makeStatusMetric(metricRow, "LevelMetric", "LEVEL")
	stageStatus = makeStatusMetric(metricRow, "StageMetric", "STAGE")
	scoreStatus = makeStatusMetric(metricRow, "ScoreMetric", "SCORE")
	comboStatus = makeStatusMetric(metricRow, "ComboMetric", "PERFECT")
	coinsStatus = makeStatusMetric(metricRow, "CoinsMetric", "COINS")
	metricRow.LevelMetric.LayoutOrder = 1
	metricRow.StageMetric.LayoutOrder = 2
	metricRow.ScoreMetric.LayoutOrder = 3
	metricRow.ComboMetric.LayoutOrder = 4
	metricRow.CoinsMetric.LayoutOrder = 5

	local progressTrack = Instance.new("Frame")
	progressTrack.Name = "StageProgress"
	progressTrack.AnchorPoint = Vector2.new(0.5, 1)
	progressTrack.Position = UDim2.new(0.5, 0, 1, -4)
	progressTrack.Size = UDim2.new(1, -20, 0, 3)
	progressTrack.BackgroundColor3 = Color3.fromRGB(67, 76, 90)
	progressTrack.BorderSizePixel = 0
	progressTrack.ClipsDescendants = true
	progressTrack.Parent = status
	round(progressTrack, 3)

	stageProgressFill = Instance.new("Frame")
	stageProgressFill.Name = "Fill"
	stageProgressFill.Size = UDim2.fromScale(0, 1)
	stageProgressFill.BackgroundColor3 = ACCENT
	stageProgressFill.BorderSizePixel = 0
	stageProgressFill.Parent = progressTrack
	round(stageProgressFill, 3)

	tutorial = Instance.new("TextLabel")
	tutorial.AnchorPoint = Vector2.new(0.5, 0)
	tutorial.Position = UDim2.new(0.5, 0, 0, 90)
	tutorial.Size = UDim2.new(1, -54, 0, 54)
	tutorial.BackgroundColor3 = Color3.fromRGB(29, 35, 44)
	tutorial.BackgroundTransparency = 0.06
	tutorial.BorderSizePixel = 0
	tutorial.Font = Enum.Font.GothamMedium
	tutorial.TextScaled = true
	tutorial.TextWrapped = true
	tutorial.TextColor3 = Color3.fromRGB(231, 236, 244)
	tutorial.Text = "DROP when the block lines up. Clear 30 stages to level up — every stage gets a little narrower."
	tutorial.Parent = gui
	round(tutorial, 12)
	textConstraint(tutorial, 12, 16)
	local tutorialSize = Instance.new("UISizeConstraint")
	tutorialSize.MaxSize = Vector2.new(560, 54)
	tutorialSize.MinSize = Vector2.new(244, 48)
	tutorialSize.Parent = tutorial

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
	dropButton.Position = UDim2.new(1, -24, 1, -24)
	dropButton.Size = UDim2.fromOffset(172, 64)
	dropButton.BackgroundColor3 = ACCENT
	dropButton.BorderSizePixel = 0
	dropButton.Font = Enum.Font.GothamBold
	dropButton.TextSize = 20
	dropButton.TextColor3 = Color3.fromRGB(24, 28, 34)
	dropButton.Text = "DROP"
	dropButton.Selectable = true
	dropButton.Parent = gui
	round(dropButton, 16)

	shopButton = Instance.new("TextButton")
	shopButton.Name = "ShopButton"
	shopButton.AnchorPoint = Vector2.new(1, 0)
	shopButton.Position = UDim2.new(1, -18, 0, 94)
	shopButton.Size = UDim2.fromOffset(108, 48)
	shopButton.BackgroundColor3 = SURFACE
	shopButton.BorderSizePixel = 0
	shopButton.Font = Enum.Font.GothamBold
	shopButton.TextSize = 14
	shopButton.TextColor3 = TEXT
	shopButton.Text = "SHOP"
	shopButton.Selectable = true
	shopButton.Parent = gui
	round(shopButton, 12)
	stroke(shopButton, Color3.fromRGB(82, 93, 110), 0.25)

	resultPanel = Instance.new("Frame")
	resultPanel.Name = "ResultPanel"
	resultPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	resultPanel.Position = UDim2.fromScale(0.5, 0.52)
	resultPanel.Size = UDim2.new(0.86, 0, 0, 244)
	resultPanel.BackgroundColor3 = SURFACE
	resultPanel.BackgroundTransparency = 0.02
	resultPanel.BorderSizePixel = 0
	resultPanel.Visible = false
	resultPanel.Parent = gui
	round(resultPanel, 18)
	stroke(resultPanel, Color3.fromRGB(241, 86, 91), 0.12)
	local resultSize = Instance.new("UISizeConstraint")
	resultSize.MaxSize = Vector2.new(520, 244)
	resultSize.MinSize = Vector2.new(266, 220)
	resultSize.Parent = resultPanel

	resultTitle = Instance.new("TextLabel")
	resultTitle.Position = UDim2.fromOffset(20, 18)
	resultTitle.Size = UDim2.new(1, -40, 0, 44)
	resultTitle.BackgroundTransparency = 1
	resultTitle.Font = Enum.Font.GothamBlack
	resultTitle.TextSize = 30
	resultTitle.TextColor3 = Color3.fromRGB(241, 86, 91)
	resultTitle.Text = "RUN OVER"
	resultTitle.Parent = resultPanel

	resultSummary = Instance.new("TextLabel")
	resultSummary.Position = UDim2.fromOffset(20, 68)
	resultSummary.Size = UDim2.new(1, -40, 0, 54)
	resultSummary.BackgroundTransparency = 1
	resultSummary.Font = Enum.Font.GothamMedium
	resultSummary.TextSize = 15
	resultSummary.TextWrapped = true
	resultSummary.TextColor3 = TEXT
	resultSummary.Text = ""
	resultSummary.Parent = resultPanel

	retryButton = Instance.new("TextButton")
	retryButton.Name = "RetryButton"
	retryButton.Position = UDim2.new(0, 20, 1, -74)
	retryButton.Size = UDim2.new(0.5, -26, 0, 54)
	retryButton.BackgroundColor3 = ACCENT
	retryButton.BorderSizePixel = 0
	retryButton.Font = Enum.Font.GothamBold
	retryButton.TextSize = 18
	retryButton.TextColor3 = Color3.fromRGB(24, 28, 34)
	retryButton.Text = "RETRY"
	retryButton.Selectable = true
	retryButton.Parent = resultPanel
	round(retryButton, 14)

	reviveButton = Instance.new("TextButton")
	reviveButton.Name = "ReviveButton"
	reviveButton.AnchorPoint = Vector2.new(1, 0)
	reviveButton.Position = UDim2.new(1, -20, 1, -74)
	reviveButton.Size = UDim2.new(0.5, -26, 0, 54)
	reviveButton.BackgroundColor3 = Color3.fromRGB(88, 78, 168)
	reviveButton.BorderSizePixel = 0
	reviveButton.Font = Enum.Font.GothamBold
	reviveButton.TextSize = 15
	reviveButton.TextColor3 = TEXT
	reviveButton.Text = "USE REVIVE"
	reviveButton.Selectable = true
	reviveButton.Visible = false
	reviveButton.Parent = resultPanel
	round(reviveButton, 14)

	shopPanel = Instance.new("Frame")
	shopPanel.Name = "ShopPanel"
	shopPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	shopPanel.Position = if UserInputService.TouchEnabled
		then UDim2.fromScale(0.5, 0.57)
		else UDim2.fromScale(0.5, 0.52)
	shopPanel.Size = if UserInputService.TouchEnabled then UDim2.fromScale(0.88, 0.68) else UDim2.fromScale(0.92, 0.78)
	shopPanel.BackgroundColor3 = SURFACE
	shopPanel.BorderSizePixel = 0
	shopPanel.Visible = false
	shopPanel.Parent = gui
	round(shopPanel, 18)
	stroke(shopPanel, ACCENT, 0.12)
	local shopSize = Instance.new("UISizeConstraint")
	shopSize.MaxSize = Vector2.new(620, 610)
	shopSize.MinSize = Vector2.new(266, 290)
	shopSize.Parent = shopPanel

	local shopTitle = Instance.new("TextLabel")
	shopTitle.Position = UDim2.fromOffset(20, 16)
	shopTitle.Size = UDim2.new(1, -130, 0, 38)
	shopTitle.BackgroundTransparency = 1
	shopTitle.Font = Enum.Font.GothamBlack
	shopTitle.TextSize = if UserInputService.TouchEnabled then 20 else 24
	shopTitle.TextXAlignment = Enum.TextXAlignment.Left
	shopTitle.TextColor3 = TEXT
	shopTitle.Text = "PERFECT DROP"
	shopTitle.Parent = shopPanel

	shopCoins = Instance.new("TextLabel")
	shopCoins.AnchorPoint = Vector2.new(1, 0)
	shopCoins.Position = UDim2.new(1, -58, 0, 18)
	shopCoins.Size = UDim2.fromOffset(160, 32)
	shopCoins.BackgroundTransparency = 1
	shopCoins.Font = Enum.Font.GothamBold
	shopCoins.TextSize = 15
	shopCoins.TextXAlignment = Enum.TextXAlignment.Right
	shopCoins.TextColor3 = Color3.fromRGB(255, 194, 66)
	shopCoins.Text = "COINS"
	shopCoins.Parent = shopPanel

	local close = Instance.new("TextButton")
	close.AnchorPoint = Vector2.new(1, 0)
	close.Position = UDim2.new(1, -14, 0, 14)
	close.Size = UDim2.fromOffset(44, 44)
	close.BackgroundColor3 = Color3.fromRGB(48, 55, 67)
	close.BorderSizePixel = 0
	close.Font = Enum.Font.GothamBold
	close.TextSize = 18
	close.TextColor3 = TEXT
	close.Text = "×"
	close.Selectable = true
	close.Parent = shopPanel
	round(close, 10)

	local scroll = Instance.new("ScrollingFrame")
	scroll.Position = UDim2.fromOffset(18, 66)
	scroll.Size = UDim2.new(1, -36, 1, -84)
	scroll.BackgroundTransparency = 1
	scroll.BorderSizePixel = 0
	scroll.ScrollBarThickness = 5
	scroll.ScrollBarImageColor3 = Color3.fromRGB(98, 108, 124)
	scroll.AutomaticCanvasSize = Enum.AutomaticSize.Y
	scroll.CanvasSize = UDim2.new()
	scroll.Parent = shopPanel

	local list = Instance.new("UIListLayout")
	list.Padding = UDim.new(0, 10)
	list.SortOrder = Enum.SortOrder.LayoutOrder
	list.Parent = scroll

	dailyButton = makeActionButton(scroll, "CLAIM DAILY REWARD", ACCENT)
	dailyButton.TextColor3 = Color3.fromRGB(24, 28, 34)

	challengeLabel = makeLabel(scroll, "DAILY CHALLENGE", 38)
	challengeLabel.TextColor3 = Color3.fromRGB(255, 194, 66)

	local blockHeader = makeLabel(scroll, "BLOCK THEMES", 30)
	blockHeader.Font = Enum.Font.GothamBold
	blockHeader.TextColor3 = MUTED

	for _, id in { "graphite", "sunset", "ocean" } do
		local button = makeActionButton(scroll, id)
		themeButtons[button] = { category = "block", id = id }
	end

	local arenaHeader = makeLabel(scroll, "ARENA THEMES", 30)
	arenaHeader.Font = Enum.Font.GothamBold
	arenaHeader.TextColor3 = MUTED

	for _, id in { "rooftop", "night" } do
		local button = makeActionButton(scroll, id)
		themeButtons[button] = { category = "arena", id = id }
	end

	reducedMotionButton = makeActionButton(scroll, "REDUCED MOTION  OFF")
	reducedMotionButton.Name = "ReducedMotionButton"
	reducedMotionButton.LayoutOrder = 50
	audioButton = makeActionButton(scroll, "AUDIO  ON")
	audioButton.Name = "AudioButton"
	audioButton.LayoutOrder = 51

	local function applyResponsiveLayout()
		local camera = Workspace.CurrentCamera
		if not camera then
			return
		end

		local viewport = camera.ViewportSize
		local compact = UserInputService.TouchEnabled and math.min(viewport.X, viewport.Y) < 600
		local levelCard = metricRow:FindFirstChild("LevelMetric")
		local stageCard = metricRow:FindFirstChild("StageMetric")
		local scoreCard = metricRow:FindFirstChild("ScoreMetric")
		local comboCard = metricRow:FindFirstChild("ComboMetric")
		local coinsCard = metricRow:FindFirstChild("CoinsMetric")

		if compact then
			status.Size = UDim2.fromOffset(math.min(560, math.max(420, viewport.X - 190)), 56)
			status.Position = UDim2.new(0.5, 0, 0, 10)
			metricRow.Position = UDim2.fromOffset(7, 6)
			metricRow.Size = UDim2.new(1, -14, 1, -15)
			metricGrid.FillDirectionMaxCells = 3
			metricGrid.CellPadding = UDim2.fromOffset(4, 0)
			metricGrid.CellSize = UDim2.new(1 / 3, -3, 1, 0)
			if levelCard then
				levelCard.Visible = false
			end
			if coinsCard then
				coinsCard.Visible = false
			end
			if stageCard then
				stageCard.Visible = true
			end
			if scoreCard then
				scoreCard.Visible = true
			end
			if comboCard then
				comboCard.Visible = true
			end
			tutorial.Position = UDim2.new(0.5, 0, 0, 72)
			tutorial.Size = UDim2.fromOffset(math.min(380, viewport.X - 210), 42)
			shopButton.Position = UDim2.new(1, -14, 0, 74)
			shopButton.Size = UDim2.fromOffset(92, 44)
			dropButton.Position = UDim2.new(1, -18, 1, -20)
			dropButton.Size = UDim2.fromOffset(154, 58)
			shopPanel.Position = UDim2.fromScale(0.5, 0.54)
			shopPanel.Size = UDim2.fromScale(0.76, 0.72)
			resultPanel.Size = UDim2.new(0.78, 0, 0, 214)
		else
			status.Size = UDim2.new(1, -36, 0, 72)
			status.Position = UDim2.new(0.5, 0, 0, 16)
			metricRow.Position = UDim2.fromOffset(8, 7)
			metricRow.Size = UDim2.new(1, -16, 1, -18)
			metricGrid.FillDirectionMaxCells = 5
			metricGrid.CellPadding = UDim2.fromOffset(5, 0)
			metricGrid.CellSize = UDim2.new(0.2, -4, 1, 0)
			if levelCard then
				levelCard.Visible = true
			end
			if coinsCard then
				coinsCard.Visible = true
			end
			if stageCard then
				stageCard.Visible = true
			end
			if scoreCard then
				scoreCard.Visible = true
			end
			if comboCard then
				comboCard.Visible = true
			end
			tutorial.Position = UDim2.new(0.5, 0, 0, 90)
			shopButton.Position = UDim2.new(1, -18, 0, 94)
			shopButton.Size = UDim2.fromOffset(108, 48)
			dropButton.Position = UDim2.new(1, -24, 1, -24)
			dropButton.Size = UDim2.fromOffset(172, 64)
			shopPanel.Position = if UserInputService.TouchEnabled
				then UDim2.fromScale(0.5, 0.57)
				else UDim2.fromScale(0.5, 0.52)
			shopPanel.Size = if UserInputService.TouchEnabled
				then UDim2.fromScale(0.88, 0.68)
				else UDim2.fromScale(0.92, 0.78)
			resultPanel.Size = UDim2.new(0.86, 0, 0, 244)
		end
	end

	applyResponsiveLayout()
	local camera = Workspace.CurrentCamera
	if camera then
		camera:GetPropertyChangedSignal("ViewportSize"):Connect(applyResponsiveLayout)
	end
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(function()
		task.defer(applyResponsiveLayout)
	end)

	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction
	local requestMeta = remotes:WaitForChild("RequestMetaState") :: RemoteFunction
	local stateChanged = remotes:WaitForChild("RoundStateChanged") :: RemoteEvent
	local metaChanged = remotes:WaitForChild("MetaStateChanged") :: RemoteEvent
	local purchaseCosmetic = remotes:WaitForChild("PurchaseCosmetic") :: RemoteFunction
	local equipCosmetic = remotes:WaitForChild("EquipCosmetic") :: RemoteFunction
	local claimDaily = remotes:WaitForChild("ClaimDaily") :: RemoteFunction

	close.Activated:Connect(function()
		setShopVisible(false)
		dropButton.Visible = currentState == "ACTIVE"
	end)
	shopButton.Activated:Connect(function()
		setShopVisible(not shopPanel.Visible)
		dropButton.Visible = currentState == "ACTIVE" and not shopPanel.Visible
		tutorial.Visible = tutorial.Visible and not shopPanel.Visible
	end)

	dailyButton.Activated:Connect(function()
		if not metaValue("dailyClaimable", false) then
			return
		end
		local result = claimDaily:InvokeServer()
		if result and result.meta then
			HudController.ApplyMeta(result.meta)
		end
	end)

	for button, info in themeButtons do
		button.Activated:Connect(function()
			if equippedFor(info.category, info.id) then
				return
			end
			if not ownedFor(info.category, info.id) then
				local purchase = purchaseCosmetic:InvokeServer(info.category, info.id)
				if not purchase or not purchase.ok then
					showGrade("NOT ENOUGH COINS")
					return
				end
				if purchase.meta then
					HudController.ApplyMeta(purchase.meta)
				end
			end
			local equipped = equipCosmetic:InvokeServer(info.category, info.id)
			if equipped and equipped.meta then
				HudController.ApplyMeta(equipped.meta)
			end
		end)
	end

	reducedMotionButton.Activated:Connect(function()
		player:SetAttribute("PDReducedMotion", not (player:GetAttribute("PDReducedMotion") == true))
		refreshShop()
	end)
	audioButton.Activated:Connect(function()
		player:SetAttribute("PDAudioMuted", not (player:GetAttribute("PDAudioMuted") == true))
		refreshShop()
	end)

	local metaResult = requestMeta:InvokeServer()
	if metaResult and metaResult.ok then
		HudController.ApplyMeta(metaResult.meta)
	end
	local stateResult = requestState:InvokeServer()
	if stateResult and stateResult.ok then
		HudController.ApplyState(stateResult.state)
	end

	stateChanged.OnClientEvent:Connect(function(state, grade)
		HudController.ApplyState(state, grade)
	end)
	metaChanged.OnClientEvent:Connect(function(meta)
		HudController.ApplyMeta(meta)
	end)

	updateTopStatus()
	refreshShop()
end

function HudController.GetDropButton(): TextButton
	return dropButton
end

function HudController.GetRetryButton(): TextButton
	return retryButton
end

function HudController.GetReviveButton(): TextButton
	return reviveButton
end

return HudController
