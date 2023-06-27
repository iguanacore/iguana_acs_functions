using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XiaWorld;
using XiaWorld.Fight;
using HarmonyLib;
using System.Reflection.Emit;

namespace iguana_acs_functions
{
    [HarmonyPatch(typeof(FabaoData), "LoadInit")]
    class iguana_GodCountIncreaserY2KLI
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction codeInstruction in instructions)
            {
                if (codeInstruction.opcode.Name == "ldc.i4.s" && codeInstruction.operand.ToString() == "36")
                {
                    int newvalue = 2048;
                    codeInstruction.opcode = OpCodes.Ldc_I4;
                    codeInstruction.operand = newvalue;
                }
            }
            return instructions;
        }

    }

    [HarmonyPatch(typeof(FabaoData), "AddGodCount")]
    class iguana_GodCountIncreaserY2KAGC
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction codeInstruction in instructions)
            {
                if (codeInstruction.opcode.Name == "ldc.i4.s" && codeInstruction.operand.ToString() == "36")
                {
                    int newvalue = 2048;
                    codeInstruction.opcode = OpCodes.Ldc_I4;
                    codeInstruction.operand = newvalue;

                }
            }
            return instructions;
        }
    }
}
