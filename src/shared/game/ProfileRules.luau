--!strict

local EconomyConfig = require("../config/EconomyConfig")
local CosmeticConfig = require("../config/CosmeticConfig")

export type Profile = {
	Version: number,
	Coins: number,
	BestScore: number,
	BestHeight: number,
	PlayerLevel: number,
	TowersCompleted: number,
	OwnedBlockThemes: { [string]: boolean },
	OwnedArenaThemes: { [string]: boolean },
	EquippedBlockTheme: string,
	EquippedArenaTheme: string,
	DailyLastClaimDay: number,
	DailyChallengeDay: number,
	DailyChallengePerfects: number,
	DailyChallengeRewarded: boolean,
	Achievements: { [string]: boolean },
	Consumables: {
		Revive: number,
		PerfectShield: number,
	},
	CoinBoostUntil: number,
	Receipts: { [string]: boolean },
}

local ProfileRules = {}

function ProfileRules.dayKey(unixTime: number): number
	return math.floor(unixTime / 86400)
end

function ProfileRules.defaultProfile(): Profile
	return {
		Version = EconomyConfig.ProfileVersion,
		Coins = 0,
		BestScore = 0,
		BestHeight = 0,
		PlayerLevel = 1,
		TowersCompleted = 0,
		OwnedBlockThemes = { [CosmeticConfig.DefaultBlockTheme] = true },
		OwnedArenaThemes = { [CosmeticConfig.DefaultArenaTheme] = true },
		EquippedBlockTheme = CosmeticConfig.DefaultBlockTheme,
		EquippedArenaTheme = CosmeticConfig.DefaultArenaTheme,
		DailyLastClaimDay = -1,
		DailyChallengeDay = -1,
		DailyChallengePerfects = 0,
		DailyChallengeRewarded = false,
		Achievements = {},
		Consumables = { Revive = 0, PerfectShield = 0 },
		CoinBoostUntil = 0,
		Receipts = {},
	}
end

local function copyBooleanMap(value): { [string]: boolean }
	local result = {}
	if type(value) == "table" then
		for key, owned in value do
			if type(key) == "string" and owned == true then
				result[key] = true
			end
		end
	end
	return result
end

function ProfileRules.normalize(value): Profile
	local profile = ProfileRules.defaultProfile()
	if type(value) ~= "table" then
		return profile
	end

	profile.Coins = math.max(0, math.floor(tonumber(value.Coins) or 0))
	profile.BestScore = math.max(0, math.floor(tonumber(value.BestScore) or 0))
	profile.BestHeight = math.max(0, math.floor(tonumber(value.BestHeight) or 0))
	profile.PlayerLevel = math.max(1, math.floor(tonumber(value.PlayerLevel) or 1))
	profile.TowersCompleted = math.max(0, math.floor(tonumber(value.TowersCompleted) or 0))
	profile.DailyLastClaimDay = math.floor(tonumber(value.DailyLastClaimDay) or -1)
	profile.DailyChallengeDay = math.floor(tonumber(value.DailyChallengeDay) or -1)
	profile.DailyChallengePerfects = math.max(0, math.floor(tonumber(value.DailyChallengePerfects) or 0))
	profile.DailyChallengeRewarded = value.DailyChallengeRewarded == true
	profile.CoinBoostUntil = math.max(0, math.floor(tonumber(value.CoinBoostUntil) or 0))
	profile.OwnedBlockThemes = copyBooleanMap(value.OwnedBlockThemes)
	profile.OwnedArenaThemes = copyBooleanMap(value.OwnedArenaThemes)
	profile.Achievements = copyBooleanMap(value.Achievements)
	profile.Receipts = copyBooleanMap(value.Receipts)

	profile.OwnedBlockThemes[CosmeticConfig.DefaultBlockTheme] = true
	profile.OwnedArenaThemes[CosmeticConfig.DefaultArenaTheme] = true

	local blockTheme = if type(value.EquippedBlockTheme) == "string"
		then value.EquippedBlockTheme
		else CosmeticConfig.DefaultBlockTheme
	if not profile.OwnedBlockThemes[blockTheme] or CosmeticConfig.BlockThemes[blockTheme] == nil then
		blockTheme = CosmeticConfig.DefaultBlockTheme
	end
	profile.EquippedBlockTheme = blockTheme

	local arenaTheme = if type(value.EquippedArenaTheme) == "string"
		then value.EquippedArenaTheme
		else CosmeticConfig.DefaultArenaTheme
	if not profile.OwnedArenaThemes[arenaTheme] or CosmeticConfig.ArenaThemes[arenaTheme] == nil then
		arenaTheme = CosmeticConfig.DefaultArenaTheme
	end
	profile.EquippedArenaTheme = arenaTheme

	if type(value.Consumables) == "table" then
		profile.Consumables.Revive = math.max(0, math.floor(tonumber(value.Consumables.Revive) or 0))
		profile.Consumables.PerfectShield = math.max(0, math.floor(tonumber(value.Consumables.PerfectShield) or 0))
	end

	profile.Version = EconomyConfig.ProfileVersion
	return profile
end

return ProfileRules
