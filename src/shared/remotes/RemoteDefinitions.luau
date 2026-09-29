--!strict

local RemoteDefinitions = {
	DropRequest = "RemoteFunction",
	RetryRun = "RemoteFunction",
	RequestState = "RemoteFunction",
	RoundStateChanged = "RemoteEvent",
}

return table.freeze(RemoteDefinitions)
