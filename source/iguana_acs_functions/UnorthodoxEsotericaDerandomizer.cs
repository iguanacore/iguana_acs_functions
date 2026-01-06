using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaWorld;
using HarmonyLib;
using System.Reflection.Emit;

namespace iguana_acs_functions
{
    class UnorthodoxEsotericaDerandomizer
    {
        public static bool enabled = true;
        public static string derandomelementstring = "None";
        //static string[] values = { "None", "Metal", "Wood", "Water", "Fire", "Earth" };
        //static int index = Array.FindIndex(values, row => row.Contains(UnorthodoxEsotericaDerandomizer.derandomelementstring));
        //static int index = Array.IndexOf(values, derandomelementstring);
        public static g_emElementKind derandomelement;

        //public static g_emElementKind derandomelement = (g_emElementKind)index;

        [HarmonyPatch(typeof(RandomGongMgr), "GetNpcSkillTree_RandEso")]
        //foreach loop for capping effective value for Manuals from Unorthodox Laws
        public static class GetNpcSkillTree_RandEsoforeachpatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!enabled) { return instructions; }


                foreach (CodeInstruction codeInstruction in instructions)
                {
                    if (codeInstruction.opcode.Name == "ldfld" && codeInstruction.operand == typeof(XiaWorld.RG_RandomNodeInfo).GetField("MinValueCoeScale"))
                    {
                        codeInstruction.operand = typeof(XiaWorld.RG_RandomNodeInfo).GetField("MaxValueCoeScale");
                    }
                }
                return instructions;
            }

        }
        [HarmonyPatch(typeof(RandomGongMgr), "GetNpcSkillTree_RandEso")]
        //codematcher for attainment capping Manuals from Unorthodox Laws
        public static class GetNpcSkillTree_RandEsocodematchpatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!enabled) { return instructions; }

                var codeMatcher = new CodeMatcher(instructions);
                var codeInstruction = new CodeInstruction(OpCodes.Neg);

                codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Ldfld, typeof(XiaWorld.RG_RandomNodeInfo).GetField("MaxDaoHangAdd"))
                    , new CodeMatch(OpCodes.Ldsfld, typeof(XiaWorld.RandomGongMgr).GetField("RandomT")))
                .Advance(1)
                .Insert(codeInstruction)
                .ThrowIfNotMatch("MaxDaoHangAddNot found");
                return codeMatcher.Instructions();
            }
        }

        /*[HarmonyPatch(typeof(XiaWorld.ThingMgr), "AddEsotericaItemCustomize")]
        [HarmonyDebug]
        //fixed element for random Manuals
        public static class AddEsotericaItemCustomizePatch
        {
            public static bool Prefix(ref g_emElementKind elementKind)
            {
                elementKind = UnorthodoxEsotericaDerandomizer.derandomelement;
                return true;
            }
        }*/
        [HarmonyPatch(typeof(XiaWorld.EsotericaMgr), "RandomEsoterica")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(g_emElementKind), typeof(g_emEsotericaType), typeof(float), typeof(int), typeof(g_emGongStageLevel), typeof(string), typeof(string), typeof(bool), typeof(string) })]
        //[HarmonyDebug]
        public static class AddEsotericaItemCustomizePatch
        {
            public static bool Prefix(ref g_emElementKind elementKind)
            {
                if (derandomelementstring == "None")
                    derandomelement = g_emElementKind.None;
                else if (derandomelementstring == "Metal")
                    derandomelement = g_emElementKind.Jin;
                else if (derandomelementstring == "Wood")
                    derandomelement = g_emElementKind.Mu;
                else if (derandomelementstring == "Water")
                    derandomelement = g_emElementKind.Shui;
                else if (derandomelementstring == "Fire")
                    derandomelement = g_emElementKind.Huo;
                if (derandomelementstring == "Earth")
                    derandomelement = g_emElementKind.Tu;
                elementKind = derandomelement;
                return true;
            }
        }

    }
}
