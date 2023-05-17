local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function fixGCBreakthroughChange()
    GameDefine.GlodDanEffectBroken[3][1] = 0.9
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
iguana_acs_functions.submods["OnInit"]["fix T3 GC Breakthrough Multiplier"] = fixGCBreakthroughChange
iguana_acs_functions.submods["OnLoad"]["fix T3 GC Breakthrough Multiplier"] = fixGCBreakthroughChange