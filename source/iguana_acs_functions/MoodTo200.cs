using HarmonyLib;
using XiaWorld.UI.InGame;

namespace iguana_acs_functions
{
    internal class MoodTo200
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(Wnd_NpcInfo), "OnInit")]
        public class MoodPanelMaxMoodTo200
        {
            public static void Postfix(Wnd_NpcInfo __instance)
            {
                UI_Panel_NpcMood p = __instance.UIInfo.m_n116;
                p.m_n76.max = 200; //Mood bar itself
                p.m_n76.m_Flag.max = 200; // Tendency indicator
                p.m_n76.m_n27.x = 0.15f * p.m_n76.width; // Fallen/Slacking indicator
            }
        }

        [HarmonyPatch(typeof(Wnd_GameMain), "OnInit")]
        public class GenericPanelMaxMoodTo200
        {
            public static void Postfix(Wnd_GameMain __instance)
            {
                UI_Panel_ThingInfo p = __instance.UIInfo.m_thinginfo;
                p.m_n28.max = 200;
                p.m_n28.m_Flag.max = 200;
                p.m_n28.m_n27.x = 0.15f * p.m_n28.width;
            }
        }
        
    }
}
