using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoaderLite;
using XiaWorld;
using static XiaWorld.MsgShowMgr.SchoolRuleDef;

namespace iguana_acs_functions
{
    public class AddSectRules
    {
        public static bool enabled = true;

        static string currentStory = null;
        static string currentPlace = null;
        static bool addToDict = false;
        static Dictionary<string, Dictionary<string, bool>> visitedPlaceStory = new Dictionary<string, Dictionary<string, bool>>();
        static Dictionary<int, string> messageToSectEventRule = new Dictionary<int, string>()
        {
            {501, "p_derivativecalc3" },
            {502, "dc_manifestcave" },
            {503, "dc_ancientbook" },
            {504, "dc_manifestmiracle" },
            {505, "dc_tracesevil" },
            {506, "dc_tracesjust" },
            {507, "dc_manifestmandate" },
            {9, "m_9"},
            {73, "m_73"},
            {53, "m_53"}
        };

        public static void OnLoad()
        {
            Dictionary<string, Dictionary<string, bool>> tentativeLoad = MLLMain.GetSaveOrDefault<Dictionary<string, Dictionary<string, bool>>>("iguana_acs_functions.AddSectRules");
            if (tentativeLoad != null)
            {
                visitedPlaceStory = tentativeLoad;
            }
        }

        public static void OnSave()
        {
            MLLMain.AddOrOverWriteSave("iguana_acs_functions.AddSectRules", visitedPlaceStory);
        }


        [HarmonyPatch(typeof(MessageMgr), "AddMessage")]
        public class filterMessageByEventRule
        {
            static bool Prefix(int msgid)
            {
                if (!enabled) return true;
                if (!messageToSectEventRule.ContainsKey(msgid)) { return true; }
                if (MsgShowMgr.Instance.CheckEventHide(messageToSectEventRule[msgid]))
                {
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(MapStoryMgr), "TriggerStorySelection")]
        public class recordStoryPlace
        {
            static void Prefix(string name, string placename = null)
            {
                currentStory = name;
                currentPlace = placename;
            }
            static void Postfix()
            {
                if (!enabled) return;
                /* We only add at the end in case the story retriggers itself - for example when trying to setup a formation 
                 * for spirit root and failing
                 */
                if (addToDict)
                {
                    addToDict = false;
                    if (!visitedPlaceStory.ContainsKey(currentPlace))
                    {
                        Dictionary<string, bool> newPlace = new Dictionary<string, bool>();
                        visitedPlaceStory[currentPlace] = newPlace;
                    }
                    visitedPlaceStory[currentPlace][currentStory] = true;
                }
                currentStory = null;
                currentPlace = null;
            }
        }

        [HarmonyPatch(typeof(MsgShowMgr), "GetStorySelect")]
        public class IgnoreRuleFirstTime
        {
            static void Postfix(ref string __result)
            {
                if (!enabled) return;
                if (currentStory == null) { return; }
                if (currentPlace == null) { return; }
                MapStoryDef storyDef = MapStoryMgr.Instance.GetStoryDef(currentStory);
                if (String.IsNullOrEmpty(storyDef.RuleKind)) { return; }
                MsgShowMgr.SchoolRuleDef storySelectDef = MsgShowMgr.Instance.GetStorySelectDef(storyDef.RuleKind);
                if (storySelectDef == null) { return; }
                /* It's a bit awkward but we can't modify defs so one of the only places where we have some way
                 * to add an indicator to change rule behaviour is in the Default int of the selections
                 */
                bool ignoreRuleFirstTime = false;

                for (int i = 0; i < storySelectDef.Selection.Count; i++)
                {
                    SelectionData currentSelection = storySelectDef.Selection[i];

                    if (currentSelection.Default == 2)
                    {
                        ignoreRuleFirstTime = true;
                        break;
                    }
                }
                if (!ignoreRuleFirstTime) { return; }
                if (!visitedPlaceStory.ContainsKey(currentPlace))
                {
                    addToDict = true;
                    __result = null;
                    return;
                }
                if (visitedPlaceStory[currentPlace].ContainsKey(currentStory))
                {
                    return;
                }
                addToDict = true;
                __result = null;
            }
        }

        [HarmonyPatch(typeof(MapStoryMgr), "DidStorySelect")]
        public class AddDupeKeyCheck
        {
            static bool Prefix(ref string __result, ref MapStoryMgr __instance, string name, Npc npc, int index, string schoolname, string placename, out bool jump, bool test = false, SecretData secret = null, string place = null)
            {
                /* We only wish to modify AddFixedStoryTempDic so that it checks if the key are not already present,
                 * for adventures that have several popups (Like City visit -> Buy things), however AddFixedStoryTempDic
                 * is only 1 line long so the compiler inlines it, so we have to replace the whole DidStorySelect with
                 * a (mostly copy-pasted) alt version
                 */
                jump = false;
                if (!enabled) return true; // out jump must be set before we return. As the vanilla function is used anyway this is not an issue
                int num = 0;
                GameDefine.SetTempDescOpen(open: true);
                try
                {
                    if (npc != null)
                    {
                        npc.LuaHelper.test = test;
                        npc.LuaHelper.secret = secret;
                        npc.LuaHelper.place = place;
                    }
                    GameDefine.ClearTempDesc();
                    if (!GameDefine.TempFixedDec.ContainsKey("[SCHOOL]"))
                    {
                        GameDefine.AddFixedStoryTempDic("[SCHOOL]", schoolname);
                    }
                    if (!GameDefine.TempFixedDec.ContainsKey("[PLACE]"))
                    {
                        GameDefine.AddFixedStoryTempDic("[PLACE]", placename);
                    }
                    MapStoryDef storyDef = __instance.GetStoryDef(name);
                    bool flag = false;
                    MapStoryDef.MapStorySelection mapStorySelection = null;
                    num = 0;
                    if (storyDef.Selections == null || storyDef.Selections.Count == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        mapStorySelection = storyDef.Selections[index];
                        flag = string.IsNullOrEmpty(mapStorySelection.Condition) || (bool)LuaMgr.Instance.NpcDoLuaWithResult(npc, mapStorySelection.Condition);
                        if (storyDef.TalkMode == 1)
                        {
                            GameDefine.AddTempDescLine($"\t[color=#D06508]{((!string.IsNullOrEmpty(mapStorySelection.Say)) ? mapStorySelection.Say : mapStorySelection.Display)}[/color]");
                        }
                    }
                    num = 1;
                    if (flag || test)
                    {
                        if (mapStorySelection != null && !string.IsNullOrEmpty(mapStorySelection.OKResult))
                        {
                            LuaMgr.Instance.NpcDoLua(npc, mapStorySelection.OKResult);
                        }
                        else if (!string.IsNullOrEmpty(storyDef.GlobleOKResult))
                        {
                            LuaMgr.Instance.NpcDoLua(npc, storyDef.GlobleOKResult);
                        }
                    }
                    num = 2;
                    if (!flag || test)
                    {
                        if (mapStorySelection != null && !string.IsNullOrEmpty(mapStorySelection.NOResult))
                        {
                            LuaMgr.Instance.NpcDoLua(npc, mapStorySelection.NOResult);
                        }
                        else if (!string.IsNullOrEmpty(storyDef.GlobleNOResult))
                        {
                            LuaMgr.Instance.NpcDoLua(npc, storyDef.GlobleNOResult);
                        }
                    }
                    string tempDesc = GameDefine.TempDesc;
                    GameDefine.ClearTempDesc();
                    GameDefine.ClearFixedStoryTempDic();
                    GameDefine.SetTempDescOpen(open: false);
                    if (npc != null)
                    {
                        jump = npc.LuaHelper.jump;
                        npc.LuaHelper.test = false;
                        npc.LuaHelper.secret = null;
                        npc.LuaHelper.place = null;
                        npc.LuaHelper.jump = false;
                    }
                    __result = tempDesc;
                }
                catch (Exception ex)
                {
                    string arg = TFMgr.Get("成功条件");
                    if (num == 1)
                    {
                        arg = TFMgr.Get("成功结果");
                    }
                    if (num == 2)
                    {
                        arg = TFMgr.Get("失败结果");
                    }
                    if (npc != null)
                    {
                        jump = npc.LuaHelper.jump;
                        npc.LuaHelper.test = false;
                        npc.LuaHelper.secret = null;
                        npc.LuaHelper.place = null;
                    }
                    KLog.Err(string.Format(TFMgr.Get("{0}执行 选项{1} 的 {2} 结果时报错\n") + ex.ToString(), name, index, arg));
                    GameDefine.ClearTempDesc();
                    GameDefine.ClearFixedStoryTempDic();
                    GameDefine.SetTempDescOpen(open: false);
                    __result = null;
                }
                return false;
            }
        }
    }

}
