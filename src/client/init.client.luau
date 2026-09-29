--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local CameraController = require(script.controllers.CameraController)
local HudController = require(script.controllers.HudController)
local InputController = require(script.controllers.InputController)

local remotes = ReplicatedStorage:WaitForChild("PerfectDropRemotes")

HudController.Start(remotes)
CameraController.Start()
InputController.Start(remotes, HudController)

print("[Perfect Drop] Client initialized")
