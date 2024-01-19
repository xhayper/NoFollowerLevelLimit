using HarmonyLib;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class FollowerBrainInfoPatches
{
    [HarmonyPatch(typeof(FollowerBrainInfo), nameof(FollowerBrainInfo.MaxLevelReached), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool FollowerBrainInfo_MaxLevelReached_getter(ref bool __result)
    {
        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(FollowerBrainInfo), nameof(FollowerBrainInfo.MaxLevelReached), MethodType.Setter)]
    [HarmonyPrefix]
    private static bool FollowerBrainInfo_MaxLevelReached_setter(FollowerBrainInfo __instance)
    {
        __instance._info.MaxLevelReached = false;
        return false;
    }
}