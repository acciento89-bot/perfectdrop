--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local CosmeticConfig = require(ReplicatedStorage.PerfectDrop.config.CosmeticConfig)
local GameConfig = require(ReplicatedStorage.PerfectDrop.config.GameConfig)
local UserInputService = game:GetService("UserInputService")

local HudController = {}

local player = Players.LocalPlayer
local gui: ScreenGui
local status: Frame
local mobileSummary: TextLabel
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
local dailyBackdrop: Frame
local dailyPanel: Frame
local dailyStatus: TextLabel
local dailyClaimButton: TextButton
local challengeLabel: TextLabel
local reducedMotionButton: TextButton
local audioButton: TextButton
local currentState = "ACTIVE"
local currentMeta = nil
local themeButtons: { [TextButton]: { category: string, id: string } } = {}
local conceptShowcase: Frame? = nil
local conceptProgressText: TextLabel? = nil
local conceptProgressFill: Frame? = nil
local conceptCoins: TextLabel? = nil
local conceptDailyCaption: TextLabel? = nil
local conceptDailyTiles: { Frame } = {}

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
	if mobileSummary then
		mobileSummary.Text = ("STAGE %d/%d   SCORE %d   PERF x%d%s"):format(
			height,
			GameConfig.Progression.MaxStages,
			score,
			combo,
			if assisted then " A" else ""
		)
	end
	statusStroke.Color = if assisted then Color3.fromRGB(137, 121, 255) else ACCENT
	local progress = math.clamp(height / GameConfig.Progression.MaxStages, 0, 1)
	if stageProgressFill then
		stageProgressFill.Size = UDim2.fromScale(progress, 1)
	end
	if conceptProgressText then
		conceptProgressText.Text = ("STAGE %d / %d"):format(height, GameConfig.Progression.MaxStages)
	end
	if conceptProgressFill then
		conceptProgressFill.Size = UDim2.fromScale(progress, 1)
	end
	if conceptCoins then
		conceptCoins.Text = ("◈ %d"):format(coins)
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
	dailyClaimButton.Text = if dailyClaimable then "CLAIM DAILY +25 ◈" else "DAILY REWARD CLAIMED"
	dailyClaimButton.Active = dailyClaimable
	dailyClaimButton.AutoButtonColor = dailyClaimable
	dailyClaimButton.BackgroundColor3 = if dailyClaimable then ACCENT else Color3.fromRGB(52, 60, 72)
	dailyClaimButton.TextColor3 = if dailyClaimable then Color3.fromRGB(24, 28, 34) else MUTED
	dailyStatus.Text = if dailyClaimable then "TODAY'S REWARD IS READY" else "CLAIMED • COME BACK TOMORROW"
	if conceptDailyCaption then
		conceptDailyCaption.Text = if dailyClaimable then "READY TO CLAIM" else "CLAIMED TODAY"
	end
	for index, tile in conceptDailyTiles do
		if index == 1 and not dailyClaimable then
			tile.BackgroundColor3 = Color3.fromRGB(49, 104, 86)
		elseif index == 1 and dailyClaimable then
			tile.BackgroundColor3 = Color3.fromRGB(112, 78, 156)
		else
			tile.BackgroundColor3 = Color3.fromRGB(48, 55, 75)
		end
	end

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
		dailyPanel.Visible = false
		dailyBackdrop.Visible = false
		refreshShop()
	end
end

local function setDailyVisible(visible: boolean)
	dailyPanel.Visible = visible
	dailyBackdrop.Visible = visible
	if visible then
		shopPanel.Visible = false
		refreshShop()
	end
end

function HudController.ToggleShop()
	setDailyVisible(false)
	setShopVisible(not shopPanel.Visible)
end

function HudController.IsOverlayOpen(): boolean
	return shopPanel.Visible or dailyPanel.Visible
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
	if blocked then
		setShopVisible(false)
		setDailyVisible(false)
	end
	local overlayOpen = HudController.IsOverlayOpen()
	dropButton.Visible = not blocked and not overlayOpen

	local bestHeight = metaValue("bestHeight", player:GetAttribute("PDBestHeight") or 0)
	tutorial.Visible = not blocked and bestHeight == 0 and (state.height or 0) < 2 and not overlayOpen
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

	local logo = Instance.new("Frame")
	logo.Name = "PerfectDropLogo"
	logo.Position = UDim2.fromOffset(18, 14)
	logo.Size = UDim2.fromOffset(178, 70)
	logo.BackgroundTransparency = 1
	logo.Parent = gui

	local logoTop = Instance.new("TextLabel")
	logoTop.BackgroundTransparency = 1
	logoTop.Size = UDim2.fromOffset(150, 34)
	logoTop.Font = Enum.Font.GothamBlack
	logoTop.Text = "PERFECT"
	logoTop.TextSize = 26
	logoTop.TextXAlignment = Enum.TextXAlignment.Left
	logoTop.TextColor3 = Color3.fromRGB(88, 223, 255)
	logoTop.TextStrokeColor3 = Color3.fromRGB(37, 34, 67)
	logoTop.TextStrokeTransparency = 0.38
	logoTop.Parent = logo

	local logoBottom = Instance.new("TextLabel")
	logoBottom.BackgroundTransparency = 1
	logoBottom.Position = UDim2.fromOffset(16, 26)
	logoBottom.Size = UDim2.fromOffset(140, 34)
	logoBottom.Font = Enum.Font.GothamBlack
	logoBottom.Text = "DROP"
	logoBottom.TextSize = 30
	logoBottom.TextXAlignment = Enum.TextXAlignment.Left
	logoBottom.TextColor3 = Color3.fromRGB(255, 185, 83)
	logoBottom.TextStrokeColor3 = Color3.fromRGB(37, 34, 67)
	logoBottom.TextStrokeTransparency = 0.34
	logoBottom.Parent = logo

	local logoTag = Instance.new("TextLabel")
	logoTag.BackgroundTransparency = 1
	logoTag.Position = UDim2.fromOffset(1, 58)
	logoTag.Size = UDim2.fromOffset(176, 12)
	logoTag.Font = Enum.Font.GothamBold
	logoTag.Text = "30 STAGES • REACH THE CROWN"
	logoTag.TextSize = 8
	logoTag.TextXAlignment = Enum.TextXAlignment.Left
	logoTag.TextColor3 = MUTED
	logoTag.Parent = logo

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
	statusSize.MaxSize = Vector2.new(610, 72)
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

	mobileSummary = Instance.new("TextLabel")
	mobileSummary.Name = "MobileSummary"
	mobileSummary.AnchorPoint = Vector2.new(1, 0)
	mobileSummary.Position = UDim2.new(1, -14, 0, 56)
	mobileSummary.Size = UDim2.fromOffset(232, 34)
	mobileSummary.BackgroundColor3 = SURFACE
	mobileSummary.BackgroundTransparency = 0.08
	mobileSummary.BorderSizePixel = 0
	mobileSummary.Font = Enum.Font.GothamBold
	mobileSummary.TextSize = 11
	mobileSummary.TextColor3 = TEXT
	mobileSummary.TextXAlignment = Enum.TextXAlignment.Center
	mobileSummary.Text = "STAGE 0/30   SCORE 0   PERF x0"
	mobileSummary.Visible = false
	mobileSummary.Parent = gui
	round(mobileSummary, 10)
	stroke(mobileSummary, ACCENT, 0.42)

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

	local quickRail = Instance.new("Frame")
	quickRail.Name = "ConceptQuickRail"
	quickRail.AnchorPoint = Vector2.new(1, 0)
	quickRail.Position = UDim2.new(1, -18, 0, 18)
	quickRail.Size = UDim2.fromOffset(94, 98)
	quickRail.BackgroundTransparency = 1
	quickRail.Parent = gui

	local quickShopButton = makeActionButton(quickRail, "SHOP", Color3.fromRGB(173, 114, 255))
	quickShopButton.Name = "ConceptShopButton"
	quickShopButton.Size = UDim2.fromOffset(94, 42)
	quickShopButton.Position = UDim2.fromOffset(0, 0)
	quickShopButton.TextColor3 = TEXT

	local dailyQuickButton = makeActionButton(quickRail, "DAILY", Color3.fromRGB(255, 178, 82))
	dailyQuickButton.Name = "ConceptDailyButton"
	dailyQuickButton.Size = UDim2.fromOffset(94, 42)
	dailyQuickButton.Position = UDim2.fromOffset(0, 50)
	dailyQuickButton.TextColor3 = Color3.fromRGB(24, 28, 34)

	conceptShowcase = Instance.new("Frame")
	conceptShowcase.Name = "ConceptShowcase"
	conceptShowcase.Size = UDim2.fromScale(1, 1)
	conceptShowcase.BackgroundTransparency = 1
	conceptShowcase.Visible = false
	conceptShowcase.Parent = gui

	local function addPanelGradient(panel: GuiObject, first: Color3, second: Color3)
		local gradient = Instance.new("UIGradient")
		gradient.Rotation = 110
		gradient.Color = ColorSequence.new(first, second)
		gradient.Parent = panel
	end

	local function configColor(values): Color3
		return Color3.fromRGB(values[1], values[2], values[3])
	end

	local shopShowcase = Instance.new("Frame")
	shopShowcase.Name = "DropShopShowcase"
	shopShowcase.AnchorPoint = Vector2.new(0, 1)
	shopShowcase.Position = UDim2.new(0, 18, 1, -18)
	shopShowcase.Size = UDim2.fromOffset(308, 208)
	shopShowcase.BackgroundColor3 = Color3.fromRGB(26, 31, 45)
	shopShowcase.BackgroundTransparency = 0.02
	shopShowcase.BorderSizePixel = 0
	shopShowcase.Parent = conceptShowcase
	round(shopShowcase, 20)
	stroke(shopShowcase, Color3.fromRGB(126, 214, 255), 0.34)
	addPanelGradient(shopShowcase, Color3.fromRGB(48, 45, 85), Color3.fromRGB(22, 27, 38))

	local showcaseTitle = Instance.new("TextLabel")
	showcaseTitle.BackgroundTransparency = 1
	showcaseTitle.Position = UDim2.fromOffset(15, 8)
	showcaseTitle.Size = UDim2.fromOffset(180, 28)
	showcaseTitle.Font = Enum.Font.GothamBlack
	showcaseTitle.TextSize = 17
	showcaseTitle.TextXAlignment = Enum.TextXAlignment.Left
	showcaseTitle.TextColor3 = TEXT
	showcaseTitle.Text = "DROP SHOP"
	showcaseTitle.Parent = shopShowcase

	local featured = Instance.new("TextLabel")
	featured.BackgroundTransparency = 1
	featured.Position = UDim2.fromOffset(202, 11)
	featured.Size = UDim2.fromOffset(88, 20)
	featured.Font = Enum.Font.GothamBold
	featured.TextSize = 9
	featured.TextXAlignment = Enum.TextXAlignment.Right
	featured.TextColor3 = Color3.fromRGB(151, 213, 255)
	featured.Text = "FEATURED"
	featured.Parent = shopShowcase

	local conceptShopButtons: { TextButton } = {}
	local featuredThemes = {
		{
			Category = "block",
			Id = "sunset",
			Name = CosmeticConfig.BlockThemes.sunset.DisplayName,
			Price = CosmeticConfig.BlockThemes.sunset.PriceCoins,
			Color = configColor(CosmeticConfig.BlockThemes.sunset.MovingRGB),
		},
		{
			Category = "block",
			Id = "ocean",
			Name = CosmeticConfig.BlockThemes.ocean.DisplayName,
			Price = CosmeticConfig.BlockThemes.ocean.PriceCoins,
			Color = configColor(CosmeticConfig.BlockThemes.ocean.MovingRGB),
		},
		{
			Category = "arena",
			Id = "night",
			Name = CosmeticConfig.ArenaThemes.night.DisplayName,
			Price = CosmeticConfig.ArenaThemes.night.PriceCoins,
			Color = configColor(CosmeticConfig.ArenaThemes.night.AccentRGB),
		},
	}
	for index, item in featuredThemes do
		local card = Instance.new("Frame")
		card.Name = ("Featured%02d"):format(index)
		card.Position = UDim2.fromOffset(14 + (index - 1) * 98, 43)
		card.Size = UDim2.fromOffset(88, 151)
		card.BackgroundColor3 = Color3.fromRGB(39, 45, 61)
		card.BackgroundTransparency = 0.02
		card.BorderSizePixel = 0
		card.Parent = shopShowcase
		round(card, 14)
		stroke(card, item.Color, 0.42)
		addPanelGradient(card, item.Color:Lerp(Color3.fromRGB(45, 39, 72), 0.72), Color3.fromRGB(30, 35, 49))

		local art = Instance.new("Frame")
		art.Name = "Preview"
		art.Position = UDim2.fromOffset(8, 8)
		art.Size = UDim2.fromOffset(72, 58)
		art.BackgroundColor3 = Color3.fromRGB(18, 23, 32)
		art.BorderSizePixel = 0
		art.Parent = card
		round(art, 11)
		stroke(art, item.Color, 0.18)

		local moving = Instance.new("Frame")
		moving.Name = "MovingBlock"
		moving.AnchorPoint = Vector2.new(0.5, 0.5)
		moving.Position = UDim2.fromScale(0.54, 0.42)
		moving.Size = UDim2.fromOffset(44, 16)
		moving.Rotation = -8
		moving.BackgroundColor3 = item.Color
		moving.BorderSizePixel = 0
		moving.Parent = art
		round(moving, 5)
		local base = Instance.new("Frame")
		base.Name = "BaseBlock"
		base.AnchorPoint = Vector2.new(0.5, 0.5)
		base.Position = UDim2.fromScale(0.44, 0.68)
		base.Size = UDim2.fromOffset(48, 13)
		base.Rotation = 5
		base.BackgroundColor3 = item.Color:Lerp(Color3.fromRGB(232, 236, 246), 0.32)
		base.BorderSizePixel = 0
		base.Parent = art
		round(base, 5)

		local name = Instance.new("TextLabel")
		name.BackgroundTransparency = 1
		name.Position = UDim2.fromOffset(5, 69)
		name.Size = UDim2.fromOffset(78, 34)
		name.Font = Enum.Font.GothamBold
		name.TextSize = 10
		name.TextWrapped = true
		name.TextColor3 = TEXT
		name.Text = item.Name
		name.Parent = card

		local price = Instance.new("TextLabel")
		price.BackgroundTransparency = 1
		price.Position = UDim2.fromOffset(5, 103)
		price.Size = UDim2.fromOffset(78, 16)
		price.Font = Enum.Font.GothamBold
		price.TextSize = 9
		price.TextColor3 = Color3.fromRGB(255, 194, 66)
		price.Text = ("◈ %d"):format(item.Price)
		price.Parent = card

		local open = makeActionButton(card, "OPEN", item.Color)
		open.Name = "OpenShop"
		open.Position = UDim2.fromOffset(9, 122)
		open.Size = UDim2.fromOffset(70, 24)
		open.TextSize = 9
		open.TextColor3 = Color3.fromRGB(22, 27, 34)
		table.insert(conceptShopButtons, open)
	end

	local rightColumn = Instance.new("Frame")
	rightColumn.Name = "ConceptRightColumn"
	rightColumn.AnchorPoint = Vector2.new(1, 0)
	rightColumn.Position = UDim2.new(1, -18, 0, 98)
	rightColumn.Size = UDim2.fromOffset(232, 366)
	rightColumn.BackgroundTransparency = 1
	rightColumn.Parent = conceptShowcase

	local conceptProgressCard = Instance.new("Frame")
	conceptProgressCard.Name = "ConceptProgressCard"
	conceptProgressCard.Size = UDim2.fromOffset(232, 86)
	conceptProgressCard.BackgroundColor3 = Color3.fromRGB(26, 31, 45)
	conceptProgressCard.BorderSizePixel = 0
	conceptProgressCard.Parent = rightColumn
	round(conceptProgressCard, 17)
	stroke(conceptProgressCard, Color3.fromRGB(89, 218, 255), 0.35)
	addPanelGradient(conceptProgressCard, Color3.fromRGB(43, 48, 78), Color3.fromRGB(23, 28, 40))

	local progressTitle = Instance.new("TextLabel")
	progressTitle.BackgroundTransparency = 1
	progressTitle.Position = UDim2.fromOffset(13, 7)
	progressTitle.Size = UDim2.fromOffset(204, 20)
	progressTitle.Font = Enum.Font.GothamBlack
	progressTitle.TextSize = 13
	progressTitle.TextColor3 = TEXT
	progressTitle.Text = "CLEAR PROGRESS"
	progressTitle.Parent = conceptProgressCard

	conceptProgressText = Instance.new("TextLabel")
	conceptProgressText.BackgroundTransparency = 1
	conceptProgressText.Position = UDim2.fromOffset(13, 28)
	conceptProgressText.Size = UDim2.fromOffset(204, 18)
	conceptProgressText.Font = Enum.Font.GothamBold
	conceptProgressText.TextSize = 9
	conceptProgressText.TextColor3 = MUTED
	conceptProgressText.Text = "STAGE 0 / 30"
	conceptProgressText.Parent = conceptProgressCard

	local conceptTrack = Instance.new("Frame")
	conceptTrack.Position = UDim2.fromOffset(13, 55)
	conceptTrack.Size = UDim2.fromOffset(206, 14)
	conceptTrack.BackgroundColor3 = Color3.fromRGB(14, 19, 29)
	conceptTrack.BorderSizePixel = 0
	conceptTrack.ClipsDescendants = true
	conceptTrack.Parent = conceptProgressCard
	round(conceptTrack, 99)
	conceptProgressFill = Instance.new("Frame")
	conceptProgressFill.Size = UDim2.fromScale(0, 1)
	conceptProgressFill.BackgroundColor3 = Color3.fromRGB(91, 220, 239)
	conceptProgressFill.BorderSizePixel = 0
	conceptProgressFill.Parent = conceptTrack
	round(conceptProgressFill, 99)

	local conceptDailyRewards = Instance.new("Frame")
	conceptDailyRewards.Name = "ConceptDailyRewards"
	conceptDailyRewards.Position = UDim2.fromOffset(0, 96)
	conceptDailyRewards.Size = UDim2.fromOffset(232, 128)
	conceptDailyRewards.BackgroundColor3 = Color3.fromRGB(26, 31, 45)
	conceptDailyRewards.BorderSizePixel = 0
	conceptDailyRewards.Parent = rightColumn
	round(conceptDailyRewards, 17)
	stroke(conceptDailyRewards, Color3.fromRGB(173, 114, 255), 0.40)
	addPanelGradient(conceptDailyRewards, Color3.fromRGB(49, 43, 80), Color3.fromRGB(23, 28, 40))

	local dailyTitle = Instance.new("TextLabel")
	dailyTitle.BackgroundTransparency = 1
	dailyTitle.Position = UDim2.fromOffset(13, 6)
	dailyTitle.Size = UDim2.fromOffset(204, 20)
	dailyTitle.Font = Enum.Font.GothamBlack
	dailyTitle.TextSize = 13
	dailyTitle.TextColor3 = TEXT
	dailyTitle.Text = "DAILY REWARDS"
	dailyTitle.Parent = conceptDailyRewards

	conceptDailyCaption = Instance.new("TextLabel")
	conceptDailyCaption.BackgroundTransparency = 1
	conceptDailyCaption.Position = UDim2.fromOffset(13, 27)
	conceptDailyCaption.Size = UDim2.fromOffset(204, 16)
	conceptDailyCaption.Font = Enum.Font.GothamBold
	conceptDailyCaption.TextSize = 8
	conceptDailyCaption.TextColor3 = MUTED
	conceptDailyCaption.Text = "READY TO CLAIM"
	conceptDailyCaption.Parent = conceptDailyRewards

	conceptDailyTiles = {}
	for index = 1, 5 do
		local tile = Instance.new("Frame")
		tile.Name = ("Day%02d"):format(index)
		tile.Position = UDim2.fromOffset(9 + (index - 1) * 44, 48)
		tile.Size = UDim2.fromOffset(39, 67)
		tile.BackgroundColor3 = Color3.fromRGB(48, 55, 75)
		tile.BorderSizePixel = 0
		tile.Parent = conceptDailyRewards
		round(tile, 9)
		stroke(tile, Color3.fromRGB(129, 147, 180), 0.55)

		local day = Instance.new("TextLabel")
		day.BackgroundTransparency = 1
		day.Position = UDim2.fromOffset(2, 4)
		day.Size = UDim2.fromOffset(35, 14)
		day.Font = Enum.Font.GothamBold
		day.TextSize = 8
		day.TextColor3 = MUTED
		day.Text = ("D%d"):format(index)
		day.Parent = tile

		local reward = Instance.new("TextLabel")
		reward.BackgroundTransparency = 1
		reward.Position = UDim2.fromOffset(2, 22)
		reward.Size = UDim2.fromOffset(35, 31)
		reward.Font = Enum.Font.GothamBlack
		reward.TextSize = 15
		reward.TextColor3 = if index == 1 then Color3.fromRGB(69, 206, 159) else Color3.fromRGB(255, 194, 66)
		reward.Text = if index == 1
			then "✓"
			elseif index == 2 then "◈"
			elseif index == 3 then "✦"
			elseif index == 4 then "◆"
			else "★"
		reward.Parent = tile
		table.insert(conceptDailyTiles, tile)
	end

	local conceptVistaCard = Instance.new("Frame")
	conceptVistaCard.Name = "ConceptVistaCard"
	conceptVistaCard.Position = UDim2.fromOffset(0, 234)
	conceptVistaCard.Size = UDim2.fromOffset(232, 132)
	conceptVistaCard.BackgroundColor3 = Color3.fromRGB(26, 31, 45)
	conceptVistaCard.BorderSizePixel = 0
	conceptVistaCard.Parent = rightColumn
	round(conceptVistaCard, 17)
	stroke(conceptVistaCard, Color3.fromRGB(255, 169, 82), 0.38)
	addPanelGradient(conceptVistaCard, Color3.fromRGB(55, 48, 75), Color3.fromRGB(23, 28, 40))

	local vistaTitle = Instance.new("TextLabel")
	vistaTitle.BackgroundTransparency = 1
	vistaTitle.Position = UDim2.fromOffset(13, 6)
	vistaTitle.Size = UDim2.fromOffset(204, 20)
	vistaTitle.Font = Enum.Font.GothamBlack
	vistaTitle.TextSize = 13
	vistaTitle.TextColor3 = TEXT
	vistaTitle.Text = "BETTER VISUALS"
	vistaTitle.Parent = conceptVistaCard

	local conceptViewport = Instance.new("ViewportFrame")
	conceptViewport.Name = "RooftopPreview"
	conceptViewport.Position = UDim2.fromOffset(12, 31)
	conceptViewport.Size = UDim2.fromOffset(208, 89)
	conceptViewport.BackgroundColor3 = Color3.fromRGB(233, 145, 103)
	conceptViewport.BorderSizePixel = 0
	conceptViewport.Ambient = Color3.fromRGB(155, 162, 190)
	conceptViewport.LightColor = Color3.fromRGB(255, 212, 173)
	conceptViewport.LightDirection = Vector3.new(-1, -1, -0.5)
	conceptViewport.Parent = conceptVistaCard
	round(conceptViewport, 12)

	local previewWorld = Instance.new("WorldModel")
	previewWorld.Parent = conceptViewport
	local previewCamera = Instance.new("Camera")
	previewCamera.CFrame = CFrame.lookAt(Vector3.new(11, 8, 14), Vector3.new(0, 1.2, 0))
	previewCamera.FieldOfView = 40
	previewCamera.Parent = conceptViewport
	conceptViewport.CurrentCamera = previewCamera

	local function previewPart(
		name: string,
		size: Vector3,
		position: Vector3,
		color: Color3,
		material: Enum.Material
	): Part
		local part = Instance.new("Part")
		part.Name = name
		part.Size = size
		part.Position = position
		part.Anchored = true
		part.CanCollide = false
		part.Color = color
		part.Material = material
		part.Parent = previewWorld
		return part
	end
	previewPart(
		"Roof",
		Vector3.new(10, 0.7, 8),
		Vector3.new(0, -0.3, 0),
		Color3.fromRGB(48, 56, 71),
		Enum.Material.Concrete
	)
	previewPart(
		"Pad",
		Vector3.new(5.2, 0.35, 4.4),
		Vector3.new(0, 0.25, 0),
		Color3.fromRGB(39, 45, 58),
		Enum.Material.Metal
	)
	previewPart(
		"MovingBlock",
		Vector3.new(4.0, 0.8, 3.3),
		Vector3.new(0.6, 1.2, 0),
		Color3.fromRGB(226, 112, 77),
		Enum.Material.Metal
	)
	previewPart(
		"TowerA",
		Vector3.new(2.1, 7, 2.1),
		Vector3.new(-4.0, 3, -4.0),
		Color3.fromRGB(36, 43, 58),
		Enum.Material.Concrete
	)
	previewPart(
		"TowerB",
		Vector3.new(2.5, 9, 2.4),
		Vector3.new(3.7, 4, -4.8),
		Color3.fromRGB(46, 50, 70),
		Enum.Material.Concrete
	)
	previewPart(
		"GoalGlow",
		Vector3.new(5.5, 0.12, 0.18),
		Vector3.new(0, 0.7, -2.2),
		Color3.fromRGB(255, 194, 66),
		Enum.Material.Neon
	)
	conceptCoins = Instance.new("TextLabel")
	conceptCoins.Name = "ConceptCoins"
	conceptCoins.AnchorPoint = Vector2.new(1, 0)
	conceptCoins.Position = UDim2.new(1, -260, 0, 20)
	conceptCoins.Size = UDim2.fromOffset(96, 42)
	conceptCoins.BackgroundColor3 = SURFACE
	conceptCoins.BackgroundTransparency = 0.03
	conceptCoins.BorderSizePixel = 0
	conceptCoins.Font = Enum.Font.GothamBold
	conceptCoins.TextSize = 14
	conceptCoins.TextColor3 = Color3.fromRGB(255, 194, 66)
	conceptCoins.Text = "◈ 0"
	conceptCoins.Parent = conceptShowcase
	round(conceptCoins, 13)
	stroke(conceptCoins, Color3.fromRGB(255, 194, 66), 0.42)

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

	dailyBackdrop = Instance.new("Frame")
	dailyBackdrop.Name = "DailyBackdrop"
	dailyBackdrop.Size = UDim2.fromScale(1, 1)
	dailyBackdrop.BackgroundColor3 = Color3.fromRGB(8, 12, 20)
	dailyBackdrop.BackgroundTransparency = 0.42
	dailyBackdrop.BorderSizePixel = 0
	dailyBackdrop.Visible = false
	dailyBackdrop.ZIndex = 44
	dailyBackdrop.Parent = gui

	dailyPanel = Instance.new("Frame")
	dailyPanel.Name = "DailyRewardsPanel"
	dailyPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	dailyPanel.Position = UDim2.fromScale(0.5, 0.52)
	dailyPanel.Size = UDim2.fromOffset(438, 292)
	dailyPanel.BackgroundColor3 = Color3.fromRGB(26, 31, 45)
	dailyPanel.BackgroundTransparency = 0.02
	dailyPanel.BorderSizePixel = 0
	dailyPanel.Visible = false
	dailyPanel.ZIndex = 45
	dailyPanel.Parent = gui
	round(dailyPanel, 20)
	stroke(dailyPanel, Color3.fromRGB(173, 114, 255), 0.30)
	local dailyGradient = Instance.new("UIGradient")
	dailyGradient.Rotation = 115
	dailyGradient.Color = ColorSequence.new(Color3.fromRGB(57, 48, 91), Color3.fromRGB(22, 27, 40))
	dailyGradient.Parent = dailyPanel

	local dailyModalTitle = Instance.new("TextLabel")
	dailyModalTitle.Name = "Title"
	dailyModalTitle.Position = UDim2.fromOffset(22, 14)
	dailyModalTitle.Size = UDim2.new(1, -88, 0, 34)
	dailyModalTitle.BackgroundTransparency = 1
	dailyModalTitle.Font = Enum.Font.GothamBlack
	dailyModalTitle.TextSize = 22
	dailyModalTitle.TextColor3 = TEXT
	dailyModalTitle.TextXAlignment = Enum.TextXAlignment.Left
	dailyModalTitle.Text = "DAILY REWARDS"
	dailyModalTitle.ZIndex = 46
	dailyModalTitle.Parent = dailyPanel

	local dailyClose = Instance.new("TextButton")
	dailyClose.Name = "Close"
	dailyClose.AnchorPoint = Vector2.new(1, 0)
	dailyClose.Position = UDim2.new(1, -14, 0, 12)
	dailyClose.Size = UDim2.fromOffset(44, 44)
	dailyClose.BackgroundColor3 = Color3.fromRGB(55, 63, 82)
	dailyClose.BorderSizePixel = 0
	dailyClose.Font = Enum.Font.GothamBold
	dailyClose.TextSize = 22
	dailyClose.TextColor3 = TEXT
	dailyClose.Text = "×"
	dailyClose.ZIndex = 46
	dailyClose.Parent = dailyPanel
	round(dailyClose, 12)

	dailyStatus = Instance.new("TextLabel")
	dailyStatus.Name = "Status"
	dailyStatus.Position = UDim2.fromOffset(22, 52)
	dailyStatus.Size = UDim2.new(1, -44, 0, 22)
	dailyStatus.BackgroundTransparency = 1
	dailyStatus.Font = Enum.Font.GothamBold
	dailyStatus.TextSize = 11
	dailyStatus.TextColor3 = MUTED
	dailyStatus.TextXAlignment = Enum.TextXAlignment.Center
	dailyStatus.Text = "DROP IN DAILY FOR A NEW REWARD"
	dailyStatus.ZIndex = 46
	dailyStatus.Parent = dailyPanel

	local dailyIcons = { "✓", "◈", "✦", "♢", "★" }
	local dailyRewards = { "TODAY", "+25", "BOOST", "THEME", "RARE" }
	for index = 1, 5 do
		local card = Instance.new("Frame")
		card.Name = ("Day%02d"):format(index)
		card.Position = UDim2.fromOffset(18 + (index - 1) * 81, 88)
		card.Size = UDim2.fromOffset(73, 112)
		card.BackgroundColor3 = Color3.fromRGB(43, 50, 68)
		card.BorderSizePixel = 0
		card.ZIndex = 46
		card.Parent = dailyPanel
		round(card, 13)
		stroke(card, Color3.fromRGB(124, 142, 176), 0.50)

		local day = Instance.new("TextLabel")
		day.Size = UDim2.new(1, 0, 0, 18)
		day.Position = UDim2.fromOffset(0, 7)
		day.BackgroundTransparency = 1
		day.Font = Enum.Font.GothamBold
		day.TextSize = 9
		day.TextColor3 = MUTED
		day.Text = ("DAY %d"):format(index)
		day.ZIndex = 47
		day.Parent = card

		local icon = Instance.new("TextLabel")
		icon.Size = UDim2.new(1, 0, 0, 42)
		icon.Position = UDim2.fromOffset(0, 30)
		icon.BackgroundTransparency = 1
		icon.Font = Enum.Font.GothamBlack
		icon.TextSize = 24
		icon.TextColor3 = if index == 1 then Color3.fromRGB(69, 206, 159) else Color3.fromRGB(255, 194, 66)
		icon.Text = dailyIcons[index]
		icon.ZIndex = 47
		icon.Parent = card

		local reward = Instance.new("TextLabel")
		reward.Size = UDim2.new(1, 0, 0, 20)
		reward.Position = UDim2.fromOffset(0, 83)
		reward.BackgroundTransparency = 1
		reward.Font = Enum.Font.GothamBold
		reward.TextSize = 9
		reward.TextColor3 = TEXT
		reward.Text = dailyRewards[index]
		reward.ZIndex = 47
		reward.Parent = card
	end

	dailyClaimButton = Instance.new("TextButton")
	dailyClaimButton.Name = "ClaimButton"
	dailyClaimButton.AnchorPoint = Vector2.new(0.5, 1)
	dailyClaimButton.Position = UDim2.new(0.5, 0, 1, -18)
	dailyClaimButton.Size = UDim2.fromOffset(182, 48)
	dailyClaimButton.BackgroundColor3 = ACCENT
	dailyClaimButton.BorderSizePixel = 0
	dailyClaimButton.Font = Enum.Font.GothamBold
	dailyClaimButton.TextSize = 14
	dailyClaimButton.TextColor3 = Color3.fromRGB(24, 28, 34)
	dailyClaimButton.Text = "CLAIM DAILY +25 ◈"
	dailyClaimButton.ZIndex = 46
	dailyClaimButton.Parent = dailyPanel
	round(dailyClaimButton, 13)
	stroke(dailyClaimButton, Color3.fromRGB(255, 213, 153), 0.40)

	dailyClose.Activated:Connect(function()
		setDailyVisible(false)
		dropButton.Visible = currentState == "ACTIVE"
	end)
	dailyBackdrop.InputBegan:Connect(function(input)
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			setDailyVisible(false)
			dropButton.Visible = currentState == "ACTIVE"
		end
	end)

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
		if conceptShowcase then
			conceptShowcase.Visible = false
		end
		local levelCard = metricRow:FindFirstChild("LevelMetric")
		local stageCard = metricRow:FindFirstChild("StageMetric")
		local scoreCard = metricRow:FindFirstChild("ScoreMetric")
		local comboCard = metricRow:FindFirstChild("ComboMetric")
		local coinsCard = metricRow:FindFirstChild("CoinsMetric")

		if compact then
			logo.Visible = false
			quickRail.Visible = false
			quickRail.Position = UDim2.new(1, -14, 0, 10)
			shopButton.Visible = true
			-- Mobile gets one tiny summary pill instead of a desktop-style status bar.
			status.Visible = false
			mobileSummary.Visible = true
			mobileSummary.AnchorPoint = Vector2.new(0, 0)
			mobileSummary.Position = UDim2.fromOffset(12, 10)
			mobileSummary.Size = UDim2.new(1, -120, 0, 40)
			status.AnchorPoint = Vector2.new(1, 0)
			statusSize.MinSize = Vector2.new(0, 0)
			statusSize.MaxSize = Vector2.new(294, 44)
			status.Position = UDim2.new(1, -14, 0, 10)
			status.Size = UDim2.fromOffset(math.min(294, math.max(252, viewport.X * 0.34)), 44)
			status.BackgroundTransparency = 1
			statusStroke.Transparency = 1
			metricRow.Position = UDim2.fromOffset(0, 0)
			metricRow.Size = UDim2.fromScale(1, 1)
			metricGrid.FillDirectionMaxCells = 3
			metricGrid.CellPadding = UDim2.fromOffset(5, 0)
			metricGrid.CellSize = UDim2.new(1 / 3, -4, 1, 0)
			progressTrack.Visible = false
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
			tutorial.Position = UDim2.new(0.5, 0, 1, -76)
			tutorial.Size = UDim2.fromOffset(math.max(220, math.min(320, viewport.X - 24)), 42)
			shopButton.Position = UDim2.new(1, -12, 0, 10)
			shopButton.Size = UDim2.fromOffset(84, 40)
			dropButton.Position = UDim2.new(1, -14, 1, -14)
			dropButton.Size = UDim2.fromOffset(150, 56)
			shopPanel.Position = UDim2.fromScale(0.5, 0.52)
			shopPanel.Size = UDim2.fromScale(0.94, 0.80)
			resultPanel.Size = UDim2.new(0.92, 0, 0, 220)
		else
			logo.Visible = true
			quickRail.Visible = true
			shopButton.Visible = false
			status.Visible = true
			mobileSummary.Visible = false
			mobileSummary.AnchorPoint = Vector2.new(1, 0)
			mobileSummary.Position = UDim2.new(1, -14, 0, 56)
			mobileSummary.Size = UDim2.fromOffset(232, 34)
			status.AnchorPoint = Vector2.new(0.5, 0)
			statusSize.MinSize = Vector2.new(300, 56)
			statusSize.MaxSize = Vector2.new(610, 72)
			status.Size = UDim2.new(1, -36, 0, 72)
			status.Position = UDim2.new(0.5, 0, 0, 16)
			status.BackgroundTransparency = 0.04
			statusStroke.Transparency = 0.2
			progressTrack.Visible = true
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
			tutorial.Position = UDim2.new(0.5, 0, 1, -72)
			tutorial.Size = UDim2.fromOffset(360, 48)
			quickRail.Position = UDim2.new(1, -18, 0, 18)
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
		setDailyVisible(false)
		setShopVisible(not shopPanel.Visible)
		dropButton.Visible = currentState == "ACTIVE" and not shopPanel.Visible
		tutorial.Visible = tutorial.Visible and not shopPanel.Visible
	end)

	for _, conceptButton in conceptShopButtons do
		conceptButton.Activated:Connect(function()
			setShopVisible(true)
			dropButton.Visible = false
		end)
	end

	quickShopButton.Activated:Connect(function()
		setDailyVisible(false)
		setShopVisible(not shopPanel.Visible)
		dropButton.Visible = currentState == "ACTIVE" and not shopPanel.Visible
	end)
	dailyQuickButton.Activated:Connect(function()
		setShopVisible(false)
		setDailyVisible(true)
		dropButton.Visible = false
	end)

	local function claimDailyReward()
		if not metaValue("dailyClaimable", false) then
			return
		end
		local result = claimDaily:InvokeServer()
		if result and result.meta then
			HudController.ApplyMeta(result.meta)
		end
	end

	dailyButton.Activated:Connect(claimDailyReward)
	dailyClaimButton.Activated:Connect(claimDailyReward)

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
