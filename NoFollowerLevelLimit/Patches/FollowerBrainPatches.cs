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

    /*
     Old instruction
     IL_0008: ldarg.0      // this
     IL_0009: ldfld        class FollowerBrainStats FollowerBrain::Stats
     IL_000e: callvirt     instance bool FollowerBrainStats::get_HasLevelledUp()
     IL_0013: brtrue.s     IL_0047

     IL_0015: call         class DataManager DataManager::get_Instance()
     IL_001a: ldfld        bool DataManager::ShowLoyaltyBars
     IL_001f: brfalse.s    IL_0047
     IL_0021: ldarg.0      // this
     IL_0022: ldfld        class FollowerBrainInfo FollowerBrain::Info
     IL_0027: callvirt     instance int32 FollowerBrainInfo::get_XPLevel()
     IL_002c: ldc.i4.s     10 // 0x0a
     IL_002e: bge.s        IL_0047

     New instruction
     IL_0008: ldarg.0      // this
     IL_0009: ldfld        class FollowerBrainStats FollowerBrain::Stats
     IL_000e: callvirt     instance bool FollowerBrainStats::get_HasLevelledUp()
     IL_0013: brtrue.s     IL_0047

     IL_0015: call         class DataManager DataManager::get_Instance()
     IL_001a: ldfld        bool DataManager::ShowLoyaltyBars
     IL_001f: brfalse.s    IL_0047
     IL_0021: nop
     IL_0022: nop
     IL_0027: nop
     IL_002c: nop
     IL_002e: nop
     */
    [HarmonyPatch(typeof(FollowerBrain), nameof(FollowerBrain.AddAdoration))]
    [HarmonyPatch([typeof(Follower), typeof(FollowerBrain.AdorationActions), typeof(Action)])]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FollowerBrain_AddAdoration(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var found = false;
        var codeIndex = 0;

        // TODO: Add operand check
        for (var i = 0; i < codes.Count; i++)
        {
            // this.Stats.HasLevelledUp
            if (codes[i].opcode != OpCodes.Ldarg_0) continue;
            if (codes[i + 1].opcode != OpCodes.Ldfld) continue;
            if (codes[i + 2].opcode != OpCodes.Callvirt) continue;
            if (codes[i + 3].opcode != OpCodes.Brtrue_S) continue;
            
            // !DataManager.Instance.ShowLoyaltyBars
            if (codes[i + 4].opcode != OpCodes.Call) continue;
            if (codes[i + 5].opcode != OpCodes.Ldfld) continue;
            if (codes[i + 6].opcode != OpCodes.Brfalse_S) continue;
            
            // this.Info.XPLevel >= 10
            if (codes[i + 7].opcode != OpCodes.Ldarg_0) continue;
            if (codes[i + 8].opcode != OpCodes.Ldfld) continue;
            if (codes[i + 9].opcode != OpCodes.Callvirt) continue;
            if (codes[i + 10].opcode != OpCodes.Ldc_I4_S && (int)codes[i + 3].operand == 10) continue;
            if (codes[i + 11].opcode != OpCodes.Bge_S) continue;

            found = true;
            codeIndex = i;

            break;
        }

        if (!found) return codes.AsEnumerable();

        codes[codeIndex + 7] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 8] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 9] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 10] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 11] = new CodeInstruction(OpCodes.Nop);

        return codes.AsEnumerable();
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