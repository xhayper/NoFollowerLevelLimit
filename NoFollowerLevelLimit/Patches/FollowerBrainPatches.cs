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
}