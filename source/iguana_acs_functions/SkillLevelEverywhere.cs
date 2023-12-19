using System.Collections.Generic;
using XiaWorld;
using HarmonyLib;
using XiaWorld.UI.InGame;
using UnityEngine;

namespace iguana_acs_functions
{
    public static class SkillLevelEverywhere
    {
        public static bool enabled = true;


        [HarmonyPatch(typeof(Panel_NpcInfoPanel), "UpdateSkill")]
        public class NPCInfoPanelUpdateBars
        {
            public static void Postfix(ref UI_panel_NpcInfo ___Panel, ref Npc ___npc, ref g_emNpcSkillType[] ___skills, g_emNpcSkillType[] ___Dskills, g_emNpcSkillType[] ___DBodyskills)
            {
                if (!enabled) { return; }
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
        public class NPCPropertyPanelUpdateBars
        {
            public static void Postfix(ref UI_NpcPropertyPanel ___Panel, ref Npc ___npc, ref g_emNpcSkillType[] ___skills, ref List<g_emNpcSkillType> ___WorkNotHave)
            {
                if (!enabled) { return; }
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


        [HarmonyPatch( typeof( Wnd_NpcWork ), "ItemRoolOver" )]
        private class NPCWnd_NpcWork
        {
            private static void Postfix( Wnd_NpcWork __instance, FairyGUI.EventContext context )
            {
                if (!enabled) { return; }
                
                try
                {
                    var uI_NpcWorkCheckbox = context.sender as UI_NpcWorkCheckbox;
                    int childIndex = ( uI_NpcWorkCheckbox.parent as FairyGUI.GList ).GetChildIndex( uI_NpcWorkCheckbox );
                    FairyGUI.GObject[] children = __instance.UIInfo.m_n15.GetChildren();

                    for ( int i = 0; i < children.Length; i++ )
                    {
                        UI_NpcWorkCheckList uI_NpcWorkCheckList = (UI_NpcWorkCheckList)children[i];

                        if ( uI_NpcWorkCheckList.m_n13.GetChildAt( childIndex ) is UI_NpcWorkCheckbox uI_NpcWorkCheckbox2 )
                        {
                            uI_NpcWorkCheckbox2.m_HighLight.selectedIndex = 1;

                            var npc = uI_NpcWorkCheckbox2.data as Npc;
                            g_emBehaviourWorkKind work = (g_emBehaviourWorkKind)uI_NpcWorkCheckbox2.data2;

                            if ( GameDefine.BehaviourLinkSkill.ContainsKey( work ) )
                            {
                                var skill = GameDefine.BehaviourLinkSkill[work];
                                uI_NpcWorkCheckbox2.m_level.text = npc.PropertyMgr.SkillData.GetSkillLevel( skill ).ToString();
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Main.Debug( e.Message );
                }
            }
        }
    }
}
