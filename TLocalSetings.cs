using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using MiraAPI.LocalSettings.Attributes;
using UnityEngine;

namespace techtech.TechTechsSoundMod;

public class TechTechsSoundModLocalSettings : LocalSettingsTab
{
    public override string TabName => "TechTech's Sound Mod";
    protected override bool ShouldCreateLabels => false;

    public override LocalSettingTabAppearance TabAppearance { get; }
    public static TechTechsSoundModLocalSettings Instance { get; private set; } = null!;

    [LocalToggleSetting]
    public ConfigEntry<bool> ModToggle {get; private set;}

    [LocalToggleSetting]
    public ConfigEntry<bool> DebugLogging {get; private set;}

    [LocalToggleSetting]
    public ConfigEntry<bool> DumpClipInfoToggle {get; private set;}

    public TechTechsSoundModLocalSettings(ConfigFile config) : base(config)
    {
        Instance = this;

        TabAppearance = new LocalSettingTabAppearance
        {
            ToggleActiveColor = Color.blue,
            ToggleInactiveColor = Color.red,
        };

        ModToggle    = config.Bind("General", "Mod Enabled",   true,  "When disabled, no sounds will be replaced.");
        DebugLogging = config.Bind("Advanced Settings", "Debug Logging", false, "Log every PlaySound call to help find the correct clip names.");
        DumpClipInfoToggle = config.Bind("Advanced Settings", "Dump Clip Info", false, "Dump all clip info in a text file");

    }
}