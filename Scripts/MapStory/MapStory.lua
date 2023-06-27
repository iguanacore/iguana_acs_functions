local tbTable = GameMain:GetMod("MapStoryHelper");--先注册一个新的MOD模块

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