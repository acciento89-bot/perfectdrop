--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local SoundService = game:GetService("SoundService")
local Workspace = game:GetService("Workspace")

local StudioJourneyProbe = {}

local function pass(message: string)
	print("[Perfect Drop E2E] PASS " .. message)
end

local function waitFor(predicate: () -> boolean, timeoutSeconds: number, message: string)
	local deadline = os.clock() + timeoutSeconds
	while os.clock() < deadline do
		if predicate() then
			return
		end
		task.wait()
	end
	error("Timed out: " .. message)
end

local function getRun(player: Player): Folder
	local world = Workspace:WaitForChild("PerfectDropWorld", 10)
	assert(world and world:IsA("Folder"), "PerfectDropWorld missing")
	local area = world:WaitForChild(tostring(player.UserId), 10)
	assert(area and area:IsA("Folder"), "player area missing")
	local run = area:WaitForChild("Run", 10)
	assert(run and run:IsA("Folder"), "run folder missing")
	return run
end

local function getTop(run: Folder): BasePart?
	local bestIndex = -1
	local best: BasePart? = nil
	for _, child in run:GetChildren() do
		if child:IsA("BasePart") then
			local indexText = string.match(child.Name, "^Stack_(%d+)$")
			if indexText then
				local index = tonumber(indexText)
				if index and index > bestIndex then
					bestIndex = index
					best = child
				end
			end
		end
	end
	return best
end

local function axisValues(block: BasePart, axis: string): (number, number)
	if axis == "X" then
		return block.Position.X, block.Size.X
	end
	return block.Position.Z, block.Size.Z
end

local function waitForOffset(player: Player, targetKind: string)
	local run = getRun(player)
	local requestState = ReplicatedStorage.PerfectDropRemotes.RequestState :: RemoteFunction
	local deadline = os.clock() + 5

	while os.clock() < deadline do
		local stateResult = requestState:InvokeServer()
		assert(stateResult and stateResult.ok, "RequestState failed")
		local axis = stateResult.state.axis
		local moving = run:FindFirstChild("MovingBlock")
		local top = getTop(run)
		if moving and moving:IsA("BasePart") and top then
			local movingCenter = axisValues(moving, axis)
			local topCenter, topSize = axisValues(top, axis)
			local offset = math.abs(movingCenter - topCenter)

			if targetKind == "PERFECT" and offset <= 0.30 then
				return
			elseif targetKind == "GOOD" and offset >= 1.20 and offset <= 1.60 then
				return
			elseif targetKind == "MISS" and offset >= topSize + 0.8 then
				return
			end
		end
		task.wait()
	end

	error("Could not find timing window for " .. targetKind)
end

local function execute()
	assert(RunService:IsStudio(), "StudioJourneyProbe is Studio-only")
	if Workspace:GetAttribute("PerfectDropE2E") ~= true then
		return
	end

	local player = Players.LocalPlayer
	local remotes = ReplicatedStorage:WaitForChild("PerfectDropRemotes")
	local dropRequest = remotes:WaitForChild("DropRequest") :: RemoteFunction
	local retryRun = remotes:WaitForChild("RetryRun") :: RemoteFunction
	local useRevive = remotes:WaitForChild("UseRevive") :: RemoteFunction
	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction
	local requestMeta = remotes:WaitForChild("RequestMetaState") :: RemoteFunction
	local claimDaily = remotes:WaitForChild("ClaimDaily") :: RemoteFunction
	local purchaseCosmetic = remotes:WaitForChild("PurchaseCosmetic") :: RemoteFunction
	local equipCosmetic = remotes:WaitForChild("EquipCosmetic") :: RemoteFunction
	local studioQA = remotes:WaitForChild("StudioQA") :: RemoteFunction

	waitFor(function()
		return player.Character ~= nil
	end, 10, "character spawn")
	pass("character spawned")

	local camera = Workspace.CurrentCamera
	assert(camera ~= nil, "camera missing")
	waitFor(function()
		return camera.CameraType == Enum.CameraType.Scriptable
	end, 5, "follow camera")
	pass("follow camera active")

	local initial = requestState:InvokeServer()
	assert(initial and initial.ok and initial.state.state == "ACTIVE", "initial round not ACTIVE")
	assert(initial.state.height == 0 and initial.state.score == 0, "initial round not clean")
	local startingPlayerLevel = initial.state.playerLevel or 1
	local startingTowersCompleted = initial.state.towersCompleted or 0
	pass(("fresh round state; starting level=%d towers=%d"):format(startingPlayerLevel, startingTowersCompleted))

	local feedbackSound = SoundService:WaitForChild("PerfectDropPing", 5)
	assert(feedbackSound and feedbackSound:IsA("Sound"), "feedback sound missing")
	waitFor(function()
		return feedbackSound.IsLoaded and feedbackSound.TimeLength > 0
	end, 8, "Roblox-owned feedback audio load")
	pass("Roblox-owned feedback audio loaded")

	local world = Workspace:FindFirstChild("PerfectDropWorld")
	assert(world and world:IsA("Folder"), "production world missing")
	assert(world:FindFirstChild("SunDisc") ~= nil, "sunset focal light missing")
	local area = world:FindFirstChild(tostring(player.UserId))
	assert(area and area:IsA("Folder"), "player production area missing")
	assert(area:FindFirstChild("StageMilestone_10") ~= nil, "stage 10 world marker missing")
	assert(area:FindFirstChild("StageMilestone_20") ~= nil, "stage 20 world marker missing")
	assert(area:FindFirstChild("StageMilestone_30") ~= nil, "stage 30 world marker missing")
	assert(area:FindFirstChild("FinishCrown") ~= nil, "finish crown missing")
	local artRun = getRun(player)
	local artMoving = artRun:FindFirstChild("MovingBlock")
	assert(artMoving and artMoving:IsA("BasePart"), "moving block missing for art gate")
	assert(artMoving:FindFirstChild("ActiveGlow") ~= nil, "active block highlight missing")
	assert(artMoving:FindFirstChild("TopFinish") ~= nil, "block top treatment missing")
	assert(artRun:FindFirstChild("LandingGuide") ~= nil, "Perfect landing guide missing")
	pass("production art landmarks, active-block treatment and landing guide mounted")
	if Workspace:GetAttribute("PerfectDropCapture") == true then
		print("[Perfect Drop Capture] GAMEPLAY")
		task.wait(4)
	end

	local perfectCount = 0
	local goodCount = 0
	for index = 1, 30 do
		local target = if index <= 6 then "GOOD" else "PERFECT"
		waitForOffset(player, target)
		local result = dropRequest:InvokeServer()
		assert(result and result.ok, ("drop %d rejected"):format(index))
		assert(result.grade ~= "MISS", ("drop %d unexpectedly missed while targeting %s"):format(index, target))
		if result.grade == "PERFECT" then
			perfectCount += 1
		else
			goodCount += 1
		end
		assert(result.state.height == index, ("drop %d height mismatch"):format(index))
		task.wait(0.12)
	end

	assert(perfectCount >= 5, ("too few Perfect runtime hits: %d"):format(perfectCount))
	assert(goodCount >= 5, ("too few Good runtime hits: %d"):format(goodCount))
	local afterThirty = requestState:InvokeServer()
	assert(afterThirty.ok and afterThirty.state.height == 30, "30-stage tower height mismatch")
	assert(afterThirty.state.state == "COMPLETED", "stage 30 did not complete the tower")
	assert(
		afterThirty.state.playerLevel == startingPlayerLevel + 1,
		"player level did not increase exactly once after tower completion"
	)
	assert(
		afterThirty.state.towersCompleted == startingTowersCompleted + 1,
		"completed tower count did not increase exactly once"
	)
	pass(
		("30-stage tower complete; PERFECT=%d GOOD=%d score=%d playerLevel=%d"):format(
			perfectCount,
			goodCount,
			afterThirty.state.score,
			afterThirty.state.playerLevel
		)
	)

	local run = getRun(player)
	local top = getTop(run)
	assert(top ~= nil, "top block missing after stage 30")
	assert(top.Size.X >= 1.75 and top.Size.Z >= 1.75, "footprint fell below configured minimum")
	assert(top.Size.X < 10 and top.Size.Z < 10, "tower did not become narrower over 30 stages")
	assert(run:FindFirstChild("MovingBlock") == nil, "moving block continued after stage 30")
	pass(("tower narrows safely to %.2fx%.2f and stops at 30/30"):format(top.Size.X, top.Size.Z))

	local completionHud = player:WaitForChild("PlayerGui"):WaitForChild("PerfectDropHUD", 5)
	assert(completionHud and completionHud:IsA("ScreenGui"), "PerfectDropHUD missing at completion")
	waitFor(function()
		local panel = completionHud:FindFirstChild("ResultPanel", true)
		local retryButton = completionHud:FindFirstChild("RetryButton", true)
		local progress = completionHud:FindFirstChild("StageProgress", true)
		local fill = progress and progress:FindFirstChild("Fill")
		return panel ~= nil
			and panel:IsA("Frame")
			and panel.Visible
			and retryButton ~= nil
			and retryButton:IsA("TextButton")
			and retryButton.Text == "NEXT TOWER"
			and fill ~= nil
			and fill:IsA("Frame")
			and fill.Size.X.Scale >= 0.999
	end, 3, "30/30 completion HUD")
	pass("30/30 HUD shows full progress and NEXT TOWER completion state")
	if Workspace:GetAttribute("PerfectDropCapture") == true then
		print("[Perfect Drop Capture] COMPLETE")
		task.wait(4)
	end

	assert(
		(camera.CFrame.Position - player.Character.HumanoidRootPart.Position).Magnitude < 60,
		"camera drifted too far away at stage 30"
	)
	pass("stage-30 camera remains close enough for mobile readability")

	local blockedAfterComplete = dropRequest:InvokeServer()
	assert(blockedAfterComplete and blockedAfterComplete.ok == false, "drop accepted after COMPLETED")
	pass("stage 30 blocks further drops")

	local completedRun = run
	local completedDescendantCount = #completedRun:GetDescendants()
	assert(
		completedDescendantCount < 260,
		("completed tower leaked too many descendants: %d"):format(completedDescendantCount)
	)

	local nextTower = retryRun:InvokeServer()
	assert(nextTower and nextTower.ok, "NEXT TOWER request rejected")
	assert(nextTower.state.state == "ACTIVE" and nextTower.state.height == 0, "next tower did not reset to stage 0")
	waitFor(function()
		return completedRun.Parent == nil
	end, 3, "completed run cleanup")
	local freshRun = getRun(player)
	assert(#freshRun:GetDescendants() < 50, "fresh tower contains leaked prior-run geometry")
	pass("NEXT TOWER cleans prior tower and starts bounded fresh state while keeping player level")

	waitForOffset(player, "MISS")
	local miss = dropRequest:InvokeServer()
	assert(miss and miss.ok and miss.grade == "MISS", "deliberate MISS did not fail")
	assert(miss.state.state == "FAILED", "MISS did not enter FAILED")
	pass("deliberate miss enters FAILED exactly once")

	local duplicate = dropRequest:InvokeServer()
	assert(duplicate and duplicate.ok == false, "drop accepted after FAILED")
	pass("post-failure drop rejected")

	task.wait(0.4)
	local retry = retryRun:InvokeServer()
	assert(retry and retry.ok, "RetryRun rejected")
	assert(retry.state.state == "ACTIVE", "retry did not restore ACTIVE")
	assert(retry.state.height == 0 and retry.state.score == 0 and retry.state.combo == 0, "retry did not reset run")
	pass("retry creates clean ACTIVE run")

	local runAfterRetry = getRun(player)
	waitFor(function()
		return runAfterRetry:FindFirstChild("MovingBlock") ~= nil and getTop(runAfterRetry) ~= nil
	end, 5, "moving block after retry")
	pass("moving block restored after retry")

	local hud = player:WaitForChild("PlayerGui"):WaitForChild("PerfectDropHUD", 5)
	assert(hud and hud:IsA("ScreenGui"), "PerfectDropHUD missing")
	assert(hud:FindFirstChild("ShopButton", true) ~= nil, "Shop button missing")
	assert(hud:FindFirstChild("DropButton", true) ~= nil, "Drop button missing")
	assert(hud:FindFirstChild("RetryButton", true) ~= nil, "Retry button missing")
	assert(hud:FindFirstChild("AudioButton", true) ~= nil, "Audio setting button missing")

	local dropUi = hud:FindFirstChild("DropButton", true)
	local shopUi = hud:FindFirstChild("ShopButton", true)
	local retryUi = hud:FindFirstChild("RetryButton", true)
	local audioUi = hud:FindFirstChild("AudioButton", true)
	assert(
		dropUi and dropUi:IsA("GuiButton") and dropUi.AbsoluteSize.X >= 64 and dropUi.AbsoluteSize.Y >= 48,
		"Drop touch target too small"
	)
	assert(
		shopUi and shopUi:IsA("GuiButton") and shopUi.AbsoluteSize.X >= 64 and shopUi.AbsoluteSize.Y >= 44,
		"Shop touch target too small"
	)
	assert(retryUi and retryUi:IsA("GuiButton") and retryUi.AbsoluteSize.Y >= 48, "Retry touch target too small")
	assert(
		audioUi and audioUi:IsA("GuiButton") and audioUi.AbsoluteSize.Y >= 44,
		"Audio setting touch target too small"
	)
	pass(("production HUD actions mounted; viewport=%dx%d"):format(camera.ViewportSize.X, camera.ViewportSize.Y))
	if Workspace:GetAttribute("PerfectDropCapture") == true then
		local shopPanel = hud:FindFirstChild("ShopPanel", true)
		assert(shopPanel and shopPanel:IsA("Frame"), "ShopPanel missing for capture")
		shopPanel.Visible = true
		print("[Perfect Drop Capture] SHOP")
		task.wait(4)
		shopPanel.Visible = false
	end

	local metaBeforeDaily = requestMeta:InvokeServer()
	assert(metaBeforeDaily and metaBeforeDaily.ok, "RequestMetaState failed")
	if metaBeforeDaily.meta.dailyClaimable == true then
		local coinsBeforeDaily = metaBeforeDaily.meta.coins
		local daily = claimDaily:InvokeServer()
		assert(daily and daily.ok, "daily reward claim failed")
		assert(daily.meta.coins > coinsBeforeDaily, "daily reward did not add coins")
		task.wait(0.8)
		local duplicateDaily = claimDaily:InvokeServer()
		assert(
			duplicateDaily and duplicateDaily.ok == false and duplicateDaily.reason == "already_claimed",
			"daily reward was not duplicate-safe"
		)
		pass("daily reward granted once and duplicate-safe")
	else
		local duplicateDaily = claimDaily:InvokeServer()
		assert(
			duplicateDaily and duplicateDaily.ok == false and duplicateDaily.reason == "already_claimed",
			"persisted claimed daily unexpectedly became claimable"
		)
		pass("persisted daily-claim state remains duplicate-safe")
	end

	local meta = requestMeta:InvokeServer()
	assert(meta and meta.ok, "meta unavailable after daily")
	local ownsSunset = meta.meta.ownedBlockThemes.sunset == true
	if not ownsSunset then
		local earnSafety = 0
		while meta.meta.coins < 250 do
			earnSafety += 1
			assert(earnSafety <= 20, "could not earn enough coins for cosmetic QA")
			waitForOffset(player, "PERFECT")
			local result = dropRequest:InvokeServer()
			assert(result and result.ok and result.grade ~= "MISS", "coin-earning QA drop failed")
			task.wait(0.12)
			meta = requestMeta:InvokeServer()
		end
		pass(("legitimate economy earned %d coins"):format(meta.meta.coins))

		local coinsBeforePurchase = meta.meta.coins
		local purchase = purchaseCosmetic:InvokeServer("block", "sunset")
		assert(purchase and purchase.ok, "sunset cosmetic purchase failed")
		assert(purchase.meta.ownedBlockThemes.sunset == true, "sunset ownership missing after purchase")
		assert(purchase.meta.coins == coinsBeforePurchase - 250, "sunset purchase price mismatch")
		pass("cosmetic purchase is server-authoritative")
	else
		pass("persisted Sunset cosmetic ownership restored")
	end

	local equipped = equipCosmetic:InvokeServer("block", "sunset")
	assert(equipped and equipped.ok, "sunset cosmetic equip failed")
	assert(equipped.meta.equippedBlockTheme == "sunset", "sunset did not equip")
	assert(player:GetAttribute("PDBlockTheme") == "sunset", "equipped block theme attribute did not sync")
	pass("cosmetic purchase and equip are server-authoritative")

	waitForOffset(player, "PERFECT")
	local themedDrop = dropRequest:InvokeServer()
	assert(themedDrop and themedDrop.ok and themedDrop.grade ~= "MISS", "themed drop failed")
	task.wait(0.2)
	local themedRun = getRun(player)
	local movingAfterTheme = themedRun:FindFirstChild("MovingBlock")
	assert(movingAfterTheme and movingAfterTheme:IsA("BasePart"), "moving block missing after cosmetic equip")
	local expectedTheme = Color3.fromRGB(228, 114, 77)
	local actualTheme = movingAfterTheme.Color
	local colorDelta = math.abs(actualTheme.R - expectedTheme.R)
		+ math.abs(actualTheme.G - expectedTheme.G)
		+ math.abs(actualTheme.B - expectedTheme.B)
	assert(colorDelta < 0.02, "equipped Sunset theme is not visible on moving block")
	pass("equipped cosmetic changes live gameplay presentation")

	local receipt = studioQA:InvokeServer("receipt_idempotency")
	assert(receipt and receipt.ok, "Studio receipt idempotency probe failed")
	assert(receipt.afterRevives == receipt.beforeRevives + 1, "duplicate receipt granted more or less than one revive")
	pass("duplicate Developer Product receipt grants exactly once")

	waitForOffset(player, "MISS")
	local missForRevive = dropRequest:InvokeServer()
	assert(missForRevive and missForRevive.ok and missForRevive.grade == "MISS", "revive setup miss failed")
	local revive = useRevive:InvokeServer()
	assert(revive and revive.ok, "revive consumption failed")
	assert(
		revive.state.state == "ACTIVE" and revive.state.assisted == true,
		"revive did not resume assisted ACTIVE state"
	)
	assert(revive.state.revives == receipt.beforeRevives, "revive inventory did not decrement exactly once")
	pass("revive restores last valid stack without competitive run state")

	local oldCharacter = player.Character
	local respawn = studioQA:InvokeServer("respawn")
	assert(respawn and respawn.ok, "Studio respawn request failed")
	waitFor(function()
		return player.Character ~= nil and player.Character ~= oldCharacter
	end, 8, "character respawn")
	local newCharacter = player.Character
	assert(newCharacter ~= nil, "new character missing")
	local newRoot = newCharacter:WaitForChild("HumanoidRootPart", 5)
	assert(newRoot and newRoot:IsA("BasePart"), "respawn root missing")
	waitFor(function()
		return camera.CameraType == Enum.CameraType.Scriptable
			and (camera.CFrame.Position - newRoot.Position).Magnitude < 80
	end, 5, "camera recovery after respawn")
	pass("respawn restores character and follow camera")

	local finalMeta = requestMeta:InvokeServer()
	assert(finalMeta and finalMeta.ok, "final meta unavailable")
	assert(finalMeta.meta.bestScore >= afterThirty.state.score, "best score did not update authoritatively")
	assert(finalMeta.meta.bestHeight >= 30, "best stage did not update authoritatively")
	assert(finalMeta.meta.playerLevel >= 2, "persistent player level did not retain tower completion")
	assert(finalMeta.meta.towersCompleted >= 1, "persistent completed tower count missing")
	assert(finalMeta.meta.achievements.first_tower == true, "tower completion achievement missing")
	assert(finalMeta.meta.dailyChallengeRewarded == true, "daily Perfect challenge did not reward")
	assert(finalMeta.meta.achievements.first_perfect == true, "first Perfect achievement missing")
	assert(finalMeta.meta.achievements.height_25 == true, "height 25 achievement missing")
	pass(
		("progression meta valid: best=%d height=%d coins=%d"):format(
			finalMeta.meta.bestScore,
			finalMeta.meta.bestHeight,
			finalMeta.meta.coins
		)
	)

	print("[Perfect Drop E2E] COMPLETE full player journey passed")
end

function StudioJourneyProbe.Start()
	if not RunService:IsStudio() then
		return
	end

	local started = false
	local function tryStart()
		if started or Workspace:GetAttribute("PerfectDropE2E") ~= true then
			return
		end
		started = true
		task.defer(function()
			local ok, err = pcall(execute)
			if not ok then
				warn("[Perfect Drop E2E] FAIL " .. tostring(err))
			end
		end)
	end

	Workspace:GetAttributeChangedSignal("PerfectDropE2E"):Connect(tryStart)
	tryStart()
end

return StudioJourneyProbe
