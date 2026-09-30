local RetentionRules = require("../src/shared/game/RetentionRules")

assert(RetentionRules.canClaimDaily(4, 5), "next day should be claimable")
assert(not RetentionRules.canClaimDaily(5, 5), "same day must not be claimable")
local day, perfects, rewarded = RetentionRules.normalizeChallengeDay(3, 4, 9, true)
assert(day == 4 and perfects == 0 and rewarded == false, "new challenge day did not reset")
local sameDay, samePerfects, sameRewarded = RetentionRules.normalizeChallengeDay(4, 4, 2, true)
assert(sameDay == 4 and samePerfects == 2 and sameRewarded == true, "same challenge day changed")

return true
