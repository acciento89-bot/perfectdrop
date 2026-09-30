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

			if targetKind == "CENTER" and offset <= 0.45 then
				return
			elseif targetKind == "GOOD" and offset >= 1.0 and offset <= 1.7 then
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
	local requestState = remotes:WaitForChild("RequestState") :: RemoteFunction

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
		local target = if index % 3 == 0 then "GOOD" else "CENTER"
		waitForOffset(player, target)
		local result = dropRequest:InvokeServer()
		assert(result and result.ok, ("drop %d rejected"):format(index))
		assert(result.grade ~= "MISS", ("drop %d unexpectedly missed"):format(index))
		if result.grade == "PERFECT" then
			perfectCount += 1
		elseif result.grade == "GOOD" then
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

	print("[Perfect Drop E2E] COMPLETE core journey passed")
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
