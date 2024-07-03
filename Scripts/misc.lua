local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")
local Range = 999;

local function InitAutoAttack(enabled)
	xlua.private_accessible(CS.Panel_ThingInfo)

	if not enabled then
		Range = 0
	else
		Range = 999
	end
	
	local playerNpcs = CS.XiaWorld.World.Instance.map.Things:GetPlayerActiveNpcs();
	for i=0, playerNpcs.Count-1 do
		if playerNpcs[i].DefMode == 1 then
			print(i .. ": SET RANGE TO 999")
			playerNpcs[i].FightBody:SetWarningRange(Range)
		else
			playerNpcs[i].FightBody:SetWarningRange(0)
		end
	end

	if CS.Wnd_GameMain.Instance.PanelThingInfo and CS.Wnd_GameMain.Instance.PanelThingInfo.Panel then

		CS.Wnd_GameMain.Instance.PanelThingInfo.Panel.m_def.onClick:Add( 
			function (ctx)
				if not CS.iguana_acs_functions.Miscellaneous.Enabled.autoAttack then
					Range = 0
					return
				else
					Range = 999
				end
				if CS.Wnd_GameMain.Instance.PanelThingInfo.Panel.m_NpcInfoBnts.visible then
					if CS.Wnd_GameMain.Instance.PanelThingInfo.thing.DefMode == 0 then
						print("SET RANGE TO 999")
						CS.Wnd_GameMain.Instance.PanelThingInfo.thing.FightBody:SetWarningRange(Range)
					else
						print("SET RANGE TO 0")
						CS.Wnd_GameMain.Instance.PanelThingInfo.thing.FightBody:SetWarningRange(0)
					end
				end
			end
			)

	end
end

local function InitDarkTooltips( enabled )
	xlua.private_accessible(CS.FairyGUI.GRoot)
	if enabled then
		CS.FairyGUI.UIConfig.tooltipsWin = UIPackage.GetItemURL("IguanaUI", "DarkToolTip");
		CS.FairyGUI.GRoot.inst._defaultTooltipWin = nil
		CS.FairyGUI.GRoot.inst._defaultTooltipWin = UIPackage.CreateObject("IguanaUI", "DarkToolTip")
	else
		CS.FairyGUI.UIConfig.tooltipsWin = UIPackage.GetItemURL("InGame", "Tip");
		CS.FairyGUI.GRoot.inst._defaultTooltipWin = nil
		CS.FairyGUI.GRoot.inst._defaultTooltipWin = UIPackage.CreateObject("InGame", "Tip");
	end
end

local itemVal_Y = 0;
local function SetTradeValueVisible( ctx )
	local enabled = CS.iguana_acs_functions.Miscellaneous.Enabled.showTradeValue;

	if not ctx.sender.contentPane.m_itemvalue.visible and enabled then
		if itemVal_oldY == 0 then
			itemVal_oldY = ctx.sender.contentPane.m_friendpontvalue.y - 50
			ctx.sender.contentPane.m_itemvalue.y = itemVal_oldY
		end
		ctx.sender.contentPane.m_itemvalue.visible = true
	end
end

function AutoAttack()
	local enabled = CS.iguana_acs_functions.Miscellaneous.Enabled.autoAttack;

	InitAutoAttack(enabled)
end

function DarkTooltips()
	local enabled = CS.iguana_acs_functions.Miscellaneous.Enabled.darkTooltips;
	InitDarkTooltips(enabled)
end


function ShowSchoolTradeValue()	
	CS.Wnd_SchoolTrade.Instance.onClick:Add( SetTradeValueVisible )
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

iguana_acs_functions.submods["OnInit"]["AutoAttack"] = AutoAttack
iguana_acs_functions.submods["OnLoad"]["AutoAttack"] = AutoAttack

iguana_acs_functions.submods["OnInit"]["DarkTooltips"] = DarkTooltips
iguana_acs_functions.submods["OnLoad"]["DarkTooltips"] = DarkTooltips

iguana_acs_functions.submods["OnInit"]["ShowSchoolTradeValue"] = ShowSchoolTradeValue
iguana_acs_functions.submods["OnLoad"]["ShowSchoolTradeValue"] = ShowSchoolTradeValue