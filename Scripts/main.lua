local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function iguana_acs_functions:applyEvent(event)
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

function iguana_acs_functions:functorAddSubmodInitializer(event)
    return function(self)
        if self.configLoaded == nil then
            if self.lateCall == nil then
                self.lateCall = {}
            end
            table.insert(self.lateCall, event)
            return
        end
        iguana_acs_functions:applyEvent(event)
    end
end

function iguana_acs_functions:OnStep(dt)
    -- The configuration from MLL is loaded after the lua files are
    -- which means we need to do our own initialization once MLL is done
    if self.timer == nil then
        self.timer = 0
    end
    self.timer = self.timer + dt
    if self.timer < 1 then
        return
    end
    local configLoaded = CS.iguana_acs_functions.iguana_acs_functions.configLoaded
    if configLoaded == false then
        self.timer = 0
        return
    end
    print("iguana_acs_functions:actual init")
    self.configLoaded = true
    self.OnStep = nil
    if self.lateCall == nil then
        return
    end
    for _, event in pairs(self.lateCall) do
        print("iguana_acs_functions:late call:"..event)
        iguana_acs_functions:applyEvent(event)
    end
end

iguana_acs_functions.OnInit = iguana_acs_functions:functorAddSubmodInitializer("OnInit")
iguana_acs_functions.OnLoad = iguana_acs_functions:functorAddSubmodInitializer("OnLoad")
