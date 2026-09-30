local ReceiptRules = require("../src/shared/game/ReceiptRules")

local receipts = { abc = true, ["123"] = true }
assert(ReceiptRules.isProcessed(receipts, "abc"), "string receipt missing")
assert(ReceiptRules.isProcessed(receipts, 123), "numeric receipt normalization failed")
assert(not ReceiptRules.isProcessed(receipts, "def"), "unknown receipt treated as processed")

return true
