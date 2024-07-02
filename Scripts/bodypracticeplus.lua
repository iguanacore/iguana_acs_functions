local iguana_acs_functions = GameMain:GetMod("iguana_acs_functions")
local Labels = {} --ID, DisplayName, Bodypart, Desc, Essences, Lv
local supProps = {  AtkPower          = "Attack Power"   ,
                    DefPower          = "Endurance"   ,
                    AtkRate           = "Attack Rate"   ,
                    DefRate           = "Defence Rate"   ,
                    ArmorPenetration  = "Qi Penetration"   ,
                    CatchFabao        = "Artifact Suppression"   ,
                    FightCost         = "Qi Cost"   ,
                    RecoverQi         = "True Qi Recovery"   ,
                    ResistanceJin     = "Metal Resistance"   ,
                    ResistanceMu      = "Wood Resistance"   ,
                    ResistanceShui    = "Water Resistance"   ,
                    ResistanceHuo     = "Fire Resistance"   ,
                    ResistanceTu      = "Earth Resistance" }

local function round(n, prec) --thanks to NumericDescriptions
	local m = 10 ^ prec
	local r = math.floor(n * m + 0.5 ) / m
	local rFloor = math.floor(r)
	if r == rFloor then
		return rFloor
	else
		return r
	end
end

-- get essence display name; TODO: Add DevourData found in ItemData to know which item contains which essence (thingitem?)
local function GetEssenceDispName(Items) 
    local essences = ""
    
    if Items == nil then return nil end
    
    for i,val in pairs(Items) do
        essences = CS.XiaWorld.ThingMgr.Instance:GetDef( CS.XiaWorld.g_emThingType.Item, val).ThingName .. ", " .. essences
    end
    
    return essences.."\n"
end

local function GetLabelAdditionalInfo(label) -- get label displayName, properties and modifiers
    
    local ModifierMgr = CS.XiaWorld.ModifierMgr.Instance
    local labelDef = CS.XiaWorld.PracticeMgr.Instance:GetBodyQuenchingLabelDef(label)

    local labelDisplayName = labelDef.DisplayName 
    local supProp =  "[color=#A025B1]" --"[color=#ED53E8]"; --CC3333 prev color
    local modifier = "";
    if labelDef.SuperPartProperties ~= nil then 
        
        for i,prop in pairs(labelDef.SuperPartProperties) do
            local propName = prop.Name:ToString()
            if supProps[propName] ~= nil then
                local value = 0
                if prop.AddV ~= 0 then
                    local propValue = round( prop.AddV , 6 )
                    value = propValue > 0 and "+"..propValue or propValue
                elseif prop.BAddV ~= 0 then
                    local propValue = round( prop.BAddV , 6 )
                    value =  ( propValue > 0 and "+"..propValue or propValue ) .. " Base "
                elseif prop.AddP ~= 0 then 
                    local propValue = round( prop.AddP , 6 ) * 100 
                    value =  (propValue > 0 and "+"..propValue or propValue ) .. "% "
                elseif prop.BAddP ~= 0 then
                    local propValue = round( prop.BAddP , 6 ) * 100
                    value =  (propValue > 0 and "+"..propValue or propValue) .. "% Base "  end 
                supProp = supProp .. value .. supProps[propName] .. "\n"
            end
            
        end
    end
    
    if labelDef.Modifier ~= nil and labelDef.Modifier ~= ""  then
    
        local mods = ModifierMgr:GetDef( labelDef.Modifier )
        
        if mods ~= nil then
            for i,str in pairs(mods:GetSimpleDesc()) do
                modifier = modifier .. str .. "\n"
            end
        end
    
    end
    local globalModfClr = "[color=#249819]" --new  "[color=#2EC220]" 
    local desc = supProp .. "[/color]".. globalModfClr  .. modifier .. "[/color]"
    
    if labelDef.Desc:find(desc) == nil and labelDef.Desc:find("#249819") == nil then 
		labelDef.Desc = labelDef.Desc .. "\n" .. desc 
	end
    
    return labelDisplayName , desc, labelDef.MaxLevel
end

local function split (inputstr, sep) -- split inputstr by delimeter sep
        if inputstr == nil then
                return nil
        end
        local t={}
        
        for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
                table.insert(t, str)
        end
        
        return t
end

local function GetBPLabels(Part, baseCache) -- get labels for the selected part type (flesh, bone, organ)
    
    local selectedLabels = {}
    
    for i, LabelDef in pairs(Labels) do
        
        if LabelDef.Bodypart:find(Part) ~= nil or LabelDef.Bodypart:find("Any") ~= nil then
            
            if LabelDef.LabelBaseCache ~= nil then
                if baseCache ~= nil then
                    if baseCache == LabelDef.LabelBaseCache then  table.insert(selectedLabels, LabelDef) end
                elseif  LabelDef.Bodypart:find(Part) ~= nil then
                    table.insert(selectedLabels, LabelDef)
                end
            else
                table.insert(selectedLabels, LabelDef) 
            end
            
        end
         
        
    end
    
    return selectedLabels

end

local function FilterMode( ctx )
	local npcID = CS.Wnd_BodyPractice.Instance.npc.ID
	CS.iguana_acs_functions.BodyPracticePlus.PrefsFilterMode(npcID, true, ctx.initiator.selectedIndex)
end

local function IgnoreRequireLabels( ctx )
	 local npcID = CS.Wnd_BodyPractice.Instance.npc.ID
	CS.iguana_acs_functions.BodyPracticePlus.PrefsIgnoreLabels(npcID, true,  ctx.sender.selected );
end

local function OnClickLabel( ctx )
	 local gLabel = ctx.data
	 local bodypart = gLabel.data2
     local label = gLabel.data3
	 local npcID = CS.Wnd_BodyPractice.Instance.npc.ID

	 CS.iguana_acs_functions.BodyPracticePlus.ModifyPreferredLabels(npcID, bodypart, label, gLabel.selected)
end

local function UpdateList(ctx) -- Add Labels to the list
    
	local BodyPracticePlus = CS.iguana_acs_functions.BodyPracticePlus
	local Wnd_BodyPractice = CS.Wnd_BodyPractice.Instance
	local BPP_List = Wnd_BodyPractice.contentPane:GetChild("BPP_List")
	local npcID = Wnd_BodyPractice.npc.ID

	BPP_List.visible = BodyPracticePlus.enabled

	if not BPP_List.visible then
		return
	end


	BPP_List:GetChild("n29").selected = BodyPracticePlus.PrefsIgnoreLabels( Wnd_BodyPractice.npc.ID );
	BPP_List:GetChild("filter_mode").selectedIndex = BodyPracticePlus.PrefsFilterMode( Wnd_BodyPractice.npc.ID );

    local gList = BPP_List:GetChild("n8")
	gList.onClickItem:Add( OnClickLabel )

    local def = ctx.data.data.def 
	
	BPP_List:GetChild("n31").text = def.DisplayName
	--[[ print(gList);
	print(def.DisplayName);
	print(def.Name);
	print(def.BPQLabelBaseCache);
	print(def.Kind);
	]]
	if gList.numChildren > 0 then gList:RemoveChildrenToPool() end
    
    --:GetChild("n82"); 
	local partLabels = GetBPLabels( def.Kind:ToString(), def.BPQLabelBaseCache)
    
	if partLabels ~= nil then 
		for i,label in pairs(partLabels) do
			local Litem = gList:AddItemFromPool();
			Litem.title = label.DisplayName
			Litem.tooltips = label.Desc
			Litem.selected = BodyPracticePlus.IsLabelPreferred(npcID, def.Kind:ToString(), label.ID)
			Litem.data2 = def.Kind:ToString() --Selected Body part
			Litem.data3 = label.ID -- Label name
            
			local image = Litem:GetChild("n82")            
			if label.Lv >  16 then image.color = CS.UnityEngine.Color(  1,   1,   1)	end
			if label.Lv == 16 then image.color = CS.UnityEngine.Color( .5,  .8,  .5)	end
			if label.Lv == 9  then image.color = CS.UnityEngine.Color(.30, .70,   1)    end
			if label.Lv == 4  then image.color = CS.UnityEngine.Color(.90, .55, .90)	end
			if label.Lv == 1  then image.color = CS.UnityEngine.Color(  1, .75, .25)	end
		end
	end

end

local function SelectAllLabels( ctx )
	local npcID = CS.Wnd_BodyPractice.Instance.npc.ID
	local gList = CS.Wnd_BodyPractice.Instance.contentPane:GetChild("BPP_List"):GetChild("n8")
	gList:SelectAll()
	for i,gobj in ipairs(gList:GetChildren()) do
		CS.iguana_acs_functions.BodyPracticePlus.ModifyPreferredLabels(npcID, gobj.data2, gobj.data3, true)
	end

end

local function ClearSelectedLabels( ctx )
	local npcID = CS.Wnd_BodyPractice.Instance.npc.ID
	local gList = CS.Wnd_BodyPractice.Instance.contentPane:GetChild("BPP_List"):GetChild("n8")
	print("ClearSelectedLabels")
	CS.iguana_acs_functions.BodyPracticePlus.ClearPreferredLabels(npcID, gList:GetChildAt(0).data2)
	gList:ClearSelection()
end

function InitBPPWindow()
	local BodyPracticePlus = CS.iguana_acs_functions.BodyPracticePlus
	local Wnd_BodyPractice = CS.Wnd_BodyPractice.Instance
    local bppFrame = Wnd_BodyPractice.contentPane:AddChild(UIPackage.CreateObject("IguanaUI", "LabelsList")); 
    
    print("[Iguana ACS Funcs - BP+] Init Body Cultivation+\n") 
    
    bppFrame.x = -300;
    bppFrame.y = 20;
    bppFrame.name = "BPP_List";
	
	bppFrame:GetChild("filter_mode").onClickItem:Add(FilterMode)
	bppFrame:GetChild("filter_mode"):GetChildAt(0).tooltips = "Fitler remolded labels based on the checked colors (on the far right)"
	bppFrame:GetChild("filter_mode"):GetChildAt(1).tooltips = "Filter remolded labels based on the selected labels from the list above.\
																\n[size=9]Note: To ensure this filter works as intended, all colors will be checked.[/size]"
	bppFrame:GetChild("n29").onClick:Add(IgnoreRequireLabels)
	bppFrame:GetChild("n29").selected = BodyPracticePlus.PrefsIgnoreLabels( Wnd_BodyPractice.npc.ID );
	bppFrame:GetChild("filter_mode").selectedIndex = BodyPracticePlus.PrefsFilterMode( Wnd_BodyPractice.npc.ID );
	--bppFrame:GetChild("n8").defaultItem = "ui://0xrxw6g7dc9rovpr"
	Wnd_BodyPractice.contentPane:GetChild("n166").onClickItem:Add( UpdateList )

	local bntSelectAll = Wnd_BodyPractice.contentPane:GetChild("BPP_List"):AddChild( UIPackage.CreateObject("InGame", "CItem") )
	local bntDeselectAll = Wnd_BodyPractice.contentPane:GetChild("BPP_List"):AddChild( UIPackage.CreateObject("InGame", "CItem") )
	
	bntSelectAll.title = ""
	bntSelectAll.tooltips = "Select All Labels"
	bntSelectAll.name = "bntSelectAll"
	bntSelectAll.width = 20
	bntSelectAll.height = 20
	bntSelectAll.x = 5
	bntSelectAll.y = 15
	bntSelectAll.m_n19.color = CS.UnityEngine.Color(.3,1,.3)
	bntSelectAll.onClick:Add( SelectAllLabels )

	bntDeselectAll.title = ""
	bntDeselectAll.tooltips = "Clear Selected Labels"
	bntDeselectAll.name = "bntDeselectAll"
	bntDeselectAll.width = 20
	bntDeselectAll.height = 20
	bntDeselectAll.x = bntSelectAll.x
	bntDeselectAll.y = bntSelectAll.y + 20
	bntDeselectAll.m_n19.color = CS.UnityEngine.Color(1,.3,.3)
	bntDeselectAll.onClick:Add( ClearSelectedLabels )

    
	Wnd_BodyPractice.onPositionChanged:Remove( InitBPPWindow );
end


local function InitBPLabels() --GetBPLabelCacheDef : cache, BodyPartDef, partType, partName
    xlua.private_accessible(CS.XiaWorld.PracticeMgr)
    local dicCache = CS.XiaWorld.PracticeMgr.s_mapBPLabelCacheDefs  
    for key,val in pairs(dicCache) do
    
        if key ~= "LostCache" then
            
            for i, _label in pairs(val.Labels) do -- Append and Organize data
                
                local LabelDef = {DisplayName ,Desc, LabelBaseCache --[[ for certain organs (eyes,ears..etc) ]] , ID, Lv, Essences, Bodypart}
                if _label.Label ~= "BQLabel_LOST" and _label.Label ~= "QuenchingLabel_Lv0_Base" and 
                                                            ( _label.ItemStr ~= nil or key:find("BaseCache") or (_label.Label:match("Lv4_%S%d+_All") ~= nil and _label.Weight > 550 ) ) then 
                    LabelDef.ID = _label.Label;
                    LabelDef.Essences = GetEssenceDispName( split(_label.ItemStr,"|") )
                    LabelDef.Bodypart = _label.BodyStr;
                    LabelDef.DisplayName, LabelDef.Desc, LabelDef.Lv  = GetLabelAdditionalInfo(LabelDef.ID)
                    LabelDef.Desc = LabelDef.Desc
                    if key:find("BaseCache") ~= nil then 
                        LabelDef.LabelBaseCache = key  
                        LabelDef.Essences  = "Any, All Life Essences"
                    end 
                    if LabelDef.Essences ~= nil then LabelDef.Desc =  LabelDef.Desc .. "Found in: " ..  LabelDef.Essences  end 
                    
                    table.insert(Labels, LabelDef)
                end
            end
            
        end    
    end  

end

local function SetTradeValueVisible( ctx )
	if not ctx.sender.contentPane.m_itemvalue.visible then
		ctx.sender.contentPane.m_itemvalue.visible = true
		ctx.sender.contentPane.m_itemvalue.y = ctx.sender.contentPane.m_friendpontvalue.y - 50
	end
end

--CS.Wnd_TipPopPanel.Instance.UIInfo.m_n0.color = CS.UnityEngine.Color(0,0,0,1)
--BPP: BodyPracticePlus
function initBPP()
	--BodyPracticePlus.ClearPreferredLabels()
    CS.Wnd_BodyPractice.Instance.onPositionChanged:Add( InitBPPWindow )
    CS.Wnd_SchoolTrade.Instance.onClick:Add( SetTradeValueVisible ) 
    InitBPLabels();

	--sort table by Lv 
    table.sort(Labels, function (a,b) return a.Lv > b.Lv;   end ) 
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
iguana_acs_functions.submods["OnInit"]["Body Cultivation+"] = initBPP
iguana_acs_functions.submods["OnLoad"]["Body Cultivation+"] = initBPP