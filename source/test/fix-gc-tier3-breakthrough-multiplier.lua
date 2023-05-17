--preliminary code to run
local g = GameMain:GetMod("TestMod")

function funcAsserter(descriptor, callback)
    return function (title, firstValue, secondValue, verbose)
        if verbose == nil then
            verbose = false
        end
        local result = callback(firstValue,secondValue)
        local text = tostring(result).." "..title
        if verbose then
            text = text.." | "..tostring(firstValue).." "..descriptor.." "..tostring(secondValue)
        end
        print(text)
        return result
    end
end
g.assert = funcAsserter("is equal to", function (a, b) return a == b end)
g.assertLessThan = funcAsserter("is less than", function (a, b) return a < b end)

function test_tier3BreakthroughMultiplier()
    local g = GameMain:GetMod("TestMod")
    g.assert("breakthrough chance at GC for T3 is unchanged", GameDefine.GlodDanEffectBroken[3][0], 1.5)
    g.assertLessThan("breakthrough chance at PS for T3 is ~90%", math.abs(GameDefine.GlodDanEffectBroken[3][1] - 0.9), 0.0001)
end
test_tier3BreakthroughMultiplier()