using System.Reflection.Emit;
using HarmonyLib;

namespace NoFollowerLevelLimit.Patches;

[HarmonyPatch]
public class FollowerInformationBoxPatches
{
    /*
    Old instruction
    IL_04c4: ldarg.0      // this
    IL_04c5: ldfld        class [UnityEngine.CoreModule]UnityEngine.GameObject FollowerInformationBox::_adorationContainer
    IL_04ca: callvirt     instance class [UnityEngine.CoreModule]UnityEngine.GameObject [UnityEngine.CoreModule]UnityEngine.GameObject::get_gameObject()
    IL_04cf: ldarg.0      // this
    IL_04d0: ldfld        class FollowerBrain FollowerInformationBox::followBrain
    IL_04d5: ldfld        class FollowerBrainInfo FollowerBrain::Info
    IL_04da: callvirt     instance int32 FollowerBrainInfo::get_XPLevel()
    IL_04df: ldc.i4.s     10 // 0x0a
    IL_04e1: clt
    IL_04e3: callvirt     instance void [UnityEngine.CoreModule]UnityEngine.GameObject::SetActive(bool)

    New instruction
    IL_04c4: ldarg.0      // this
    IL_04c5: ldfld        class [UnityEngine.CoreModule]UnityEngine.GameObject FollowerInformationBox::_adorationContainer
    IL_04ca: callvirt     instance class [UnityEngine.CoreModule]UnityEngine.GameObject [UnityEngine.CoreModule]UnityEngine.GameObject::get_gameObject()
    IL_04cf: ldc.i4.1
    IL_04d0: nop
    IL_04d5: nop
    IL_04da: nop
    IL_04df: nop
    IL_04e1: nop
    IL_04e3: callvirt     instance void [UnityEngine.CoreModule]UnityEngine.GameObject::SetActive(bool)

    */
    [HarmonyPatch(typeof(FollowerInformationBox), nameof(FollowerInformationBox.ConfigureImpl))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> FollowerInformationBox_ConfigureImpl(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

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

        if (!found) return codes.AsEnumerable();
        
        codes[codeIndex + 3] = new CodeInstruction(OpCodes.Ldc_I4_1);
        codes[codeIndex + 4] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 5] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 6] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 7] = new CodeInstruction(OpCodes.Nop);
        codes[codeIndex + 8] = new CodeInstruction(OpCodes.Nop);

        return codes.AsEnumerable();
    }
}