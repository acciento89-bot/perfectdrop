--!strict

local ContextActionService = game:GetService("ContextActionService")
local UserInputService = game:GetService("UserInputService")

local InputController = {}

local lastInputAt = -math.huge
local busy = false

local function throttle(interval: number): boolean
	local now = os.clock()
	if busy or now - lastInputAt < interval then
		return false
	end
	lastInputAt = now
	return true
end

function InputController.Start(remotes: Folder, hud)
	local dropRequest = remotes:WaitForChild("DropRequest") :: RemoteFunction
	local retryRun = remotes:WaitForChild("RetryRun") :: RemoteFunction
	local useRevive = remotes:WaitForChild("UseRevive") :: RemoteFunction

	local function requestDrop()
		if hud.GetState() ~= "ACTIVE" or hud.IsOverlayOpen() or not throttle(0.14) then
			return
		end
		busy = true
		task.spawn(function()
			local ok, result = pcall(dropRequest.InvokeServer, dropRequest)
			if not ok then
				warn("[Perfect Drop] DropRequest failed: " .. tostring(result))
			end
			busy = false
		end)
	end

	local function requestRetry()
		local state = hud.GetState()
		if (state ~= "FAILED" and state ~= "COMPLETED") or not throttle(0.2) then
			return
		end
		busy = true
		task.spawn(function()
			local ok, result = pcall(retryRun.InvokeServer, retryRun)
			if not ok then
				warn("[Perfect Drop] RetryRun failed: " .. tostring(result))
			end
			busy = false
		end)
	end

	local function requestRevive()
		if hud.GetState() ~= "FAILED" or not throttle(0.2) then
			return
		end
		busy = true
		task.spawn(function()
			local ok, result = pcall(useRevive.InvokeServer, useRevive)
			if not ok then
				warn("[Perfect Drop] UseRevive failed: " .. tostring(result))
			end
			busy = false
		end)
	end

	ContextActionService:BindAction("PerfectDrop_Drop", function(_, inputState)
		if inputState == Enum.UserInputState.Begin then
			requestDrop()
		end
		return Enum.ContextActionResult.Sink
	end, false, Enum.KeyCode.Space, Enum.KeyCode.ButtonA)

	ContextActionService:BindAction("PerfectDrop_Retry", function(_, inputState)
		if inputState == Enum.UserInputState.Begin then
			requestRetry()
		end
		return Enum.ContextActionResult.Pass
	end, false, Enum.KeyCode.R, Enum.KeyCode.ButtonX)

	ContextActionService:BindAction("PerfectDrop_Shop", function(_, inputState)
		if inputState == Enum.UserInputState.Begin then
			hud.ToggleShop()
		end
		return Enum.ContextActionResult.Pass
	end, false, Enum.KeyCode.Tab, Enum.KeyCode.ButtonY)

	UserInputService.InputBegan:Connect(function(input, processed)
		if processed or hud.IsOverlayOpen() then
			return
		end
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			requestDrop()
		end
	end)

	hud.GetDropButton().Activated:Connect(requestDrop)
	hud.GetRetryButton().Activated:Connect(requestRetry)
	hud.GetReviveButton().Activated:Connect(requestRevive)
end

return InputController
