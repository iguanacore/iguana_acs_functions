using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaWorld.UI.InGame;
using XiaWorld;

namespace iguana_acs_functions
{
    internal class FixCampingHeadUI
    {
        public static bool enabled = true;

        public static Dictionary<int, int> campingNPCsStage = new Dictionary<int, int>();

        [HarmonyPatch(typeof(PlacesMgr), "Step")]
        public class CorrectProgressTimer
        {
            public static Dictionary<int, float> lastRecordedStoryTimes = new Dictionary<int, float>();
            public static void Postfix(ref PlacesMgr __instance)
            {
               if (!FixCampingHeadUI.enabled) { return; }
                foreach (PlacesMgr.MapExploreData mapExplor in __instance.MapExplors)
                {
                    if (!(ThingMgr.Instance.FindThingByID(mapExplor.NpcID) is Npc npc))
                    {
                        continue;
                    }
                    if (!mapExplor.IsStay)
                    {
                        continue;
                    }
                    ToilBase toil = npc.JobEngine.CurJob.GetCurToil();
                    campingNPCsStage[mapExplor.NpcID] = mapExplor.Stage;
                    switch (mapExplor.Stage)
                    {
                        case 0://on the way
                            toil.SetProgress(mapExplor.StageP / mapExplor.NeedTime);
                            break;
                        case 1://reached place
                            if (!lastRecordedStoryTimes.ContainsKey(mapExplor.NpcID))
                            {
                                lastRecordedStoryTimes[mapExplor.NpcID] = mapExplor.Story;
                            }
                            else if (mapExplor.Story > lastRecordedStoryTimes[mapExplor.NpcID])
                            {
                                lastRecordedStoryTimes[mapExplor.NpcID] = mapExplor.Story;
                            }
                            toil.SetProgress(1 - mapExplor.Story / lastRecordedStoryTimes[mapExplor.NpcID]);
                            break;
                        case 2://coming back
                            lastRecordedStoryTimes[mapExplor.NpcID] = 0;
                            toil.SetProgress(mapExplor.StageP / mapExplor.NeedTime);
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Wnd_GameMain), "AddNpcHead")]
        public class ChangeNpcHeadColor
        {
            public static void Postfix(ref Wnd_GameMain __instance, int i, Npc npc)
            {
                if (!FixCampingHeadUI.enabled) { return; }
                UI_Bnt_NpcHead uI_Bnt_NpcHead = __instance.UIInfo.m_lisNpcs.GetChildAt(i) as UI_Bnt_NpcHead;
                if (npc.JobEngine.CurJob == null || npc.JobEngine.CurJob.jobdef == null)
                {
                    return;
                }
                if (npc.JobEngine.CurJob.jobdef.Name != "JobLeave2Explore" || !campingNPCsStage.ContainsKey(npc.ID))
                {
                    return;
                }
                if (campingNPCsStage[npc.ID] != 1)
                {
                    return;
                }
                ToilBase curToil = npc.JobEngine.CurJob.GetCurToil();
                uI_Bnt_NpcHead.m_Tip.selectedIndex = 2;
                uI_Bnt_NpcHead.m_TipP.scaleX = 1 - curToil.Progress;
            }
        }

        [HarmonyPatch(typeof(Wnd_GameMain), "AddNpcHeadSimple")]
        public class ChangeNpcHeadSimpleColor
        {
            public static void Postfix(ref Wnd_GameMain __instance, int i, Npc npc)
            {
                if (!FixCampingHeadUI.enabled) { return; }
                UI_Bnt_NpcHeadSimple uI_Bnt_NpcHead = __instance.UIInfo.m_lisNpcs.GetChildAt(i) as UI_Bnt_NpcHeadSimple;
                if (npc.JobEngine.CurJob == null || npc.JobEngine.CurJob.jobdef == null)
                {
                    return;
                }
                if (npc.JobEngine.CurJob.jobdef.Name != "JobLeave2Explore" || !campingNPCsStage.ContainsKey(npc.ID))
                {
                    return;
                }
                if (campingNPCsStage[npc.ID] != 1)
                {
                    return;
                }
                ToilBase curToil = npc.JobEngine.CurJob.GetCurToil();
                uI_Bnt_NpcHead.m_Tip.selectedIndex = 2;
                uI_Bnt_NpcHead.m_TipP.scaleX = 1 - curToil.Progress;
            }
        }
    }
}
