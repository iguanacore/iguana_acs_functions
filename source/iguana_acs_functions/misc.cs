using HarmonyLib;
using XiaWorld;


namespace iguana_acs_functions
{
    class Miscellaneous
    {
        public class MiscEnable
        {
            public bool autoAttack;
            public bool darkTooltips;
            public bool darkTipPopup;
            public bool showEssences;
            public bool showTradeValue;

            public MiscEnable()
            {
                autoAttack = false;
                darkTooltips = false;
                darkTipPopup = false;
                showEssences = false;
                showTradeValue = false;
            }
        }

        public static MiscEnable Enabled = new MiscEnable();


        [HarmonyPatch]
        public class Patch_DarkTooltips
        {
            static string[] colorsOLD = new string[] {
                "#9e6404",  "#B65704",  "#FF0000",  "#604343",  "#306BC7",  "#D06508",  "#ffa900",
                "#f700ff",  "#0099ff",  "#33cc00",  "#ffffff",  "#bf642f",  "#990000",  "#009900",
                "#999900",  "#880000",  "#008800",  "#888888",  "#00FF00",  "#FFFF33",  "#66FFFF",
                "#80D5FF",  "#ff0000",  "#0000FF",  "#009600",  "#FFFFFF",  "#483721",  "#FFA900",
                "#FFFF00",  "#909090",  "#006B12",  "#00c400",  "#0099cc",  "#F700FF",  "#FF8B00",
                "#FF4000",  "#000000",  "#651919" };
            static string[] colorsNEW = new string[] {
                "#EB9406",  "#E38F4F",  "#FF2B2B",  "#C28787",  "#5898ED",  "#FF9233",  "#ffa900",
                "#FA5CFF",  "#26B0FF",  "#33cc00",  "#ffffff",  "#bf642f",  "#B83E3E",  "#00C700",
                "#CFCF00",  "#C75858",  "#00B800",  "#BFBFBF",  "#00FF00",  "#FFFF33",  "#66FFFF",
                "#85E2FF",  "#FF2424",  "#00BBFF",  "#00C700",  "#FFFFFF",  "#A17B4A",  "#FFC400",
                "#FFFF00",  "#ABABAB",  "#009E1B",  "#00D600",  "#0099cc",  "#FF2BF8",  "#FF8B00",
                "#FF4F0F",  "#FFFFFF",  "#DE4E4E" };

            [HarmonyPatch( typeof( TFMgr ), "GetValue" )]
            static void Postfix( string __0, bool __1, ref string __result )
            {
                __result = NewColors( __result );
            }

            [HarmonyPostfix]
            [HarmonyPatch( typeof( Wnd_GameMain ), "GetNpcHeadTip" )]
            static void PostfixEdgeCase( FairyGUI.GObject __0, object __1, ref string __result )
            {
                __result = NewColors( __result );
            }

            static string NewColors( string str )
            {
                if ( !Enabled.darkTooltips )
                    return str;

                for ( int i = 0; i < colorsOLD.Length; i++ )
                {
                    if ( colorsOLD[i] != colorsNEW[i] )
                        str = str.Replace( colorsOLD[i], colorsNEW[i] );
                }
                return str;
            }
        }


        [HarmonyPatch( typeof( Wnd_TipPopPanel ), "UpdateItem" )]
        public class Patch_Wnd_PopPanel
        {
            //Dark Info Popup
            static UnityEngine.Color oldClr = new UnityEngine.Color( 1, 1, 1, 0 );
            static void Postfix( ref Wnd_TipPopPanel __instance, Thing ___thing )
            {
                if ( __instance?.contentPane == null )
                    return;
                
                var n0 = __instance.contentPane.GetChild( "n0" ) as FairyGUI.GImage;
                if ( oldClr.a == 0)
                    oldClr = n0.color;


                if(Enabled.darkTipPopup)
                    n0.color = new UnityEngine.Color( 0, 0, 0, 1 );
                else
                    n0.color = oldClr;

                if ( Enabled.showEssences )
                {
                    string text = "";
                    var thing = ___thing as ItemThing;
                    if ( thing != null )
                        if ( thing.def != null )
                        {
                            var itemData = thing.def.Item;
                            if ( itemData == null )
                            {
                                itemData = thing.StuffDef?.Item;
                            }
                            if ( itemData?.DevourDatas != null )
                            {
                                //Known issue for Artifacts and items with creators, there's a lacking newline in there somewhere.
                                foreach ( var devdt in itemData.DevourDatas )
                                {
                                    ThingDef def = ThingMgr.Instance.GetDef( g_emThingType.Item, devdt.Name );
                                    text += $"[color=#0EAFDE]{def.ThingName}: {devdt.Count} - {devdt.Rate * 100}%[/color]\n";
                                }
                            }
                        }

                    __instance.UIInfo.m_ThingInfoTxt.text += text;
                }
            }
        }
    }
}
