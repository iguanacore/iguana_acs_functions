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

local function GetEssenceDispName(Items) -- get essence display name; TODO: Add DevourData found in ItemData to know which item contains which essence (thingitem?)
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
    local supProp ="[color=#A025B1]"; --CC3333 prev color
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
    
    local desc = supProp .. "[/color]"..  "[color=#249819]" .. modifier .. "[/color]"
    
    if labelDef.Desc:find(desc) == nil then labelDef.Desc = labelDef.Desc .. "\n" .. desc end
    
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
	if ctx.initiator.selectedIndex == 0 then
		CS.iguana_acs_functions.BodyPracticePlus.filter_mode = 0
	else
		CS.iguana_acs_functions.BodyPracticePlus.filter_mode = 1
	end
end

local function IgnoreRequireLabels( ctx )
	CS.iguana_acs_functions.BodyPracticePlus.ignoreRequiredLabels = ctx.sender.selected
end

local function OnClickLabel( ctx )
	 local gLabel = ctx.data
	 local bodypart = gLabel.data2
     local label = gLabel.data3

	 CS.iguana_acs_functions.BodyPracticePlus.ModifyPreferredLabels(bodypart, label, gLabel.selected)
end

local function UpdateList(ctx) -- Add Labels to the list
    
	local BPP_List = CS.Wnd_BodyPractice.Instance.contentPane:GetChild("BPP_List")

	BPP_List.visible = CS.iguana_acs_functions.BodyPracticePlus.enabled

	if not BPP_List.visible then
		return
	end

	BPP_List:GetChild("filter_mode").onClickItem:Add(FilterMode)
	BPP_List:GetChild("filter_mode"):GetChildAt(0).tooltips = "Fitler remolded labels based on the checked colors (on the far right)"
	BPP_List:GetChild("filter_mode"):GetChildAt(1).tooltips = "Filter remolded labels based on the selected labels from the list above.\
																\n[size=9]Note: To ensure this filter works as intended, all colors will be checked.[/size]"
	BPP_List:GetChild("n29").onClick:Add(IgnoreRequireLabels)

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
			Litem.selected = CS.iguana_acs_functions.BodyPracticePlus.IsLabelPreferred(def.Kind:ToString(), label.ID)
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
	local gList = CS.Wnd_BodyPractice.Instance.contentPane:GetChild("BPP_List"):GetChild("n8")
	gList:SelectAll()
	children = gList:GetChildren()
	for i=0, children.Length-1 do
		CS.iguana_acs_functions.BodyPracticePlus.ModifyPreferredLabels(children[i].data2, children[i].data3, true)
	end

end

local function ClearSelectedLabels( ctx )
	local gList = CS.Wnd_BodyPractice.Instance.contentPane:GetChild("BPP_List"):GetChild("n8")
	print("ClearSelectedLabels")
	CS.iguana_acs_functions.BodyPracticePlus.ClearPreferredLabels(gList:GetChildAt(0).data2)
	gList:ClearSelection()
end

function InitBPPWindow()
	local Wnd_BodyPractice = CS.Wnd_BodyPractice.Instance
    local bppFrame = Wnd_BodyPractice.contentPane:AddChild(UIPackage.CreateObject("BPP_List", "LabelsList")); 
    
    print("[Iguana ACS Funcs - BP+] Init Body Cultivation+\n") 
    
    bppFrame.x = -300;
    bppFrame.y = 20;
    bppFrame.name = "BPP_List";
    --bppFrame:GetChild("n8").defaultItem = "ui://0xrxw6g7dc9rovpr"
	Wnd_BodyPractice.contentPane:GetChild("n166").onClickItem:Add( UpdateList )

	local btnSelectAll = Wnd_BodyPractice.contentPane:GetChild("BPP_List"):AddChild( UIPackage.CreateObject("InGame", "CItem") )
	local btnDeselectAll = Wnd_BodyPractice.contentPane:GetChild("BPP_List"):AddChild( UIPackage.CreateObject("InGame", "CItem") )
	
	btnSelectAll.title = ""
	btnSelectAll.tooltips = "Select All Labels"
	btnSelectAll.name = "btnSelectAll"
	btnSelectAll.width = 20
	btnSelectAll.height = 20
	btnSelectAll.x = 5
	btnSelectAll.y = 15
	btnSelectAll.m_n19.color = CS.UnityEngine.Color(.3,1,.3)
	btnSelectAll.onClick:Add( SelectAllLabels )

	btnDeselectAll.title = ""
	btnDeselectAll.tooltips = "Clear Selected Labels"
	btnDeselectAll.name = "btnDeselectAll"
	btnDeselectAll.width = 20
	btnDeselectAll.height = 20
	btnDeselectAll.x = btnSelectAll.x
	btnDeselectAll.y = btnSelectAll.y + 20
	btnDeselectAll.m_n19.color = CS.UnityEngine.Color(1,.3,.3)
	btnDeselectAll.onClick:Add( ClearSelectedLabels )

    
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
--BPP: BodyPracticePlus
function initBPP()
	CS.iguana_acs_functions.BodyPracticePlus.ClearPreferredLabels()
    CS.Wnd_BodyPractice.Instance.onPositionChanged:Add( InitBPPWindow )
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
