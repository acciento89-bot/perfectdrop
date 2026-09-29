--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local RemoteDefinitions = require(ReplicatedStorage.PerfectDrop.remotes.RemoteDefinitions)
local WorldService = require(script.services.WorldService)
local RoundService = require(script.services.RoundService)

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
RoundService.Start(remotes)

Players.PlayerRemoving:Connect(function(player)
	RoundService.RemovePlayer(player)
	WorldService.ReleaseLane(player)
end)

print("[Perfect Drop] Server initialized")
