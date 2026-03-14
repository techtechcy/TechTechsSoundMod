using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.PluginLoading;
using Reactor;

namespace techtech.TechTechsSoundMod;

[BepInAutoPlugin("techtech.techtechsoundsmod", "TechTechsSoundMod")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]

public partial class TechTechsSoundMod : BasePlugin, IMiraPlugin
{
    public Harmony Harmony { get; } = new(Id);
    public string OptionsTitleText => "TechTech's Sound Mod\nDynamically replace in-game sounds with your own audio files";
    public ConfigFile GetConfigFile() => Config;
    public static TechTechsSoundMod Instance { get; private set; } = null!;
    public new ManualLogSource Log => base.Log;

    public override void Load()
    {
        Instance = this;
        SoundModManager.Initialize();

        Harmony.PatchAll();

        Log.LogInfo("[SoundMod] TechTech's Sound Mod loaded successfully.");
        Log.LogInfo($"[SoundMod] Drop audio files into: {SoundModManager.SoundsFolder}");
    }
}