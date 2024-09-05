using FairyGUI;
using ModLoaderLite.Config;
using ModLoaderLite;
using System.Collections.Generic;
using System;

namespace iguana_acs_functions
{

    public class iguana_acs_functions
    {
        static EventCallback0 eventHandleConfig = new EventCallback0(HandleConfig);
        public static bool configLoaded = false;
        public static Dictionary<string, bool> config = new Dictionary<string, bool>()
            {
                { "Skill Level Everywhere", SkillLevelEverywhere.enabled},
                { "Sort Manuals by Attainment", SortManualsByAttainment.enabled},
                { "Fix Camping Head UI", FixCampingHeadUI.enabled},
                { "Add Sect Rules", AddSectRules.enabled},
                { "Infinite Mouse Scroll", InfiniteScroll.enabled},
                { "Heavenly Reforge Limit Increase", iguana_GodCountIncreaser.enabled},
                { "Raise Agency Pop Limit to 999,999", iguana_OutspreadPopIncreaser.enabled },
                { "Sync perfect alignment moodlet", true},
                { "Fix T3 GC Breakthrough Multiplier", true},
                { "Instantly choose story option with hotkey", InstantStoryHotkey.enabled},
                { "Display Base Mental State", DisplayBaseMentalState.enabled},
                { "Display max Mood at 200", MoodTo200.enabled},
                { "Display Adventure Results Notifications", DisplayAdventureResults.enabled},
                { "Chance Estimation Precision", GetRateStringPrecision.enabled},
                { "Formation Sharing", ZhenImportExport.enabled },
                { "Body Cultivation+ Less Grindy Remolding", BodyPracticePlus.enabled },
                { "Misc: List Essences Within an Item(Popup)", Miscellaneous.Enabled.showEssences },
                { "Misc: Instantly Attack Hostile NPC", Miscellaneous.Enabled.autoAttack    },
                { "Misc: Dark Info Popup", Miscellaneous.Enabled.darkTipPopup  },
                { "Misc: Dark Tooltips(reload save to take effect)", Miscellaneous.Enabled.darkTooltips  },
                { "Misc: Display Sect Traded Items' Value", Miscellaneous.Enabled.showTradeValue}
            };
        static Dictionary<string, List<Action>> loadSaveSubmods = new Dictionary<string, List<Action>>()
            {
                { "Add Sect Rules", new List<Action>(){AddSectRules.OnLoad, AddSectRules.OnSave } },
                { "Display Base Mental State",new List<Action>(){ DisplayBaseMentalState.OnLoad, null } },
                { "Body Cultivation+ Less Grindy Remolding",new List<Action>(){ BodyPracticePlus.OnLoad, BodyPracticePlus.OnSave } }
            };
        static void OnLoadInit(string funcName)
        { 
            Dictionary<string, bool> loadConfig = MLLMain.GetSaveOrDefault<Dictionary<string, bool>>("iguana_acs_functions_config");
            if (loadConfig != null)
            {
                // We don't directly overwrite as we must handle loaded configs from previous versions of the mod
                foreach (KeyValuePair<string, bool> kvp in loadConfig)
                {
                    config[kvp.Key] = kvp.Value;
                };
            }
            if (!Configuration.ListItems.ContainsKey("iguana_acs_functions"))
            {
                foreach (KeyValuePair<string, bool> kvp in config)
                {
                    Configuration.AddCheckBox("iguana_acs_functions", kvp.Key, kvp.Key, kvp.Value);
                }
            }
            //to avoid duplicates of the same callback
            Configuration.Unsubscribe( eventHandleConfig );
            Configuration.Subscribe( eventHandleConfig );
            
            HandleConfig(); // Needed or the loaded config isn't applied immediately
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if ((!config.ContainsKey(kvp.Key) || config[kvp.Key]) && kvp.Value[0] != null)
                {
                    KLog.Dbg($"\tiguana_acs_functions: {funcName} submod {kvp.Key}");
                    kvp.Value[0]();
                }
            }
            configLoaded = true;
        }

        public static void OnInit(){ 
            OnLoadInit("OnInit");
        }
        public static void OnLoad() {
            OnLoadInit("OnLoad");
        }
        public static void OnSave()
        {
            MLLMain.AddOrOverWriteSave("iguana_acs_functions_config", config);
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if ((!config.ContainsKey(kvp.Key) || config[kvp.Key]) && kvp.Value[1] != null)
                {
                    KLog.Dbg("\tiguana_acs_functions: OnSave submod " + kvp.Key);
                    kvp.Value[1]();
                }
            }
        }

        private static void HandleConfig()
        {
            KLog.Dbg( "ssssssssssss" );
            SkillLevelEverywhere.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Skill Level Everywhere");
            SortManualsByAttainment.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Sort Manuals by Attainment");
            FixCampingHeadUI.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Fix Camping Head UI");
            InfiniteScroll.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Infinite Mouse Scroll");
            AddSectRules.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Add Sect Rules");
            iguana_GodCountIncreaser.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Heavenly Reforge Limit Increase");
            iguana_OutspreadPopIncreaser.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Raise Agency Pop Limit to 999,999");
            InstantStoryHotkey.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Instantly choose story option with hotkey");
            DisplayBaseMentalState.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Display Base Mental State");
            MoodTo200.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Display max Mood at 200");
            DisplayAdventureResults.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Display Adventure Results Notifications");
            GetRateStringPrecision.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Chance Estimation Precision");
            ZhenImportExport.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Formation Sharing");
            BodyPracticePlus.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Body Cultivation+ Less Grindy Remolding" );
            Miscellaneous.Enabled.showEssences = Configuration.GetCheckBox( "iguana_acs_functions", "Misc: List Essences Within an Item(Popup)" );
            Miscellaneous.Enabled.autoAttack = Configuration.GetCheckBox( "iguana_acs_functions", "Misc: Instantly Attack Hostile NPC" );
            Miscellaneous.Enabled.darkTipPopup = Configuration.GetCheckBox( "iguana_acs_functions", "Misc: Dark Info Popup" );
            Miscellaneous.Enabled.darkTooltips = Configuration.GetCheckBox( "iguana_acs_functions", "Misc: Dark Tooltips(reload save to take effect)" );
            Miscellaneous.Enabled.showTradeValue = Configuration.GetCheckBox( "iguana_acs_functions", "Misc: Display Sect Traded Items' Value" );
            Dictionary<string, bool> newConfig = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> kvp in config)
            {
                bool newValue = Configuration.GetCheckBox("iguana_acs_functions", kvp.Key);
                if (kvp.Value != newValue)
                {
                    newConfig[kvp.Key] = newValue;
                }
            }
            foreach (KeyValuePair<string, bool> kvp in newConfig)
            {
                config[kvp.Key] = kvp.Value;
            }
        }
    }
}
