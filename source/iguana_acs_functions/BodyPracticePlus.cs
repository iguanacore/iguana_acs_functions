using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic; 
using XiaWorld;

namespace iguana_acs_functions
{
    class BodyPracticePlus
    {
        public static bool enabled = true;
        public static bool debug = false;

        [HarmonyPatch(typeof(Wnd_BodyRollShow),"Begin" )]
        public class Patch_BRS_Begin
        {
            static void Prefix( Wnd_BodyRollShow __instance, System.Collections.IEnumerator __result )
            {
                if(enabled && UnityEngine.Time.timeScale != 15f)
                    UnityEngine.Time.timeScale = 15f;
            }
        }
        [HarmonyPatch(typeof(Wnd_BodyRollShow),"OnHide")]
        public class Patch_BRS_OnHide
        {
            static void Postfix( Wnd_BodyRollShow __instance )
            {
                UnityEngine.Time.timeScale = 1f;
            }

        }

        [HarmonyPatch( typeof( PracticeMgr ),  "GetRandomQuenchingLabelList" )]
        public class Patch_RandQuenchLabel
        {
            //GetRandomQuenchingLabelList(XiaWorld.Npc npc, int seed, string part, string item, string method, int count, string mustcahce)
            static void Postfix( PracticeMgr __instance, ref List<BPLabelCacheDef.LabelDataTemp> __result,
                                                        Npc __0, int __1, string __2, string __3, string __4, int __5, string __6 )
            {
                if ( !enabled ) return;
                try
                {
                    Npc npc = __0;
                    string part = __2, essence = __3;
                    var origLabels = __result;

                    if ( debug )
                    {
                        KLog.Dbg( "==========Original List==========" );//
                        foreach ( var label in __result ) KLog.Dbg( $"{label.Label} x {label.Lv}" );//
                    }
                    //list will contain undesired Labels ;
                    var lRemove = new List<BPLabelCacheDef.LabelDataTemp>();

                    //Remove Duplicates (and keep the highest one)
                    for ( int i = 0; i < origLabels.Count; i++ )
                    {
                        var label = origLabels[i];

                        for ( int j = i + 1; j < origLabels.Count; j++ )
                        {
                            var subLabel = origLabels[j];

                            if ( lRemove.Contains( label ) )
                                break;

                            if ( subLabel.Label == label.Label )
                            {
                                if ( label.Lv > subLabel.Lv)
                                    lRemove.Add( subLabel );
                                else
                                {
                                    lRemove.Add( label );
                                }
                            }

                        }
                    }

                    origLabels.RemoveAll( e => lRemove.Contains( e ) );
                    lRemove.Clear();

                    if ( debug )
                    {
                        KLog.Dbg( "==========Duplicates Removed==========" );//
                        foreach ( var label in origLabels ) KLog.Dbg( $"{label.Label} x {label.Lv}" );
                    }

                    __result = FilterByColorCare( npc, part, origLabels );

                    if ( debug )
                    {
                        KLog.Dbg( "==========Modified List==========" );//------------
                        foreach ( var label in __result ) KLog.Dbg( $"{label.Label} x {label.Lv}" );
                    }
                }
                catch ( Exception err )
                {
                    KLog.Dbg( "[Iguana ACS Functions - BP+] " + err.Message );
                }
            }
        }

        static List<BPLabelCacheDef.LabelDataTemp> FilterByColorCare(Npc npc, string bodyPart, List<BPLabelCacheDef.LabelDataTemp> origLabels)
        {
            var partdef = npc.PropertyMgr.BodyData.GetPart( bodyPart ).def;
            string cache = "BaseCache";

            if ( !string.IsNullOrEmpty( partdef.BPQLabelBaseCache ) )
                cache = partdef.BPQLabelBaseCache;

            BPLabelCacheDef.LabelDataTemp baseLabel = PracticeMgr.Instance.GetRandomBodyQuenchingLabelByCache( npc, bodyPart, cache ); ;
            baseLabel.Lv = 1;

            //get quenched labels of @bodyPart
            List<BodyPractice.BodyQuenchingData.Label> QuenchedLabels = npc.PropertyMgr.Practice.BodyPracticeData.GetQuenchingData( bodyPart )?.ls;

            //list will contain undesired Labels 
            var lRemove = new List<BPLabelCacheDef.LabelDataTemp>();

            //will keep track track of labels kept in @origLabels
            //                    <@labelName, @isQuenched>
            var lKept = new Dictionary<string, bool>();

            foreach ( var label in origLabels )
            {
                bool bAddTo_lRemove = RemoveByColorCare(npc,label);
                bool isQuenched = false;

                //Don't remove labels required by this body part
                if ( bAddTo_lRemove )
                    bAddTo_lRemove = !npc.PropertyMgr.Practice.BodyPracticeData.CheckLabelNeed( bodyPart, label.Label );
                
                //Filter labels that are Already Max Quenched
                if ( QuenchedLabels != null )
                {
                    foreach ( var qLabel in QuenchedLabels )
                    { 
                        if ( bAddTo_lRemove ) break; // Quenched Labels Will be ignored, unless their rarity isn't filtered

                        if ( qLabel.n == label.Label )
                        {
                            int maxAmount = PracticeMgr.Instance.GetBodyQuenchingLabelDef( qLabel.n ).MaxLevel;
                            bAddTo_lRemove = false;

                            //check quenched label's level.
                            if ( qLabel.l >= maxAmount )
                            {
                                bAddTo_lRemove = true;
                            }
                            else
                                isQuenched = true;

                            break;
                        }
                    }
                }

                if ( bAddTo_lRemove ) lRemove.Add( label );
                else
                    lKept.Add( label.Label, isQuenched );

            }



            origLabels.RemoveAll( e => lRemove.Contains( e ) );


            bool bQuenched = false;
            try
            {
                bQuenched = lKept[origLabels[0].Label];
            }
            catch(Exception err)
            {
                bQuenched = false;
            }

           
            //add baseLabel  
            if ( origLabels.Count == 0 || ( origLabels.Count == 1 && !bQuenched ) )
            {
                origLabels.Insert( 0, baseLabel );
            }
            else if ( !origLabels.Exists( l => l.Label == baseLabel.Label ) || origLabels.Count >= 2 )
            {
                if ( origLabels.Count == 1 )
                {
                    origLabels[0].Lv--;
                    npc.PropertyMgr.Practice.BodyPracticeData.AddLabel( bodyPart, baseLabel.Label, baseLabel.Lv );

                    if ( !Wnd_BodyRollLabel.rslabels.Exists( l => l.n == baseLabel.Label ) )
                    {
                        Wnd_BodyRollLabel.rslabels.Add( new BodyPractice.BodyQuenchingData.Label { l = 1, n = baseLabel.Label } );
                    }
                }
                else
                {
                    origLabels.Insert( 0, baseLabel );
                }
            }



            return origLabels;
        }

        static bool RemoveByColorCare(Npc npc, BPLabelCacheDef.LabelDataTemp label)
        {
            int rarity = PracticeMgr.Instance.GetBodyQuenchingLabelDef( label.Label ).Rate;
            var clrs = npc.PropertyMgr.Practice.BodyPracticeData.CareColor.ToList();

            //In case the Player left all colors unchecked
            if ( clrs.Max() == clrs.Min() )
                return false;

            return !npc.PropertyMgr.Practice.BodyPracticeData.ColorCare( rarity );
        }
    }
}
