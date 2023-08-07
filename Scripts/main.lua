local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function functorAddSubmodInitializer(event)
    return function ()
        if iguana_acs_functions.submods == nil then
            iguana_acs_functions.submods = {}
        end
        if iguana_acs_functions.submods[event] == nil then
            iguana_acs_functions.submods[event] = {}
        end
        local config = CS.iguana_acs_functions.iguana_acs_functions.config
        for title, submodFunc in pairs( iguana_acs_functions.submods[event]) do
            local doesConfigExist, configValue = config:TryGetValue(title) 
            if (doesConfigExist == false) or (configValue == true) then -- nil = no configuration -> always enabled
                print("iguana_acs_functions:"..event..": calling submod func "..title)
                submodFunc()
            else
                print("iguana_acs_functions:"..event..": "..title.." disabled, ignoring")
            end
        end
    end
end

iguana_acs_functions.OnInit = functorAddSubmodInitializer("OnInit")
iguana_acs_functions.OnLoad = functorAddSubmodInitializer("OnLoad")
