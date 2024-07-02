using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using XiaWorld;
using ModLoaderLite;

namespace iguana_acs_functions
{
    class BodyPracticePlus
    {
        enum Filter { ByColor, BySelection };

        class BppData
        {
            public bool ignoreRequiredLabels;
            public int filter_mode;
            /// <summary>
            /// Preferred Labels for Each Body Part. Body part Type is used as a key to a list of preferred labels.
            /// </summary>
            public Dictionary<string, List<string>> PreferredLabels;

            public BppData( BppData bppData )
            {
                ignoreRequiredLabels = bppData.ignoreRequiredLabels;
                filter_mode = bppData.filter_mode;
                PreferredLabels = new Dictionary<string, List<string>>( bppData.PreferredLabels );
            }

            public BppData()
            {
                ignoreRequiredLabels = false;
                filter_mode = 0;
                PreferredLabels = new Dictionary<string, List<string>>();
            }

            public void ModifyPreferredLabels( string bodypart, string label, bool isAdd )
            {
                if ( !PreferredLabels.ContainsKey( bodypart ) )
                {
                    PreferredLabels[bodypart] = new List<string>();
                }

                if ( isAdd )
                {
                    if ( !PreferredLabels[bodypart].Contains( label ) )
                        PreferredLabels[bodypart].Add( label );
                }
                else
                    PreferredLabels[bodypart].Remove( label );

                if ( debug )
                {
                    KLog.Dbg( "PreferredLabels: " );
                    foreach ( var k in PreferredLabels )
                    {
                        KLog.Dbg( k.Key.ToString() );
                        foreach ( var j in k.Value )
                            KLog.Dbg( j.ToString() );
                    }
                }

            }

            public bool IsLabelPreferred( string bodypart, string label, bool isFiltering )
            {
                if ( PreferredLabels.ContainsKey( bodypart ) )
                {
                    if ( PreferredLabels[bodypart].Contains( label ) || ( isFiltering && PreferredLabels[bodypart].Count == 0 ) )
                        return true;
                }
                else if ( isFiltering )
                    return true;

                return false;
            }

            public void ClearPreferredLabels( string bodypart )
            {
                if ( bodypart == "all" )
                    PreferredLabels.Clear();
                else
                {
                    if ( PreferredLabels.ContainsKey( bodypart ) )
                        PreferredLabels[bodypart].Clear();
                }
            }
        }

        public static bool enabled = true;
        public static bool debug = false;

        static Dictionary<int, BppData> PrefsByNpc = new Dictionary<int, BppData>();

        public static void OnLoad()
        {
            var saveData = MLLMain.GetSaveOrDefault<Dictionary<int, BppData>>( "iguana_acs_functions.BodyPracticePlus" );
            if ( saveData != null )
            {
                PrefsByNpc = new Dictionary<int, BppData>( saveData );
            }
        }

        public static void OnSave()
        {
            MLLMain.AddOrOverWriteSave( "iguana_acs_functions.BodyPracticePlus", PrefsByNpc );
        }

        [HarmonyPatch( typeof( Wnd_BodyRollShow ), "Begin" )]
        public class Patch_BRS_Begin
        {
            static void Prefix( Wnd_BodyRollShow __instance, System.Collections.IEnumerator __result )
            {
                if ( enabled && UnityEngine.Time.timeScale != 15f )
                {
                    UnityEngine.Time.timeScale = 15f;
                }
            }
        }

        [HarmonyPatch( typeof( Wnd_BodyRollShow ), "OnHide" )]
        public class Patch_BRS_OnHide
        {
            static void Postfix( Wnd_BodyRollShow __instance )
            {
                UnityEngine.Time.timeScale = MainManager.Instance.gamespeed;
            }

        }

        [HarmonyPatch( typeof( PracticeMgr ), "GetRandomQuenchingLabelList" )]
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
                        foreach ( var label in __result )
                            KLog.Dbg( $"{PracticeMgr.Instance.GetBodyQuenchingLabelDef( label.Label )?.DisplayName} x {label.Lv}" );//
                    }
                    //list will contain undesired Labels ;
                    var lRemove = new List<BPLabelCacheDef.LabelDataTemp>();

                    //Remove Duplicates (and keep the one with the highest amount)
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
                                if ( label.Lv > subLabel.Lv )
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
                        // KLog.Dbg( "==========Duplicates Removed==========" );//
                        //foreach ( var label in origLabels ) KLog.Dbg( $"{label.Label} x {label.Lv}" );
                    }


                    __result = FilterLabelsList( npc, part, origLabels );

                    if ( debug )
                    {
                        KLog.Dbg( "==========Modified List==========" );//------------
                        foreach ( var label in __result )
                            KLog.Dbg( $"{PracticeMgr.Instance.GetBodyQuenchingLabelDef( label.Label )?.DisplayName} x {label.Lv}" );
                    }
                }
                catch ( Exception err )
                {
                    KLog.Dbg( "[Iguana ACS Functions - BP+] " + err.Message + "\n" + err.StackTrace );
                }
            }
        }


        public static void ModifyPreferredLabels( int npcID, string bodypart, string label, bool isAdd )
        {
            if ( !PrefsByNpc.ContainsKey( npcID ) )
            {
                PrefsByNpc[npcID] = new BppData();
            }

            if ( debug )
            {
                KLog.Dbg( "PreferredLabels: " );
                foreach ( var k in PrefsByNpc )
                {
                    KLog.Dbg( k.Key.ToString() );
                    KLog.Dbg( k.Value.PreferredLabels.Count.ToString() );
                }
            }

            PrefsByNpc[npcID].ModifyPreferredLabels( bodypart, label, isAdd );

        }

        public static void ClearPreferredLabels( int npcID, string bodypart = "all" )
        {
            PrefsByNpc[npcID].ClearPreferredLabels( bodypart );
        }

        /// <summary>
        ///  Returns a filtered list of labels based on the selected color(s) or selected label(s), and will remove labels that are
        ///  maxed out.
        ///  if none is selected, it will return origLabels without much modification. 
        /// </summary> 
        static List<BPLabelCacheDef.LabelDataTemp> FilterLabelsList( Npc npc, string bodyPart, List<BPLabelCacheDef.LabelDataTemp> origLabels )
        {
            var partdef = npc.PropertyMgr.BodyData.GetPart( bodyPart ).def;
            string cache = "BaseCache";

            //preferences set for selected npc
            int filter_mode = PrefsFilterMode(npc.ID);
            bool ignoreRequiredLabels = PrefsIgnoreLabels(npc.ID);
            
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

            //valid only if filter_mode is set to BySelection.
            //cancel "keep remolding" loop of this part, if all preferred labels have been remolded to max
            List<bool> cancelLoop = new List<bool>();

            foreach ( var label in origLabels )
            {
                //remove label based on it's rarity level or not
                bool bAddTo_lRemove = false;

                switch ( (Filter)filter_mode )
                {
                    case Filter.ByColor:
                        bAddTo_lRemove = RemoveByColorCare( npc, label );
                        break;
                    case Filter.BySelection:
                        //TODO: give player the option to choose to remember selected labels for each body part, or for each type(organ,flesh...etc)
                        bAddTo_lRemove = !IsLabelPreferred(npc.ID, partdef.Kind.ToString(), label.Label, true );

                        //when in selection mode, if you select a green label, but left the green color unchecked
                        //it will be removed by the game's own filter unless it's already quenched(remolded).
                        if ( npc.PropertyMgr.Practice.BodyPracticeData.CareColor == null )
                            npc.PropertyMgr.Practice.BodyPracticeData.CareColor = new int[5] { 1, 1, 1, 1, 1 };

                        var glist = (FairyGUI.GList)Wnd_BodyPractice.Instance.contentPane.GetChild( "n340" );
                        for ( int i = 0; i < 5; i++ )
                        {
                            npc.PropertyMgr.Practice.BodyPracticeData.CareColor[i] = 1;
                            ( (XiaWorld.UI.InGame.UI_zhennodelevelcheckbox)glist.GetChildAt( i ) ).selected = true;
                        }
                        break;
                }

                bool isQuenched = false;

                //Don't remove labels required by this body part unless we're told not to
                if ( bAddTo_lRemove && !ignoreRequiredLabels )
                    bAddTo_lRemove = !npc.PropertyMgr.Practice.BodyPracticeData.CheckLabelNeed( bodyPart, label.Label );
                
                //Filter labels that are Already Max Quenched
                if ( QuenchedLabels != null )
                {
                    foreach ( var qLabel in QuenchedLabels )
                    {
                        if ( bAddTo_lRemove ) // Quenched Labels Will be ignored
                        {
                            break;
                        }
                        if ( qLabel.n == label.Label )
                        {
                            int maxAmount = PracticeMgr.Instance.GetBodyQuenchingLabelDef( qLabel.n ).MaxLevel;

                            //check quenched label's level.
                            if ( qLabel.l >= maxAmount )
                            {
                                bAddTo_lRemove = true;
                            }
                            else
                                isQuenched = true;

                            cancelLoop.Add( bAddTo_lRemove );
                            break;
                        }
                    }

                    bool cantAddLabel = QuenchedLabels.Count >= npc.PropertyMgr.Practice.BodyPracticeData.GetPartMaxLabelCount( partdef.Name );
                    if ( !bAddTo_lRemove && !isQuenched && cantAddLabel )
                        bAddTo_lRemove = true;

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
            catch ( Exception err )
            {
                bQuenched = false;
            }
            
            //add baseLabel
            //reason: for each remold done, once a label has been selected, the first label in the list
            //          is added to the selected label and its quantity is always 1.
            bool baseLabelExists = origLabels.Exists( l => l.Label == baseLabel.Label );
            if ( origLabels.Count == 0 || ( origLabels.Count >= 1 && !bQuenched ) )
            {
                origLabels.Insert( 0, baseLabel );
            }
            else if ( !baseLabelExists || origLabels.Count >= 2 )
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
                else if( !baseLabelExists || origLabels[0].Label != baseLabel.Label || ( origLabels[0].Lv > 1 && origLabels[0].Label == baseLabel.Label ) )
                {
                    origLabels.Insert( 0, baseLabel );
                }
            }


            if( (Filter)filter_mode == Filter.BySelection )
            {
                if( !cancelLoop.Contains( false ) && cancelLoop.Count > 0 )
                {
                    var doQuenchData = npc.PropertyMgr.Practice.BodyPracticeData.GetDoQuenchingData( partdef.Name );
                    if( doQuenchData != null && doQuenchData.loop )
                    {
                        doQuenchData.loop = false;
                    }
                }
            }

            return origLabels;
        }


        static bool RemoveByColorCare( Npc npc, BPLabelCacheDef.LabelDataTemp label )
        {
            int rarity = PracticeMgr.Instance.GetBodyQuenchingLabelDef( label.Label ).Rate;
            var clrs = npc.PropertyMgr.Practice.BodyPracticeData.CareColor.ToList();

            //In case the Player left all colors unchecked or checked
            if ( clrs.Max() == clrs.Min() )
                return false;

            return !npc.PropertyMgr.Practice.BodyPracticeData.ColorCare( rarity );
        }


        /// <summary>
        /// check if bodypart exists in dictionary and if it contains label.
        /// isFiltering is used only when we are filtering the list, in case PreferredLabels is empty or there's no label,
        /// in such case we will always return true.
        /// </summary>
        public static bool IsLabelPreferred( int npcID, string bodypart, string label, bool isFiltering = false )
        {
            if ( debug )
                KLog.Dbg( $"{npcID} - {bodypart} x {label} == {isFiltering}" );
            if ( PrefsByNpc.ContainsKey( npcID ) )
            {
                return PrefsByNpc[npcID].IsLabelPreferred( bodypart, label, isFiltering );
            }
            
            return isFiltering ? true : false;
        }


        public static int PrefsFilterMode(int npcID, bool set=false, int val = 0)
        {
            if ( !PrefsByNpc.ContainsKey( npcID ) )
                PrefsByNpc[npcID] = new BppData();

            if( set )
                PrefsByNpc[npcID].filter_mode = val;

            return PrefsByNpc[npcID].filter_mode;
            
        }

        public static bool PrefsIgnoreLabels( int npcID, bool set = false, bool val = false )
        {
            if ( !PrefsByNpc.ContainsKey( npcID ) )
                PrefsByNpc[npcID] = new BppData();

            if ( set )
                PrefsByNpc[npcID].ignoreRequiredLabels = val ;

            return PrefsByNpc[npcID].ignoreRequiredLabels;
        }
    }
}
