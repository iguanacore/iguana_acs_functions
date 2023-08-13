local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")

function changeSecretEventShows()
    local event11 = MapStoryMgr:GetSecretDef(11) --Traces of Ancient Book
    event11.MsgShowType = "s_ancientbook"
    for i = 20,22 do
        local event = MapStoryMgr:GetSecretDef(i) -- Traces of Just Sect
        event.MsgShowType = "s_tracesjust"
    end
end

function NewMagicLeave(self, success)
    local SECRET = {53,76}
    local MsgShowMgr = CS.XiaWorld.MsgShowMgr.Instance
	if success == true then
		local LuaHelper = self.bind.LuaHelper;
		local daohang = LuaHelper:GetDaoHang();
		local rate = daohang / 4000 * (LuaHelper:GetIntelligence() + LuaHelper:GetLuck());
		if world:CheckRate(rate) then
            if MsgShowMgr:CheckEventHide("p_derivativecalc1") == false then
			    world:ShowStoryBox(XT("大衍神算施展成功，获得一条秘闻。"), XT("大衍神算"));
            end
            local result = world:RandomInt(SECRET[1],SECRET[2])
            if MsgShowMgr:CheckEventHide("p_derivativecalc2") == false then
                if result <= 57 then
                    MessageMgr:AddMessage(502)
                elseif result <= 58 then
                    MessageMgr:AddMessage(503)
                elseif result <= 63 then
                    MessageMgr:AddMessage(504)
                elseif result <= 66 then
                    MessageMgr:AddMessage(505)
                elseif result <= 69 then
                    MessageMgr:AddMessage(506)
                else
                    MessageMgr:AddMessage(507)
                end
            end
			GameEventMgr:TriggerEvent(result)	
		else
            if MsgShowMgr:CheckEventHide("p_derivativecalc1") == false then
			    world:ShowStoryBox(XT("大衍神算施展失败。"), XT("大衍神算"));
            end
            if MsgShowMgr:CheckEventHide("p_derivativecalc2") == false then
                MessageMgr:AddMessage(501)
            end
		end
	end
end

function addSectRules()
    changeSecretEventShows()
    local tbTable = GameMain:GetMod("MagicHelper");
    local tbMagic = tbTable:GetMagic("Magic_MapStory");
    tbMagic.MagicLeave = NewMagicLeave
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
iguana_acs_functions.submods["OnInit"]["Add Sect Rules"] = addSectRules
iguana_acs_functions.submods["OnLoad"]["Add Sect Rules"] = addSectRules