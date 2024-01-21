using System.Reflection.Emit;
using HarmonyLib;
using Lamb.UI.Rituals;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class RitualInfoCardPatches
{
    [HarmonyPatch(typeof(RitualInfoCard), nameof(RitualInfoCard.Configure))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RitualItem_Configure(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FollowerBrain), "Info")),
                new CodeMatch(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(FollowerBrainInfo), "XPLevel")),
                new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)0x0a),
                new CodeMatch(OpCodes.Blt)
            )
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .InstructionEnumeration();
    }
}