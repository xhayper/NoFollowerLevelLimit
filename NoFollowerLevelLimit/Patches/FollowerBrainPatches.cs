using System.Reflection.Emit;
using HarmonyLib;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class FollowerBrainPatches
{
    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.IsMaxLevel))]
    [HarmonyPrefix]
    private static bool FollowerBrain_IsMaxLevel(ref bool __result)
    {
        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.OnMaxLevelReached))]
    [HarmonyPrefix]
    private static bool FollowerBrain_OnMaxLevelReached()
    {
        return false;
    }

    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.AddAdoration))]
    [HarmonyPatch([typeof(Follower), typeof(FollowerBrain.AdorationActions), typeof(Action)])]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FollowerBrain_AddAdoration(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FollowerBrain), "Info")),
                new CodeMatch(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(FollowerBrainInfo), "XPLevel")),
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S && ((sbyte)10).Equals(i.operand)) //,
                // new CodeMatch(i => i.opcode == OpCodes.Bge_S)
            )
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .SetAndAdvance(OpCodes.Nop, null)
            .InstructionEnumeration();
    }

    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.GetWillLevelUp))]
    [HarmonyPrefix]
    private static bool FollowerBrain_GetWillLevelUp(FollowerBrain __instance, ref bool __result,
        ref FollowerBrain.AdorationActions Action)
    {
        __result = (double)__instance.Stats.Adoration + __instance.GetAddorationToAdd(Action) >=
                   __instance.Stats.MAX_ADORATION;
        return false;
    }

    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.CanLevelUp))]
    [HarmonyPrefix]
    private static bool FollowerBrain_GetWillLevelUp(FollowerBrain __instance, ref bool __result)
    {
        __result = (double)__instance.Stats.Adoration >= __instance.Stats.MAX_ADORATION;
        return false;
    }
}