--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local CosmeticConfig = require(ReplicatedStorage.PerfectDrop.config.CosmeticConfig)
local EconomyConfig = require(ReplicatedStorage.PerfectDrop.config.EconomyConfig)
local RetentionConfig = require(ReplicatedStorage.PerfectDrop.config.RetentionConfig)
local ProfileRules = require(ReplicatedStorage.PerfectDrop.game.ProfileRules)
local RetentionRules = require(ReplicatedStorage.PerfectDrop.game.RetentionRules)
local RateLimiter = require(ReplicatedStorage.PerfectDrop.security.RateLimiter)

local ProfileService = {}

local dataStore = nil
if game.GameId ~= 0 then
	local ok, storeOrError = pcall(function()
		return DataStoreService:GetDataStore(
			("%s_v%d"):format(EconomyConfig.DataStoreBaseName, EconomyConfig.DataStoreVersion)
		)
	end)
	if ok then
		dataStore = storeOrError
	elseif not RunService:IsStudio() then
		error(("Perfect Drop DataStore unavailable: %s"):format(tostring(storeOrError)))
	end
end

local profiles: { [Player]: any } = {}
local dirty: { [Player]: boolean } = {}
local metaChanged: RemoteEvent?

local purchaseLimiter = RateLimiter.new(0.25)
local equipLimiter = RateLimiter.new(0.15)
local dailyLimiter = RateLimiter.new(0.75)

local function keyFor(player: Player): string
	return ("player_%d"):format(player.UserId)
end

local function currentDay(): number
	return ProfileRules.dayKey(os.time())
end

local function normalizeDaily(profile)
	local day, perfects, rewarded = RetentionRules.normalizeChallengeDay(
		profile.DailyChallengeDay,
		currentDay(),
		profile.DailyChallengePerfects,
		profile.DailyChallengeRewarded
	)
	profile.DailyChallengeDay = day
	profile.DailyChallengePerfects = perfects
	profile.DailyChallengeRewarded = rewarded
end

local function syncAttributes(player: Player, profile)
	normalizeDaily(profile)
	player:SetAttribute("PDCoins", profile.Coins)
	player:SetAttribute("PDBestScore", profile.BestScore)
	player:SetAttribute("PDBestHeight", profile.BestHeight)
	player:SetAttribute("PDPlayerLevel", profile.PlayerLevel)
	player:SetAttribute("PDTowersCompleted", profile.TowersCompleted)
	player:SetAttribute("PDBlockTheme", profile.EquippedBlockTheme)
	player:SetAttribute("PDArenaTheme", profile.EquippedArenaTheme)
	player:SetAttribute("PDRevives", profile.Consumables.Revive)
	player:SetAttribute("PDPerfectShields", profile.Consumables.PerfectShield)
	player:SetAttribute("PDDailyPerfects", profile.DailyChallengePerfects)
	player:SetAttribute("PDDailyRewarded", profile.DailyChallengeRewarded)
end

local function snapshot(profile)
	normalizeDaily(profile)
	return {
		coins = profile.Coins,
		bestScore = profile.BestScore,
		bestHeight = profile.BestHeight,
		playerLevel = profile.PlayerLevel,
		towersCompleted = profile.TowersCompleted,
		ownedBlockThemes = profile.OwnedBlockThemes,
		ownedArenaThemes = profile.OwnedArenaThemes,
		equippedBlockTheme = profile.EquippedBlockTheme,
		equippedArenaTheme = profile.EquippedArenaTheme,
		dailyClaimable = RetentionRules.canClaimDaily(profile.DailyLastClaimDay, currentDay()),
		dailyPerfects = profile.DailyChallengePerfects,
		dailyRequired = RetentionConfig.DailyChallenge.PerfectsRequired,
		dailyChallengeRewarded = profile.DailyChallengeRewarded,
		achievements = profile.Achievements,
		revives = profile.Consumables.Revive,
		perfectShields = profile.Consumables.PerfectShield,
		coinBoostUntil = profile.CoinBoostUntil,
	}
end

local function emit(player: Player)
	local profile = profiles[player]
	if profile and metaChanged then
		syncAttributes(player, profile)
		metaChanged:FireClient(player, snapshot(profile))
	end
end

local function loadPlayer(player: Player)
	if profiles[player] then
		return
	end

	local loaded = nil
	local lastError = nil
	if dataStore then
		for attempt = 1, 3 do
			local ok, value = pcall(dataStore.GetAsync, dataStore, keyFor(player))
			if ok then
				loaded = value
				lastError = nil
				break
			end
			lastError = value
			task.wait(attempt * 0.5)
		end
	elseif RunService:IsStudio() then
		player:SetAttribute("PDStudioMemoryProfile", true)
	end

	if lastError ~= nil and not RunService:IsStudio() then
		warn(("[Perfect Drop] profile load failed for %d: %s"):format(player.UserId, tostring(lastError)))
		player:Kick("Your Perfect Drop data could not be loaded safely. Please rejoin.")
		return
	end

	if lastError ~= nil then
		warn(("[Perfect Drop] Studio profile fallback for %d: %s"):format(player.UserId, tostring(lastError)))
	end

	local profile = ProfileRules.normalize(loaded)
	profiles[player] = profile
	dirty[player] = false
	syncAttributes(player, profile)
	emit(player)
end

local function savePlayer(player: Player): boolean
	local profile = profiles[player]
	if not profile then
		return true
	end
	if not dirty[player] and player.Parent ~= nil then
		return true
	end

	if not dataStore then
		if RunService:IsStudio() then
			dirty[player] = false
			return true
		end
		return false
	end

	local payload = ProfileRules.normalize(profile)
	local ok, err = pcall(function()
		dataStore:UpdateAsync(keyFor(player), function()
			return payload
		end)
	end)
	if not ok then
		warn(("[Perfect Drop] profile save failed for %d: %s"):format(player.UserId, tostring(err)))
		return false
	end
	dirty[player] = false
	return true
end

local function markDirty(player: Player)
	if profiles[player] then
		dirty[player] = true
		syncAttributes(player, profiles[player])
		emit(player)
	end
end

local function awardAchievement(profile, achievementId: string)
	if profile.Achievements[achievementId] then
		return
	end
	local definition = RetentionConfig.Achievements[achievementId]
	if not definition then
		return
	end
	profile.Achievements[achievementId] = true
	profile.Coins += definition.RewardCoins
end

function ProfileService.WaitForProfile(player: Player)
	local deadline = os.clock() + 12
	while player.Parent and not profiles[player] and os.clock() < deadline do
		task.wait()
	end
	return profiles[player]
end

function ProfileService.Get(player: Player)
	return profiles[player]
end

function ProfileService.GetSnapshot(player: Player)
	local profile = profiles[player]
	if not profile then
		return nil
	end
	return snapshot(profile)
end

function ProfileService.RecordDrop(
	player: Player,
	grade: string,
	score: number,
	height: number,
	combo: number,
	competitive: boolean
): number
	local profile = profiles[player]
	if not profile then
		return 0
	end

	normalizeDaily(profile)
	local reward = if grade == "PERFECT" then EconomyConfig.CoinsPerPerfect else EconomyConfig.CoinsPerGood
	if profile.CoinBoostUntil > os.time() then
		reward *= 2
	end
	if height > 0 and height % EconomyConfig.HeightBonusEvery == 0 then
		reward += EconomyConfig.HeightBonusCoins
	end
	profile.Coins += reward
	if competitive then
		profile.BestScore = math.max(profile.BestScore, score)
		profile.BestHeight = math.max(profile.BestHeight, height)
	end

	if grade == "PERFECT" then
		profile.DailyChallengePerfects += 1
		awardAchievement(profile, "first_perfect")
		if combo >= 5 then
			awardAchievement(profile, "combo_5")
		end
	end
	if height >= 10 then
		awardAchievement(profile, "height_10")
	end
	if height >= 25 then
		awardAchievement(profile, "height_25")
	end

	if
		not profile.DailyChallengeRewarded
		and profile.DailyChallengePerfects >= RetentionConfig.DailyChallenge.PerfectsRequired
	then
		profile.DailyChallengeRewarded = true
		profile.Coins += RetentionConfig.DailyChallenge.RewardCoins
	end

	markDirty(player)
	return reward
end

function ProfileService.CompleteTower(player: Player, competitive: boolean): number
	local profile = profiles[player]
	if not profile then
		return 0
	end

	profile.TowersCompleted += 1
	profile.PlayerLevel += 1
	profile.Coins += EconomyConfig.TowerCompletionCoins
	if competitive then
		awardAchievement(profile, "first_tower")
		if profile.PlayerLevel >= 5 then
			awardAchievement(profile, "level_5")
		end
	end
	markDirty(player)
	return EconomyConfig.TowerCompletionCoins
end

function ProfileService.ClaimDaily(player: Player)
	local profile = profiles[player]
	if not profile then
		return { ok = false, reason = "profile_unavailable" }
	end
	local day = currentDay()
	if not RetentionRules.canClaimDaily(profile.DailyLastClaimDay, day) then
		return { ok = false, reason = "already_claimed", meta = snapshot(profile) }
	end
	profile.DailyLastClaimDay = day
	profile.Coins += RetentionConfig.DailyRewardCoins
	markDirty(player)
	return { ok = true, rewardCoins = RetentionConfig.DailyRewardCoins, meta = snapshot(profile) }
end

function ProfileService.PurchaseCosmetic(player: Player, category: string, cosmeticId: string)
	local profile = profiles[player]
	if not profile then
		return { ok = false, reason = "profile_unavailable" }
	end

	local definitions
	local owned
	if category == "block" then
		definitions = CosmeticConfig.BlockThemes
		owned = profile.OwnedBlockThemes
	elseif category == "arena" then
		definitions = CosmeticConfig.ArenaThemes
		owned = profile.OwnedArenaThemes
	else
		return { ok = false, reason = "invalid_category" }
	end

	local definition = definitions[cosmeticId]
	if not definition then
		return { ok = false, reason = "unknown_cosmetic" }
	end
	if owned[cosmeticId] then
		return { ok = true, alreadyOwned = true, meta = snapshot(profile) }
	end
	if profile.Coins < definition.PriceCoins then
		return { ok = false, reason = "insufficient_coins", meta = snapshot(profile) }
	end

	profile.Coins -= definition.PriceCoins
	owned[cosmeticId] = true
	markDirty(player)
	return { ok = true, meta = snapshot(profile) }
end

function ProfileService.EquipCosmetic(player: Player, category: string, cosmeticId: string)
	local profile = profiles[player]
	if not profile then
		return { ok = false, reason = "profile_unavailable" }
	end

	if category == "block" then
		if not profile.OwnedBlockThemes[cosmeticId] or CosmeticConfig.BlockThemes[cosmeticId] == nil then
			return { ok = false, reason = "not_owned" }
		end
		profile.EquippedBlockTheme = cosmeticId
	elseif category == "arena" then
		if not profile.OwnedArenaThemes[cosmeticId] or CosmeticConfig.ArenaThemes[cosmeticId] == nil then
			return { ok = false, reason = "not_owned" }
		end
		profile.EquippedArenaTheme = cosmeticId
	else
		return { ok = false, reason = "invalid_category" }
	end

	markDirty(player)
	return { ok = true, meta = snapshot(profile) }
end

function ProfileService.ConsumeRevive(player: Player): boolean
	local profile = profiles[player]
	if not profile or profile.Consumables.Revive <= 0 then
		return false
	end
	profile.Consumables.Revive -= 1
	markDirty(player)
	return true
end

function ProfileService.ConsumePerfectShield(player: Player): boolean
	local profile = profiles[player]
	if not profile or profile.Consumables.PerfectShield <= 0 then
		return false
	end
	profile.Consumables.PerfectShield -= 1
	markDirty(player)
	return true
end

local function applyProduct(profile, productName: string, nowUnix: number): boolean
	if productName == "Revive" then
		profile.Consumables.Revive += 1
	elseif productName == "PerfectShield" then
		profile.Consumables.PerfectShield += 1
	elseif productName == "CoinBoost15m" then
		profile.CoinBoostUntil = math.max(nowUnix, profile.CoinBoostUntil) + 15 * 60
	else
		return false
	end
	return true
end

function ProfileService.ProcessReceipt(player: Player, purchaseId: string, productName: string): boolean
	local profile = profiles[player]
	if not profile then
		return false
	end
	if profile.Receipts[purchaseId] then
		return true
	end

	local nowUnix = os.time()

	-- Flush unsaved progression before the atomic receipt transaction.
	-- Otherwise the receipt UpdateAsync could reload an older stored profile
	-- and overwrite fresh tower/level/currency progress in memory.
	if dataStore and dirty[player] and not savePlayer(player) then
		warn(("[Perfect Drop] receipt pre-save failed for %d"):format(player.UserId))
		return false
	end

	if not dataStore then
		if not RunService:IsStudio() then
			return false
		end
		if not applyProduct(profile, productName, nowUnix) then
			return false
		end
		profile.Receipts[purchaseId] = true
		markDirty(player)
		return true
	end

	local updatedProfile = nil
	local ok, err = pcall(function()
		updatedProfile = dataStore:UpdateAsync(keyFor(player), function(stored)
			local storedProfile = ProfileRules.normalize(stored)
			if storedProfile.Receipts[purchaseId] then
				return storedProfile
			end
			if not applyProduct(storedProfile, productName, nowUnix) then
				return nil
			end
			storedProfile.Receipts[purchaseId] = true
			return storedProfile
		end)
	end)
	if not ok or updatedProfile == nil then
		warn(("[Perfect Drop] atomic receipt failed for %d: %s"):format(player.UserId, tostring(err)))
		return false
	end

	profiles[player] = ProfileRules.normalize(updatedProfile)
	dirty[player] = false
	syncAttributes(player, profiles[player])
	emit(player)
	return true
end

function ProfileService.Save(player: Player): boolean
	return savePlayer(player)
end

function ProfileService.Start(remotes: Folder)
	metaChanged = remotes:WaitForChild("MetaStateChanged") :: RemoteEvent
	local requestMeta = remotes:WaitForChild("RequestMetaState") :: RemoteFunction
	local purchaseCosmetic = remotes:WaitForChild("PurchaseCosmetic") :: RemoteFunction
	local equipCosmetic = remotes:WaitForChild("EquipCosmetic") :: RemoteFunction
	local claimDaily = remotes:WaitForChild("ClaimDaily") :: RemoteFunction

	requestMeta.OnServerInvoke = function(player)
		local profile = ProfileService.WaitForProfile(player)
		if not profile then
			return { ok = false, reason = "profile_unavailable" }
		end
		return { ok = true, meta = snapshot(profile) }
	end
	purchaseCosmetic.OnServerInvoke = function(player, category, cosmeticId)
		if not purchaseLimiter:Allow(player) then
			return { ok = false, reason = "rate_limited", meta = ProfileService.GetSnapshot(player) }
		end
		return ProfileService.PurchaseCosmetic(player, category, cosmeticId)
	end
	equipCosmetic.OnServerInvoke = function(player, category, cosmeticId)
		if not equipLimiter:Allow(player) then
			return { ok = false, reason = "rate_limited", meta = ProfileService.GetSnapshot(player) }
		end
		return ProfileService.EquipCosmetic(player, category, cosmeticId)
	end
	claimDaily.OnServerInvoke = function(player)
		if not dailyLimiter:Allow(player) then
			return { ok = false, reason = "rate_limited", meta = ProfileService.GetSnapshot(player) }
		end
		return ProfileService.ClaimDaily(player)
	end

	Players.PlayerAdded:Connect(loadPlayer)
	for _, player in Players:GetPlayers() do
		task.defer(loadPlayer, player)
	end

	Players.PlayerRemoving:Connect(function(player)
		savePlayer(player)
		purchaseLimiter:Forget(player)
		equipLimiter:Forget(player)
		dailyLimiter:Forget(player)
		profiles[player] = nil
		dirty[player] = nil
	end)

	task.spawn(function()
		while true do
			task.wait(60)
			for player in profiles do
				if dirty[player] then
					savePlayer(player)
				end
			end
		end
	end)

	game:BindToClose(function()
		for player in profiles do
			savePlayer(player)
		end
	end)
end

return ProfileService
