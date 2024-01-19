using System.Reflection.Emit;
using HarmonyLib;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class FollowerInformationBoxPatches
{
    [HarmonyPatch(typeof(FollowerInformationBox), nameof(FollowerInformationBox.ConfigureImpl))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FollowerInformationBox_ConfigureImpl(
        IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        {
            var found = false;
            var codeIndex = 0;

            // TODO: fill in operand
            for (var i = 0; i < codes.Count; i++)
            {
                // get adorationContainer
                if (codes[i].opcode != OpCodes.Ldarg_0) continue;
                if (codes[i + 1].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 2].opcode != OpCodes.Callvirt) continue;

                // this._followerInfo.XPLevel < 10
                if (codes[i + 3].opcode != OpCodes.Ldarg_0) continue;
                if (codes[i + 4].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 5].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 6].opcode != OpCodes.Ldc_I4_S && (int)codes[i + 7].operand == 10) continue;
                if (codes[i + 7].opcode != OpCodes.Clt) continue;

                // set active
                if (codes[i + 8].opcode != OpCodes.Callvirt) continue;

                found = true;
                codeIndex = i;

                break;
            }

            if (found)
            {
                codes[codeIndex + 3] = new CodeInstruction(OpCodes.Ldc_I4_1);
                codes[codeIndex + 4] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 5] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 6] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 7] = new CodeInstruction(OpCodes.Nop);
            }
        }

        {
            var found = false;
            var codeIndex = 0;

            // TODO: fill in operand
            for (var i = 0; i < codes.Count; i++)
            {
                // get adorationContainer
                if (codes[i].opcode != OpCodes.Ldarg_0) continue;
                if (codes[i + 1].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 2].opcode != OpCodes.Callvirt) continue;

                // this.followBrain.Info.XPLevel < 10
                if (codes[i + 3].opcode != OpCodes.Ldarg_0) continue;
                if (codes[i + 4].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 5].opcode != OpCodes.Ldfld) continue;
                if (codes[i + 6].opcode != OpCodes.Callvirt) continue;
                if (codes[i + 7].opcode != OpCodes.Ldc_I4_S && (int)codes[i + 7].operand == 10) continue;
                if (codes[i + 8].opcode != OpCodes.Clt) continue;

                // set active
                if (codes[i + 9].opcode != OpCodes.Callvirt) continue;

                found = true;
                codeIndex = i;

                break;
            }

            if (found)
            {
                codes[codeIndex + 3] = new CodeInstruction(OpCodes.Ldc_I4_1);
                codes[codeIndex + 4] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 5] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 6] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 7] = new CodeInstruction(OpCodes.Nop);
                codes[codeIndex + 8] = new CodeInstruction(OpCodes.Nop);
            }
        }

        return codes.AsEnumerable();
    }
}