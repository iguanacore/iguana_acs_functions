using System.Linq;
using System.Collections.Generic;
using XiaWorld;
using HarmonyLib;
using XiaWorld.UI.InGame;
using UnityEngine;

namespace iguana_acs_functions
{
    public class UIChanges
    {
        [HarmonyPatch(typeof(Panel_NpcInfoPanel), "UpdateSkill")]
        private class NPCInfoPanelUpdateBars
        {
            private static void Postfix(ref UI_panel_NpcInfo ___Panel, ref Npc ___npc, ref g_emNpcSkillType[] ___skills, g_emNpcSkillType[] ___Dskills, g_emNpcSkillType[] ___DBodyskills)
            {
                g_emNpcSkillType[] array = ((!___npc.IsDisciple) ? ___skills : ((___npc.GongKind != g_emGongKind.Body) ? ___Dskills : ___DBodyskills));
                for (int i = 0; i < ___Panel.m_n98.numItems; i++)
                {
                    UI_SkillProgressBarDouble uI_SkillProgressBarDouble = ___Panel.m_n98.GetChildAt(i) as UI_SkillProgressBarDouble;
                    if (i >= array.Length) { continue; }
                    g_emNpcSkillType g_emNpcSkillType = array[i];
                    Vector2 skillEvaluate = ___npc.PropertyMgr.SkillData.GetSkillEvaluate(g_emNpcSkillType);
                    NpcSkillData.NpcSkillUnit skill = ___npc.PropertyMgr.SkillData.GetSkill(g_emNpcSkillType);
                    if (skill == null)
                    {
                        continue;
                    }
                    uI_SkillProgressBarDouble.max = 20.0;
                    uI_SkillProgressBarDouble.m_bar2.scaleX = Mathf.Clamp(skillEvaluate.x / 20f, 0f, 1f);
                }
            }
        }

        [HarmonyPatch(typeof(Panel_NpcPropertyPanel), "UpdateSkill")]
        private class NPCPropertyPanelUpdateBars
        {
            private static void Postfix(ref UI_NpcPropertyPanel ___Panel, ref Npc ___npc, ref g_emNpcSkillType[] ___skills, ref List<g_emNpcSkillType> ___WorkNotHave)
            {
                for (int i = 0; i < ___skills.Length; i++)
                {
                    g_emNpcSkillType g_emNpcSkillType = ___skills[i];
                    Vector2 skillEvaluate = ___npc.PropertyMgr.SkillData.GetSkillEvaluate(g_emNpcSkillType);
                    UI_SkillProgressBarDouble uI_SkillProgressBarDouble = ___Panel.m_n11.GetChildAt(i) as UI_SkillProgressBarDouble;
                    if (___npc.Rank == g_emNpcRank.Disciple && ___WorkNotHave.Contains(g_emNpcSkillType))
                    {
                        continue;
                    }
                    NpcSkillData.NpcSkillUnit skill = ___npc.PropertyMgr.SkillData.GetSkill(g_emNpcSkillType);
                    if (skill == null) { continue; }
                    uI_SkillProgressBarDouble.max = 20.0;
                    uI_SkillProgressBarDouble.m_bar2.scaleX = Mathf.Clamp(skillEvaluate.x / 20f, 0f, 1f);
                }
            }
        }
    }
}
