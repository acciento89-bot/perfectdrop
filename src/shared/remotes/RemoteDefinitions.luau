--!strict

local RemoteDefinitions = {
	DropRequest = "RemoteFunction",
	RetryRun = "RemoteFunction",
	UseRevive = "RemoteFunction",
	RequestState = "RemoteFunction",
	RoundStateChanged = "RemoteEvent",

	RequestMetaState = "RemoteFunction",
	PurchaseCosmetic = "RemoteFunction",
	EquipCosmetic = "RemoteFunction",
	ClaimDaily = "RemoteFunction",
	MetaStateChanged = "RemoteEvent",
}

return table.freeze(RemoteDefinitions)
