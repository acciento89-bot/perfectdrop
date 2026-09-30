--!strict

local MarketplaceService = game:GetService("MarketplaceService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local MonetizationConfig = require(ReplicatedStorage.PerfectDrop.config.MonetizationConfig)
local ReceiptRules = require(ReplicatedStorage.PerfectDrop.game.ReceiptRules)
local ProfileService = require(script.Parent.ProfileService)

local PurchaseService = {}

local productsById: { [number]: string } = {}
for productName, definition in MonetizationConfig.Products do
	if definition.ProductId > 0 then
		productsById[definition.ProductId] = productName
	end
end

function PurchaseService.Start()
	MarketplaceService.ProcessReceipt = function(receiptInfo)
		local player = Players:GetPlayerByUserId(receiptInfo.PlayerId)
		if not player then
			return Enum.ProductPurchaseDecision.NotProcessedYet
		end

		local productName = productsById[receiptInfo.ProductId]
		if not productName then
			warn(("[Perfect Drop] unknown developer product id %d"):format(receiptInfo.ProductId))
			return Enum.ProductPurchaseDecision.NotProcessedYet
		end

		local purchaseKey = ReceiptRules.key(receiptInfo.PurchaseId)
		if ProfileService.ProcessReceipt(player, purchaseKey, productName) then
			return Enum.ProductPurchaseDecision.PurchaseGranted
		end

		return Enum.ProductPurchaseDecision.NotProcessedYet
	end
end

return PurchaseService
