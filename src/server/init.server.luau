--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local RemoteDefinitions = require(ReplicatedStorage.PerfectDrop.remotes.RemoteDefinitions)
local WorldService = require(script.services.WorldService)
local ProfileService = require(script.services.ProfileService)
local RoundService = require(script.services.RoundService)
local PurchaseService = require(script.services.PurchaseService)

local remotes = ReplicatedStorage:FindFirstChild("PerfectDropRemotes")
if remotes then
	remotes:Destroy()
end

remotes = Instance.new("Folder")
remotes.Name = "PerfectDropRemotes"
remotes.Parent = ReplicatedStorage

for name, className in RemoteDefinitions do
	local remote = Instance.new(className)
	remote.Name = name
	remote.Parent = remotes
end

WorldService.Start()
ProfileService.Start(remotes)
RoundService.Start(remotes)
PurchaseService.Start()

if RunService:IsStudio() then
	local studioQA = Instance.new("RemoteFunction")
	studioQA.Name = "StudioQA"
	studioQA.Parent = remotes
	studioQA.OnServerInvoke = function(player, action)
		if action == "receipt_idempotency" then
			local before = ProfileService.GetSnapshot(player)
			assert(before ~= nil, "profile unavailable")
			local purchaseId = ("studio-receipt-%d"):format(player.UserId)
			assert(ProfileService.ProcessReceipt(player, purchaseId, "Revive"), "first Studio receipt failed")
			assert(ProfileService.ProcessReceipt(player, purchaseId, "Revive"), "duplicate Studio receipt failed")
			local after = ProfileService.GetSnapshot(player)
			assert(after ~= nil, "profile unavailable after receipt")
			return {
				ok = true,
				beforeRevives = before.revives,
				afterRevives = after.revives,
			}
		elseif action == "respawn" then
			player:LoadCharacter()
			return { ok = true }
		end
		return { ok = false, reason = "unknown_action" }
	end
end

Players.PlayerRemoving:Connect(function(player)
	RoundService.RemovePlayer(player)
	WorldService.ReleaseLane(player)
end)

print("[Perfect Drop] Server initialized")
