using HarmonyLib;

namespace iguana_acs_functions
{
    class ZhenImportExport
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(Wnd_CreateZhen), "OnShown")]
        public static class ZhenImportExportPatch
        {
            static void Postfix(Wnd_CreateZhen __instance)
            {

                if (!enabled) { return; }

                __instance.UIInfo.m_n39.visible = true; //The Export/Share button
                __instance.UIInfo.m_n40.visible = true; //The Import button
                __instance.UIInfo.m_n105.visible = true; //The Import/Export parent group
               
            }
        }

    }
}
