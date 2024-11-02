using System.Reflection;
using HarmonyLib;
using BepInEx;

namespace NoFollowerLevelLimit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
// [BepInProcess("Cult Of The Lamb.exe")] // To be decided
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} loaded!");
    }

    private void OnEnable()
    {
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        SaveAndLoad.OnLoadComplete += SaveAndLoad_OnLoadComplete;
        Logger.LogInfo($"{_harmony.GetPatchedMethods().Count()} harmony patches applied!");
    }

    private void OnDisable()
    {
        _harmony.UnpatchSelf();
        SaveAndLoad.OnLoadComplete -= SaveAndLoad_OnLoadComplete;
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} unloaded!");
    }

    private void SaveAndLoad_OnLoadComplete()
    {
        foreach (var follower in DataManager.instance.Followers)
        {
            follower.MaxLevelReached = false;
        }
    }
}
