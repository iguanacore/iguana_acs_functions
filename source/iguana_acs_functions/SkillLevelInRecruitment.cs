using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using XiaWorld;
using FairyGUI;
using XiaWorld.UI.InGame;

namespace iguana_acs_functions
{
    public static class SkillLevelInRecruitment
    {
        public static bool enabled = true;

		[HarmonyPatch(typeof(Wnd_ZhaoLanWindow), "SetSlist")]
		public static class Patch5
		{
			public static void Postfix(Wnd_ZhaoLanWindow __instance, Npc npc, GList slist)
			{
				try
				{
					List<NpcSkillData.NpcSkillUnit> npcSkillUnits = Traverse.Create(__instance).Field<List<NpcSkillData.NpcSkillUnit>>("npcSkillUnits").Value;
					slist.RemoveChildrenToPool();
					for (int i = 0; i < Patch5.skills.Length; i++)
					{
						g_emNpcSkillType type = Patch5.skills[i];
						NpcSkillData.NpcSkillUnit skill = npc.PropertyMgr.SkillData.GetSkill(type);
						npcSkillUnits.Add(skill);
					}
					npcSkillUnits.Sort((NpcSkillData.NpcSkillUnit a, NpcSkillData.NpcSkillUnit b) => -npc.PropertyMgr.SkillData.GetSkillLevel(a.Type).CompareTo(npc.PropertyMgr.SkillData.GetSkillLevel(b.Type)));
					foreach (NpcSkillData.NpcSkillUnit npcSkillUnit in npcSkillUnits)
					{
						UI_SkillProgressBarMini ui_SkillProgressBarMini = slist.AddItemFromPool() as UI_SkillProgressBarMini;
						ui_SkillProgressBarMini.m_text.text = GameDefine.GetSkillName(npcSkillUnit.Type, false, g_emGongKind.Dao);
						ui_SkillProgressBarMini.m_icon.url = GameDefine.GetSkillIcon(npcSkillUnit.Type, false, g_emGongKind.Dao);
						ui_SkillProgressBarMini.m_type.selectedIndex = 1;
						ui_SkillProgressBarMini.m_Love.selectedIndex = npcSkillUnit.Love;
						bool ban = npcSkillUnit.Ban;
						if (ban)
						{
							ui_SkillProgressBarMini.m_Ban.selectedIndex = 1;
							ui_SkillProgressBarMini.m_levelb.text = "0";
						}
						else
						{
							ui_SkillProgressBarMini.m_levelb.text = npc.PropertyMgr.SkillData.GetSkillLevel(npcSkillUnit.Type).ToString("f0");
						}
					}
					npcSkillUnits.Clear();
				}
				catch (Exception e)
				{
					KLog.Dbg("[SkillLevelInRecruitment] error:" + e.ToString(), new object[0]);
				}
			}

			private static g_emNpcSkillType[] skills = new g_emNpcSkillType[]
			{
			g_emNpcSkillType.Mining,
			g_emNpcSkillType.Farming,
			g_emNpcSkillType.Building,
			g_emNpcSkillType.Manual,
			g_emNpcSkillType.SocialContact,
			g_emNpcSkillType.Art,
			g_emNpcSkillType.Cooking,
			g_emNpcSkillType.Medicine,
			g_emNpcSkillType.Fight,
			g_emNpcSkillType.Qi,
			g_emNpcSkillType.DouFa,
			g_emNpcSkillType.DanQi
			};
		}

		[HarmonyPatch(typeof(Wnd_ZhaoLanWindow), "UpdateList")]
		public static class Patch6
		{
			private static void Postfix(Wnd_ZhaoLanWindow __instance)
			{
				try
				{
					UI_LingGuanDianWin UIInfo = Traverse.Create(__instance).Field("UIInfo").GetValue<UI_LingGuanDianWin>();
					List<Npc> _shownpclist = Traverse.Create(__instance).Field("_shownpclist").GetValue<List<Npc>>();
					int ft;
					int.TryParse(UIInfo.m_sortorder.value, out ft);
					bool flag = ft != 0 && ft >= 6;
					if (flag)
					{
						g_emNpcSkillType t = (g_emNpcSkillType)(ft - 5);
						_shownpclist.Sort(delegate (Npc a, Npc b)
						{
							int value = a.PropertyMgr.SkillData.GetSkillLevel(t);
							return b.PropertyMgr.SkillData.GetSkillLevel(t).CompareTo(value);
						});
						int _selectcount = Traverse.Create(__instance).Field("_selectcount").GetValue<int>();
						UIInfo.m_n355.m_n355.numItems = _shownpclist.Count;
						UIInfo.m_n372.text = string.Format(TFMgr.Get("剩余招揽人数:{0}人"), _selectcount);
					}
				}
				catch (Exception e)
				{
					KLog.Dbg("[SkillLevelInRecruitment] error:" + e.ToString(), new object[0]);
				}
			}
		}

		[HarmonyPatch(typeof(Wnd_ZhaoLanWindow), "OnInit")]
		public static class Patch7
		{
			public static void Postfix(Wnd_ZhaoLanWindow __instance)
			{
				try
				{
					UI_LingGuanDianWin UIInfo = Traverse.Create(__instance).Field("UIInfo").GetValue<UI_LingGuanDianWin>();
					UIInfo.m_sortorder.values = new string[]
					{
					"0",
					"1",
					"2",
					"3",
					"4",
					"5",
					"6",
					"7",
					"8",
					"9",
					"10",
					"11",
					"12",
					"13",
					"14",
					"15"
					};
					UIInfo.m_sortorder.items = new string[]
					{
					TFMgr.Get("无"),
					TFMgr.Get("神识"),
					TFMgr.Get("根骨"),
					TFMgr.Get("魅力"),
					TFMgr.Get("悟性"),
					TFMgr.Get("机缘"),
					"Fight",
					"Qi",
					"SocialContact",
					"Medicine",
					"Cooking",
					"Building",
					"Farming",
					"Mining",
					"Art",
					"Manual"
					};
				}
				catch (Exception e)
				{
					KLog.Dbg("[SkillLevelInRecruitment] error:" + e.ToString(), new object[0]);
				}
			}
		}
	}
}
