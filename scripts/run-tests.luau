local tests = {
	require("../tests/difficulty-rules.spec"),
	require("../tests/drop-rules.spec"),
	require("../tests/round-state-rules.spec"),
	require("../tests/score-rules.spec"),
}

for index, result in tests do
	assert(result == true, ("test module %d did not return true"):format(index))
end

print(("Perfect Drop: %d pure Luau test files passed"):format(#tests))
