using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using XiaWorld;
using System.Reflection.Emit;

namespace iguana_acs_functions
{
    class GetRateStringPrecision
    {
        public static bool enabled = true;

        [HarmonyPatch(typeof(GameUlt), "GetRateString")]
        class GetRateStringPrecisionPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!enabled) { return instructions; }
                foreach (CodeInstruction codeInstruction in instructions)
                {
                    if (codeInstruction.opcode.Name == "ldc.r4" && codeInstruction.operand.ToString() == "0.1") // 0.1 is the original rate below which it doesn't display the value (insignificant. Changed to 0.01
                    {
                        float newvaluef = 0.01f;
                        codeInstruction.operand = newvaluef;
                    }
                    if (codeInstruction.opcode.Name == "ldc.i4.s" && codeInstruction.operand.ToString() == "10") //10 with ldc.i4.s is the Min Value, or the success cap. Changed to 100
                    {
                        int newvalue = 100;
                        codeInstruction.operand = newvalue;
                    }
                    if (codeInstruction.opcode.Name == "ldc.r4" && codeInstruction.operand.ToString() == "10") // 10 with ldc.r4 is the rate multiplier. 100 to make it a proper percentage, 10 for the vanilla string.
                    {
                        float newvaluef = 100f;
                        codeInstruction.operand = newvaluef;
                    }
                    if (codeInstruction.opcode.Name == "ldstr" && codeInstruction.operand.ToString() == "{0}\u6210") // The string used. Vanilla has {0}成	{0}0 % , making it round to the nearest ten.
                    {
                        string newvaluestring = "{0} %";
                        codeInstruction.operand = newvaluestring;
                    }
                    
                }
                return instructions;
            }
        }
    }
}
