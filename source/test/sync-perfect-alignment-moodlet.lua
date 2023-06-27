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

function GetValueByMap(tbMap, Key)
    -- we do like other lua scripts and implement our own getvaluebymap
    -- as generics don't have full support for xlua
	local Value
	for k, v in pairs(tbMap) do
		if Key >= k then
			return v
		end
		Value = v
	end
	return Value
end

function test()
    local g = GameMain:GetMod("TestMod")
    g.assert("1.87 returns the max value",GetValueByMap(GameDefine.PracticeElement2MoonNameMap, 1.87), "PracticeElement1")
    g.assert("1.84 returns the 2nd value",GetValueByMap(GameDefine.PracticeElement2MoonNameMap, 1.84), "PracticeElement2")
    g.assert("0 returns the 3rd value",GetValueByMap(GameDefine.PracticeElement2MoonNameMap, 0), "PracticeElement3")
    g.assert("-1 returns the last value",GetValueByMap(GameDefine.PracticeElement2MoonNameMap, -1), "PracticeElement4")
end
test()