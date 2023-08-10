using HarmonyLib;
using XiaWorld.UI.InGame;

namespace iguana_acs_functions
{
    public class InstantStoryHotkey
    {
        public static bool enabled = true;
        [HarmonyPatch(typeof(Wnd_StorySelect), "SelectClicking")]
        static class FillUpSelectionInstantly
        {
            static void Postfix(int ___CurDown, Wnd_StorySelect __instance)
            {
                if (!enabled) { return; }
                if (___CurDown < 0)
                {
                    return;
                }
                UI_WndBnt_StorySelect uI_WndBnt_StorySelect = __instance.UIInfo.m_selection.GetChildAt(___CurDown) as UI_WndBnt_StorySelect;
                uI_WndBnt_StorySelect.m_n12.fillAmount = 1f;

            }
        }
    }
}
