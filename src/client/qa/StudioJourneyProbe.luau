--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
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
	if Workspace:GetAttribute("PerfectDropE2E") == false then
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
	pass("fresh round state")

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
	assert(afterThirty.ok and afterThirty.state.height == 30, "30-drop run height mismatch")
	assert(afterThirty.state.state == "ACTIVE", "30-drop run left ACTIVE state")
	pass(
		("30 consecutive mixed drops; PERFECT=%d GOOD=%d score=%d combo=%d"):format(
			perfectCount,
			goodCount,
			afterThirty.state.score,
			afterThirty.state.combo
		)
	)

	local run = getRun(player)
	local top = getTop(run)
	assert(top ~= nil, "top block missing after 30 drops")
	assert(top.Size.X >= 1.25 and top.Size.Z >= 1.25, "footprint fell below configured minimum")
	pass(("tower footprint valid %.2fx%.2f"):format(top.Size.X, top.Size.Z))

	waitForOffset(player, "MISS")
	local miss = dropRequest:InvokeServer()
	assert(miss and miss.ok and miss.grade == "MISS", "deliberate MISS did not fail")
	assert(miss.state.state == "FAILED", "MISS did not enter FAILED")
	pass("deliberate miss enters FAILED exactly once")

	local duplicate = dropRequest:InvokeServer()
	assert(duplicate and duplicate.ok == false, "drop accepted after FAILED")
	pass("post-failure drop rejected")

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
	pass("production HUD actions mounted")

	local metaBeforeDaily = requestMeta:InvokeServer()
	assert(metaBeforeDaily and metaBeforeDaily.ok, "RequestMetaState failed")
	assert(metaBeforeDaily.meta.dailyClaimable == true, "fresh daily reward should be claimable")
	local coinsBeforeDaily = metaBeforeDaily.meta.coins

	local daily = claimDaily:InvokeServer()
	assert(daily and daily.ok, "daily reward claim failed")
	assert(daily.meta.coins > coinsBeforeDaily, "daily reward did not add coins")
	local duplicateDaily = claimDaily:InvokeServer()
	assert(
		duplicateDaily and duplicateDaily.ok == false and duplicateDaily.reason == "already_claimed",
		"daily reward was not duplicate-safe"
	)
	pass("daily reward is granted once and duplicate-safe")

	local meta = requestMeta:InvokeServer()
	assert(meta and meta.ok, "meta unavailable after daily")
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

	local purchase = purchaseCosmetic:InvokeServer("block", "sunset")
	assert(purchase and purchase.ok, "sunset cosmetic purchase failed")
	assert(purchase.meta.ownedBlockThemes.sunset == true, "sunset ownership missing after purchase")
	local coinsAfterPurchase = purchase.meta.coins
	assert(coinsAfterPurchase == meta.meta.coins - 250, "sunset purchase price mismatch")

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
	assert(finalMeta.meta.bestHeight >= 30, "best height did not update authoritatively")
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
	task.defer(function()
		local ok, err = pcall(execute)
		if not ok then
			warn("[Perfect Drop E2E] FAIL " .. tostring(err))
		end
	end)
end

return StudioJourneyProbe
