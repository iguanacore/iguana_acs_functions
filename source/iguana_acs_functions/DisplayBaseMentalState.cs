using FairyGUI;
using HarmonyLib;
using UnityEngine;
using XiaWorld;
using XiaWorld.UI.InGame;
using System;

namespace iguana_acs_functions
{
    internal class DisplayBaseMentalState
    {
        public static bool enabled = true;
        //private static GImage baseMentalStateBar = null;
        public static GImage initializeUIProgressBar( UI_Panel_ThingInfo Panel )
        {
            var baseMentalStateBar = UIPackage.CreateObjectFromURL( "ui://0xrxw6g7nhdq52ow1z" ) as GImage;
            UI_NormalSlider currentMentalStateSlider = Panel.m_n39;
            baseMentalStateBar.name = "baseMentalStateBar";
            baseMentalStateBar.height = currentMentalStateSlider.m_bar.height;
            baseMentalStateBar.color = new Color( 0.5f, 1, 1, 1 );
            baseMentalStateBar.x = currentMentalStateSlider.m_bar.x;
            baseMentalStateBar.y = currentMentalStateSlider.m_bar.y;
            return baseMentalStateBar;
        }

        public static void OnLoad()
        {
            //baseMentalStateBar = null;
        }

        [HarmonyPatch( typeof( Panel_ThingInfo ), "ShowNpc" )]
        public class UpdateBaseMentalStateSlider
        {
            static void Postfix( Npc npc, UI_Panel_ThingInfo ___Panel )
            {
                try
                {
                    if ( npc == null || ___Panel == null ) { return; }
                    if ( !npc.IsPlayerThing || npc.IsVistor || npc.GongKind != g_emGongKind.Dao || !enabled )
                    {
                        if( ___Panel.m_n39.GetChild( "baseMentalStateBar" ) != null && ___Panel.m_n39.GetChild( "baseMentalStateBar" )?.visible == true)
                        {
                            ___Panel.m_n39.GetChild( "baseMentalStateBar" ).visible = false;
                        }
                        return;
                    }


                    string mentalStateDisplayName = PropertyMgr.Instance.GetDef( "MindStateBaseValue" ).DisplayName;
                    float baseMentalState = npc.GetPropertySecond( "MindStateBaseValue" );
                    int needLevel = npc.Needs.GetNeedLevel( g_emNeedType.MindState );
                    string mentalStateExtraText = string.Empty;
                    float needValue2 = npc.Needs.GetNeedValue( g_emNeedType.MindState );
                    if ( npc.Needs.GetNeed( g_emNeedType.MindState ) is MindStateNeed mindStateNeed2 )
                    {
                        float maxValue2 = mindStateNeed2.MaxValue;
                        if ( mindStateNeed2.Value >= maxValue2 && needValue2 < maxValue2 )
                        {
                            mentalStateExtraText = TFMgr.Get( "\n[color=#FF0000]此角色在其他地方心境有亏，此时心境已达极致，无法圆满。[/color]" );
                        }
                    }
                    ___Panel.m_n39.tooltips = string.Format( TFMgr.Get( "心境:{0:N0} ({1}){2}\n{3}: {4:N0}\n心境会大幅度影响修炼相关行为的效率和成功率。" ), ___Panel.m_n39.value, GameDefine.MindState2Txt[needLevel], mentalStateExtraText, mentalStateDisplayName, baseMentalState );
                    UI_NormalSlider currentMentalStateSlider = ___Panel.m_n39;
                    var baseMentalStateBar = currentMentalStateSlider.GetChild( "baseMentalStateBar" );

                    if ( baseMentalStateBar != null )
                    {
                        baseMentalStateBar.visible = true;
                    }

                    if ( currentMentalStateSlider.numChildren == 9 )
                    {
                        baseMentalStateBar = initializeUIProgressBar( ___Panel );
                        currentMentalStateSlider.AddChild( baseMentalStateBar );
                    }
                    if ( baseMentalState < 0 )
                    {
                        baseMentalStateBar.width = 0;
                    }
                    else
                    {
                        baseMentalStateBar.width = Math.Min( 1, baseMentalState / 100 ) * currentMentalStateSlider.width;
                    }
                }
                catch ( Exception e )
                {
                    KLog.Dbg( e.Source );
                    KLog.Dbg( e.StackTrace );
                }
            }
        }
    }
}
