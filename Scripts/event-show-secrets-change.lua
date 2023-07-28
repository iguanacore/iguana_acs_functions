local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function changeSecretEventShows()
    local event11 = MapStoryMgr:GetSecretDef(11) --Traces of Ancient Book
    event11.MsgShowType = "s_ancientbook"
    for i = 20,22 do
        local event = MapStoryMgr:GetSecretDef(i) -- Traces of Just Sect
        event.MsgShowType = "s_tracesjust"
    end
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
iguana_acs_functions.submods["OnInit"]["add sect rules - change traces of just sect/ancient book show type"] = changeSecretEventShows
iguana_acs_functions.submods["OnLoad"]["add sect rules - change traces of just sect/ancient book show type"] = changeSecretEventShows