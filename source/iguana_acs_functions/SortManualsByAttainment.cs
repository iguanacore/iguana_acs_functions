using System.Collections.Generic;
using XiaWorld;
using HarmonyLib;
using XiaWorld.UI.InGame;
using FairyGUI;

namespace iguana_acs_functions
{
    public class SortManualsByAttainment
    {
        public static List<UI_FiveForceFlag> createdItems = new List<UI_FiveForceFlag>();

        [HarmonyPatch(typeof(Wnd_CangJingGeWindow), "CheckEsoLise")]
        public class SortManuals
        {
            public static void Postfix(ref List<string> ___showesoshowlist, ref Npc ____npc, ref UI_CangJingGeWindow ___UIInfo)
            {
                for (int i = 0; i < createdItems.Count; i++)
                {
                    createdItems[i].Dispose();
                }
                createdItems.Clear();
                if (___showesoshowlist.Count == 0) return;
                Npc npc = ____npc;
                int getAttainment(string manualName)
                {
                    EsotericaData sysEsoterica = EsotericaMgr.Instance.GetSysEsoterica(manualName);
                    if (npc == null)
                    {
                        return sysEsoterica.Difficulty;
                    }
                    else
                    {
                        return sysEsoterica.GetRealDifficulty(npc.PropertyMgr.Practice.Gong.ElementKind);
                    }

                }
                int compareAttainment(string manualX, string manualY)
                {
                    return getAttainment(manualX) - getAttainment(manualY);
                }
                ___showesoshowlist.Sort(compareAttainment);
                ___UIInfo.m_elist.RefreshVirtualList();
            }
        }
        [HarmonyPatch(typeof(Wnd_CangJingGeWindow), "RenderEsoListItem")]
        public class UIAddElementIcon
        {
            public static void Postfix(int index, GObject obj, ref List<string> ___showesoshowlist, ref Npc ____npc, ref int ____type)
            {
                UI_BookShelfEso uI_BookShelfEso = obj as UI_BookShelfEso;
                string manualName = ___showesoshowlist[index];
                EsotericaData sysEsoterica = EsotericaMgr.Instance.GetSysEsoterica(manualName);
                if ((int)sysEsoterica.Element == 0)
                {
                    return;
                }
                UI_FiveForceFlag elementIcon = UI_FiveForceFlag.CreateInstance();
                elementIcon.width = (int)(elementIcon.width / 2 - 1);
                elementIcon.height = (int)(elementIcon.height / 2 - 1);
                elementIcon.x = elementIcon.x + 1;
                elementIcon.y = elementIcon.y + 1;
                elementIcon.visible = true;
                elementIcon.m_Fouls.selectedIndex = (int)sysEsoterica.Element;
                if (____type == 1)
                {
                    elementIcon.grayed = !____npc.PropertyMgr.Practice.CheckCanLearnEsoteric(sysEsoterica.Name, null, needexp: false);
                }
                uI_BookShelfEso.AddChild(elementIcon);
                createdItems.Add(elementIcon);
            }
        }
    }
}
