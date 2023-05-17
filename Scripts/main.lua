local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")
local events = GameMain:GetMod("_Event");

function functorAddSubmodInitializer(event)
    return function ()
        if iguana_acs_functions.submods == nil then
            iguana_acs_functions.submods = {}
        end
        if iguana_acs_functions.submods[event] == nil then
            iguana_acs_functions.submods[event] = {}
        end
        for title, submodFunc in pairs( iguana_acs_functions.submods[event]) do
            print("iguana_acs_functions:"..event..": calling submod func "..title)
            submodFunc()
        end
    end
end

iguana_acs_functions.OnInit = functorAddSubmodInitializer("OnInit")
iguana_acs_functions.OnLoad = functorAddSubmodInitializer("OnLoad")
