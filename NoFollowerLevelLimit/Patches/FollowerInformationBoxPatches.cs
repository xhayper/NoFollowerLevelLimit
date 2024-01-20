using System.Reflection.Emit;
using HarmonyLib;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class FollowerInformationBoxPatches
{
    [HarmonyPatch(typeof(FollowerInformationBox), nameof(FollowerInformationBox.ConfigureImpl))]
    [HarmonyPostfix]
    private static void FollowerInformationBox_ConfigureImpl(FollowerInformationBox __instance)
    {
        __instance._adorationContainer.SetActive(true);
    }
}