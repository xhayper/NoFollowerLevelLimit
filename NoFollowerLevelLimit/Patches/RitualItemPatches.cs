using System.Reflection.Emit;
using HarmonyLib;
using Lamb.UI.Rituals;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class RitualItemPatches
{
    [HarmonyPatch(typeof(RitualItem), nameof(RitualItem.Configure))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RitualItem_Configure(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FollowerBrain), "Info")),
                new CodeMatch(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(FollowerBrainInfo), "XPLevel")),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S && ((sbyte)10).Equals(i.operand)) //,
                // new CodeMatch(i => i.opcode == OpCodes.Blt_S)
            )
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .InstructionEnumeration();
    }
}