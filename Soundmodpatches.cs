#nullable enable
using System;
using HarmonyLib;
using UnityEngine;

namespace techtech.TechTechsSoundMod;

[HarmonyPatch(typeof(SoundManager), nameof(SoundManager.PlaySound))]
public static class Patch_PlaySound
{
    public static void Prefix(AudioClip? clip)
    {
        if (clip == null || SoundModManager.PendingInjections.Count == 0) return;
        try
        {
            var name = clip.name?.ToString() ?? "";
            if (TechTechsSoundModLocalSettings.Instance.DebugLogging.Value)
                TechTechsSoundMod.Instance.Log.LogInfo($"[SoundMod][DEBUG] PlaySound '{name}'");
            SoundModManager.TryInject(clip, name);
        }
        catch { }
    }
}

[HarmonyPatch(typeof(SoundManager), nameof(SoundManager.PlaySoundImmediate))]
public static class Patch_PlaySoundImmediate
{
    public static void Prefix(AudioClip? clip)
    {
        if (clip == null || SoundModManager.PendingInjections.Count == 0) return;
        try { SoundModManager.TryInject(clip, clip.name?.ToString() ?? ""); }
        catch { }
    }
}


// [HarmonyPatch(typeof(SoundManager), nameof(SoundManager.PlayNamedSound))]
// public static class Patch_PlayNamedSound
// {
//     public static void Prefix(AudioClip? sound)
//     {
//         if (sound == null || SoundModManager.PendingInjections.Count == 0) return;
//         try { SoundModManager.TryInject(sound, sound.name?.ToString() ?? ""); }
//         catch { }
//     }
// }