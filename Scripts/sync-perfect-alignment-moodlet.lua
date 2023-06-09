local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function syncPerfectAlignmentMoodlet()
    -- It is not possible to simply remove the first key and add it back with a proper value
    -- as GetValueByMap iterates over the dictionary with foreach, which, in the game's (and current's ) .NET
    -- implementation, loops by order of key insertion (https://stackoverflow.com/questions/32892313/system-collections-generic-dictionary-foreach-order)
    -- so the dictionary has to be recreated entirely
    local Dictionary_Float_String = CS.System.Collections.Generic.Dictionary(CS.System.Single, CS.System.String)
    local dictReplacement = Dictionary_Float_String()
    dictReplacement:Add(1.85, "PracticeElement1")
    dictReplacement:Add(0.6, "PracticeElement2")
    dictReplacement:Add(0, "PracticeElement3")
    dictReplacement:Add(-2, "PracticeElement4")
    GameDefine.PracticeElement2MoonNameMap = dictReplacement
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
iguana_acs_functions.submods["OnInit"]["Sync perfect alignment moodlet"] = syncPerfectAlignmentMoodlet
iguana_acs_functions.submods["OnLoad"]["Sync perfect alignment moodlet"] = syncPerfectAlignmentMoodlet