using FairyGUI;
using ModLoaderLite.Config;
using ModLoaderLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static FairyGUI.UIConfig;

namespace iguana_acs_functions
{

    public class iguana_acs_functions
    {
        static Dictionary<string, bool> config = new Dictionary<string, bool>()
            {
                { "SkillLevelEverywhere", SkillLevelEverywhere.enabled},
                { "SortManualsByAttainment", SortManualsByAttainment.enabled},
                { "FixCampingHeadUI", FixCampingHeadUI.enabled},
                { "AddSectRules", AddSectRules.enabled},
                { "InfiniteMouseScroll", InfiniteScroll.enabled}
            };
        static Dictionary<string, List<Action>> loadSaveSubmods = new Dictionary<string, List<Action>>()
        {
            { "AddSectRules", new List<Action>(){AddSectRules.OnLoad, AddSectRules.OnSave } }
        };
        public static void OnLoad()
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
                OnInit();
            }
            foreach(KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if (!config.ContainsKey(kvp.Key) || config[kvp.Key])
                {
                    kvp.Value[0]();
                }
            }
        }
        public static void OnInit()
        {
            if (!Configuration.ListItems.ContainsKey("iguana_acs_functions"))
            {
                foreach (KeyValuePair<string, bool> kvp in config)
                {
                    Configuration.AddCheckBox("iguana_acs_functions", kvp.Key, kvp.Key, kvp.Value);
                }
                Configuration.Subscribe(new EventCallback0(HandleConfig));
            }
        }


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
            SkillLevelEverywhere.enabled = Configuration.GetCheckBox("iguana_acs_functions", "SkillLevelEverywhere");
            SortManualsByAttainment.enabled = Configuration.GetCheckBox("iguana_acs_functions", "SortManualsByAttainment");
            FixCampingHeadUI.enabled = Configuration.GetCheckBox("iguana_acs_functions", "FixCampingHeadUI");
            InfiniteScroll.enabled = Configuration.GetCheckBox("iguana_acs_functions", "InfiniteMouseScroll");
            AddSectRules.enabled = Configuration.GetCheckBox("iguana_acs_functions", "AddSectRules");
        }
    }
}
