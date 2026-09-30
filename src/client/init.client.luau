--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local CameraController = require(script.controllers.CameraController)
local FeedbackController = require(script.controllers.FeedbackController)
local HudController = require(script.controllers.HudController)
local InputController = require(script.controllers.InputController)
local StudioJourneyProbe = require(script.qa.StudioJourneyProbe)

local remotes = ReplicatedStorage:WaitForChild("PerfectDropRemotes")

HudController.Start(remotes)
CameraController.Start()
FeedbackController.Start(remotes)
InputController.Start(remotes, HudController)
StudioJourneyProbe.Start()

print("[Perfect Drop] Client initialized")
