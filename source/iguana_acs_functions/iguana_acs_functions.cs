using FairyGUI;
using ModLoaderLite.Config;
using ModLoaderLite;
using System.Collections.Generic;
using System;

namespace iguana_acs_functions
{

    public class iguana_acs_functions
    {
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
                { "Instantly choose story option with hotkey", InstantStoryHotkey.enabled}

            };
        static Dictionary<string, List<Action>> loadSaveSubmods = new Dictionary<string, List<Action>>()
            {
                { "AddSectRules", new List<Action>(){AddSectRules.OnLoad, AddSectRules.OnSave } }
            };
        public static void OnInit()
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
                Configuration.Subscribe(new EventCallback0(HandleConfig));
            }
            HandleConfig(); // Needed or the loaded config isn't applied immediately
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if (!config.ContainsKey(kvp.Key) || config[kvp.Key])
                {
                    kvp.Value[0]();
                }
            }
            configLoaded = true;
        }
        public static Action OnLoad = OnInit;
        public static void OnSave()
        {
            MLLMain.AddOrOverWriteSave("iguana_acs_functions_config", config);
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if (!config.ContainsKey(kvp.Key) || config[kvp.Key])
                {
                    kvp.Value[1]();
                }
            }
        }

        private static void HandleConfig()
        {
            SkillLevelEverywhere.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Skill Level Everywhere");
            SortManualsByAttainment.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Sort Manuals by Attainment");
            FixCampingHeadUI.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Fix Camping Head UI");
            InfiniteScroll.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Infinite Mouse Scroll");
            AddSectRules.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Add Sect Rules");
            iguana_GodCountIncreaser.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Heavenly Reforge Limit Increase");
            iguana_OutspreadPopIncreaser.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Raise Agency Pop Limit to 999,999");
            InstantStoryHotkey.enabled = Configuration.GetCheckBox("iguana_acs_functions", "Instantly choose story option with hotkey");
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
