--!strict

local ContextActionService = game:GetService("ContextActionService")
local UserInputService = game:GetService("UserInputService")

local InputController = {}

local lastInputAt = -math.huge
local busy = false

local function throttle(): boolean
	local now = os.clock()
	if busy or now - lastInputAt < 0.16 then
		return false
	end
	lastInputAt = now
	return true
end

function InputController.Start(remotes: Folder, hud)
	local dropRequest = remotes:WaitForChild("DropRequest") :: RemoteFunction
	local retryRun = remotes:WaitForChild("RetryRun") :: RemoteFunction

	local function requestDrop()
		if hud.GetState() ~= "ACTIVE" or not throttle() then
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
		if hud.GetState() ~= "FAILED" or not throttle() then
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

	ContextActionService:BindAction(
		"PerfectDrop_Drop",
		function(_, inputState)
			if inputState == Enum.UserInputState.Begin then
				requestDrop()
			end
			return Enum.ContextActionResult.Sink
		end,
		false,
		Enum.KeyCode.Space,
		Enum.KeyCode.ButtonA
	)

	UserInputService.InputBegan:Connect(function(input, processed)
		if processed then
			return
		end
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			requestDrop()
		end
	end)

	hud.GetDropButton().Activated:Connect(requestDrop)
	hud.GetRetryButton().Activated:Connect(requestRetry)
end

return InputController
