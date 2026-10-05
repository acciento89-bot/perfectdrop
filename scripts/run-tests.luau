local tests = {
	require("../tests/menu-visibility-contract.spec"),
	require("../tests/concept-showcase-ui.spec"),
	require("../tests/concept-fidelity.spec"),
	require("../tests/visual-polish.spec"),
	require("../tests/difficulty-rules.spec"),
	require("../tests/drop-rules.spec"),
	require("../tests/profile-rules.spec"),
	require("../tests/rate-limiter.spec"),
	require("../tests/receipt-rules.spec"),
	require("../tests/retention-rules.spec"),
	require("../tests/round-state-rules.spec"),
	require("../tests/score-rules.spec"),
	require("../tests/simulation-rules.spec"),
	require("../tests/stage-rules.spec"),
}

for index, result in tests do
	assert(result == true, ("test module %d did not return true"):format(index))
end

print(("Perfect Drop: %d pure Luau test files passed"):format(#tests))
