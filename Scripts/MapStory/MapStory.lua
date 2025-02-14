local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")
local tbTable = GameMain:GetMod("MapStoryHelper");--duplicated for the two new functions

--testing, the initial configuration, which can't be toggled anymore
function tbTable:IncreaseAllPop(me,region)
	local k = math.min(self:Rand(self:ReturnPop(2)), 999999 - region.Population);
	me:AddMsg(XT("有约{0}位流民加入了{1}。"), k, region.def.DisplayName);
	region:AddPopulation(k);
end
function tbTable:SlightlyIncreaseAllPop(me,region)
	local k = math.min(self:Rand(self:ReturnPop(1)), 999999 - region.Population);
	me:AddMsg(XT("有约{0}位流民加入了{1}。"), k, region.def.DisplayName);
	region:AddPopulation(k);
end


--[[previous version, includes the buggy version of IncreaseAllPop

function NewIncreaseAllPop(me,region)
	local k = math.min(tbTable:Rand(tbTable:ReturnPop(2)), 999999 - region.Population);
	me:AddMsg(XT("有约{0}位流民加入了{1}。"), k, region.def.DisplayName);
	region:AddPopulation(k);
end
function NewSlightlyIncreaseAllPop(me,region)
	local k = math.min(tbTable:Rand(tbTable:ReturnPop(1)), 999999 - region.Population);
	me:AddMsg(XT("有约{0}位流民加入了{1}。"), k, region.def.DisplayName);
	region:AddPopulation(k);
end

function raiseAgencyPopLimit()
	local tbTable = GameMain:GetMod("MapStoryHelper");--先注册一个新的MOD模块
	tbTable.IncreaseAllPop = NewIncreaseAllPop
	tbTable.SlightlyIncreaseAllPop = NewSlightlyIncreaseAllPop
end

if iguana_acs_functions.submods == nil then
    iguana_acs_functions.submods = {}
end
if iguana_acs_functions.submods["OnInit"] == nil then
    iguana_acs_functions.submods["OnInit"] = {}
end
if iguana_acs_functions.submods["OnLoad"] == nil then
    iguana_acs_functions.submods["OnLoad"] = {}
end
iguana_acs_functions.submods["OnInit"]["Raise Agency Pop Limit to 999,999"] = raiseAgencyPopLimit
iguana_acs_functions.submods["OnLoad"]["Raise Agency Pop Limit to 999,999"] = raiseAgencyPopLimit
]]