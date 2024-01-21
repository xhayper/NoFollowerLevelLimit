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
        /*
         *
    IL_0021: ldarg.0      // this
    IL_0022: ldfld        class FollowerBrainInfo FollowerBrain::Info
    IL_0027: callvirt     instance int32 FollowerBrainInfo::get_XPLevel()
    IL_002c: ldc.i4.s     10 // 0x0a
    IL_002e: bge.s        IL_0047
         */
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(FollowerBrain), "Info")),
                new CodeMatch(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(FollowerBrainInfo), "XPLevel")),
                new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)0x0a),
                new CodeMatch(OpCodes.Bge)
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