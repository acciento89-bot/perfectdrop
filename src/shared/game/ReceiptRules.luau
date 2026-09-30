--!strict

local ReceiptRules = {}

function ReceiptRules.key(purchaseId: string | number): string
	return tostring(purchaseId)
end

function ReceiptRules.isProcessed(receipts: { [string]: boolean }, purchaseId: string | number): boolean
	return receipts[ReceiptRules.key(purchaseId)] == true
end

return ReceiptRules
