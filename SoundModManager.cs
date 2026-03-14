#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

namespace techtech.TechTechsSoundMod;

public static class SoundMap
{
    // Key = filename stem in TechTechsSounds folder
    // Value = AudioClip.name(s) used by the game
    public static readonly Dictionary<string, string[]> Entries = new(StringComparer.OrdinalIgnoreCase)
    {
    ///////// Vanilla /////////
    
    /// General ///
    {"player_disconnect",       new[] {"playerdisconnect"}},
    
    // Impostor
    {"kill(killer)",            new[] {"impostor_kill"}},                     // the sound the killer hears when killing someone
    {"sabotage_alarm",          new[] {"Alarm_sabotage"}},
    {"kill_music",              new[] {"impostor_killMusic"}},

    {"kill(victim)_knife",      new[] {"kill_knife"}},
    {"kill(victim)_tongue",     new[] {"Kill_Alien"}},
    {"kill(victim)_shooting",   new[] {"kill_gun"}},
    {"kill(victim)_neck_twist", new[] {"kill_neck"}},


    // Meetings
    {"meeting",                 new[] {"alarm_emergencymeeting"}},            // the whooop sound
    {"emergency_button_click",  new[] {"panel_emergencyButton"}},    
    {"notification",            new[] {"notification"}},
    
    {"vote_timer",              new[] {"vote_timer"}},
    {"select_player_to_vote",   new[] {"votescreen_avote"}},
    {"lock_vote",               new[] {"votescreen_lockin"}},

    {"eject_text_typing",       new[] {"eject_text"}},
    {"eject_sheld",             new[] {"eject_skeld"}},



    // UI
    {"ui_hover",                new[] {"UI_Hover"}},
    {"button_click",            new[] {"UI_Select"}},    


        
        
    // Ambient
    {"ambient_weaponsroom",     new[] {"AMB_Weapons"}},
    {"ambient_oxygenroom",      new[] {"AMB_O2Room"}},
    {"ambient_shieldroom",      new[] {"AMB_ShieldRoom"}},
    {"ambient_reactorroom",     new[] {"AMB_ReactorRoom"}},
    {"ambient_engineroom",      new[] {"AMB_EngineRoom"}},
    {"ambient_commsroom",       new[] {"AMB_CommsRoom"}},
    {"ambient_securityroom",    new[] {"AMB_SecurityRoom"}},
    {"ambient_medbayroom",      new[] {"AMB_MedbayRoom"}},




    ///////// Town of Us: Mira /////////
    
    /// Guessing ///
    {"guess_kill",              new[] {"votescreen_playerdead"}},
    
    /// Role Specific ///
    
    // Mayor
        {"mayor_reveal",            new[] {"MayorRevealSound"}},
    


    }; /// {"filename",                new[] {"clipname"}},

    public static readonly string README = @"TechTech's Sound Mod:
    ------------------------

    To replace a sound, place a wav audio file in this folder named as shown below:


    ///////// Vanilla /////////
    
    --- General ---
    player_disconnect.wav               -> Plays when a player disconnects
     
    --- Impostor --- 
    kill(killer).wav                    -> The sound the killer hears when killing someone
    sabotage_alarm.wav                  -> The alarm you hear when a sabotage happens
    kill_music.wav

    kill(victim)_knife.wav              -> Plays with the stabbing kill animation on the victim
    kill(victim)_tongue.wav             -> Plays with the tongue kill animation on the victim
    kill(victim)_shooting.wav           -> Plays with the shoot kill animation on the victim
    kill(victim)_neck_twist.wav         -> Plays with the neck twist kill animation on the victim
    
    --- Ambient ---
    ambient_weaponsroom.wav             -> Ambient Sound of the Weapons room
    ambient_oxygenroom.wav              -> Ambient Sound of the Oxygen room
    ambient_shieldroom.wav              -> Ambient Sound of the Shield room
    ambient_reactorroom.wav             -> Ambient Sound of the Reactor room
    ambient_engineroom.wav              -> Ambient Sound of the Engine room
    ambient_commsroom.wav               -> Ambient Sound of the Communications room
    ambient_securityroom.wav            -> Ambient Sound of the Security room
    ambient_medbayroom.wav              -> Ambient Sound of the Medbay room

    --- Meetings ---
    meeting.wav                         -> The whooop meeting sound
    notification.wav                    -> The notification you hear when you get a new message in chat
    emergency_button_click.wav          -> The click of the emergency button

    vote_timer.wav                      -> Plays with the final countdown at the end of a meeting
    select_player_to_vote.wav           -> Plays when selecting a player to vote.                         
    lock_vote.wav                       -> Plays when pressing the tick button after selecting a player (locking your vote)

    eject_text_typing.wav               -> The typing of letters after a meeting
    eject_skeld.wav                     -> Eject sound on the Skeld map

    --- UI ---
    ui_hover.wav                        -> The sound you hear when hovering over a button
    button_click.wav                    -> Button click sound    




    ///////// Town Of Us: Mira /////////
    
    --- Guessing ---
    guess_kill.wav                      -> Plays to everyone when someone dies during a meeting
    
    --- Role Specific ----
    
    - Mayor
        mayor_reveal.wav                -> Plays when a politician becomes a mayor / mayor reveals role
    ";

    public static readonly string[] Extensions = { ".wav" };
}

public static class SoundModManager
{
    public static string SoundsFolder { get; private set; } = null!;
    private static ManualLogSource Log => TechTechsSoundMod.Instance.Log;
    public static readonly Dictionary<string, float[]> PendingInjections = new(StringComparer.OrdinalIgnoreCase);
    public static readonly HashSet<string> Injected = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, string> ClipInfo = new(StringComparer.OrdinalIgnoreCase);
    private static bool DumpWritten = false;

    private static readonly HashSet<string> SeenClips = new(StringComparer.OrdinalIgnoreCase);


    private static void DebugLogClip(AudioClip clip, string clipName)
    {
        if (!TechTechsSoundModLocalSettings.Instance.DebugLogging.Value)
            return;

        if (SeenClips.Contains(clipName))
            return;

        SeenClips.Add(clipName);

        Log.LogInfo(
            $"[SoundMod][DEBUG] AudioClip detected: {clipName} | " +
            $"{clip.frequency}Hz | {clip.channels}ch | {clip.samples} samples | {clip.length:F3}s"
        );
    }



    private static void WriteDump()
    {
        if (!TechTechsSoundModLocalSettings.Instance.DumpClipInfoToggle.Value)
            return;

        if (DumpWritten) return;

        try
        {
            var bepInExRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "BepInEx"));
            var path = Path.Combine(bepInExRoot, "TechTechsSoundInfo.txt");

            using var sw = new StreamWriter(path);

            sw.WriteLine("TechTech's Sound Mod - Audio Info Dump");
            sw.WriteLine("--------------------------------------");
            sw.WriteLine();

            foreach (var (stem, aliases) in SoundMap.Entries)
            {
                sw.WriteLine($"File name: {stem}.wav");

                foreach (var alias in aliases)
                {
                    sw.WriteLine($"  ClipName: {alias}");

                    if (ClipInfo.TryGetValue(alias, out var info))
                    {
                        sw.WriteLine(info.Replace("\n", "\n  "));
                    }
                    else
                    {
                        sw.WriteLine("  (not played yet)");
                    }
                }

                sw.WriteLine();
            }

            Log.LogInfo($"[SoundMod] Wrote clip info dump: {path}");
            DumpWritten = true;
        }
        catch (Exception ex)
        {
            Log.LogWarning($"[SoundMod] Failed to write clip info dump: {ex}");
        }
    }
    private static void RecordClipInfo(AudioClip clip, string clipName)
    {
        if (!TechTechsSoundModLocalSettings.Instance.DumpClipInfoToggle.Value)
            return;

        if (!SoundMap.Entries.ContainsValue(new[] { clipName }) && !PendingInjections.ContainsKey(clipName))
            return;

        if (ClipInfo.ContainsKey(clipName))
            return;

        var info =
            $"Clip: {clipName}\n" +
            $"SampleRate: {clip.frequency}\n" +
            $"Channels: {clip.channels}\n" +
            $"Samples: {clip.samples}\n" +
            $"LengthSeconds: {clip.length:F3}\n";

        ClipInfo[clipName] = info;
    }

    public static void Initialize()
    {
        var gameRoot = Path.GetDirectoryName(Application.dataPath)!;
        SoundsFolder = Path.Combine(gameRoot, "TechTechsSounds");

        if (!Directory.Exists(SoundsFolder))
        {
            Directory.CreateDirectory(SoundsFolder);
            Log.LogInfo($"[SoundMod] Created sounds folder: {SoundsFolder}");
        }
        else
        {
            Log.LogInfo($"[SoundMod] Sounds folder found: {SoundsFolder}");
        }

        WriteReadme();
        LoadAllWavs();
    }

    private static void LoadAllWavs()
    {
        PendingInjections.Clear();
        Injected.Clear();
        int loaded = 0;

        foreach (var (stem, aliases) in SoundMap.Entries)
        {
            string? file = null;
            foreach (var ext in SoundMap.Extensions)
            {
                var p = Path.Combine(SoundsFolder, stem + ext);
                if (File.Exists(p)) { file = p; break; }
            }
            if (file == null) continue;

            try
            {
                var samples = ParseWavToFloat(file);
                foreach (var alias in aliases)
                    PendingInjections[alias] = samples;
                Log.LogInfo($"[SoundMod] Loaded '{Path.GetFileName(file)}' → [{string.Join(", ", aliases)}] ({samples.Length} samples)");
                loaded++;
            }
            catch (Exception ex)
            {
                Log.LogWarning($"[SoundMod] Failed to load '{file}': {ex.Message}");
            }
        }

        Log.LogInfo($"[SoundMod] {loaded} WAV file(s) ready to inject.");
    }

    public static void TryInject(AudioClip clip, string clipName)
    {
        if (!TechTechsSoundModLocalSettings.Instance.ModToggle.Value) return;
        if (!TechTechsSoundModLocalSettings.Instance.ModToggle.Value) return;
        if (Injected.Contains(clipName)) return;
        if (!PendingInjections.TryGetValue(clipName, out var samples)) return;

        try {
            var targetLength = clip.samples * clip.channels;

            if (samples.Length != targetLength)
            {
                var resized = new float[targetLength];
                Array.Copy(samples, resized, Math.Min(samples.Length, targetLength));
                samples = resized;
            }

            clip.LoadAudioData();
            clip.SetData(samples, 0);
            Injected.Add(clipName);

            Log.LogInfo($"[SoundMod] Injected '{clipName}' ({samples.Length} samples)");
        }
        catch (Exception ex) {
            Log.LogWarning($"[SoundMod] Failed to inject '{clipName}': {ex.Message}");
        }

        DebugLogClip(clip, clipName);
        RecordClipInfo(clip, clipName);
        if (TechTechsSoundModLocalSettings.Instance.DumpClipInfoToggle.Value){ WriteDump(); }
    }

    private static float[] ParseWavToFloat(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        var riff = new string(br.ReadChars(4));
        if (riff != "RIFF") throw new InvalidDataException($"Not a RIFF file: '{riff}'");
        br.ReadInt32();
        var wave = new string(br.ReadChars(4));
        if (wave != "WAVE") throw new InvalidDataException($"Not a WAVE file: '{wave}'");

        int channels = 0, sampleRate = 0, bitsPerSamp = 0;
        byte[] pcmData = Array.Empty<byte>();

        while (fs.Position < fs.Length - 8)
        {
            var id   = new string(br.ReadChars(4));
            var size = br.ReadInt32();
            var next = fs.Position + size;

            if (id == "fmt ")
            {
                var fmt = br.ReadInt16();
                if (fmt != 1) throw new InvalidDataException($"Only PCM WAV supported (format={fmt}).");
                channels    = br.ReadInt16();
                sampleRate  = br.ReadInt32();
                br.ReadInt32();
                br.ReadInt16();
                bitsPerSamp = br.ReadInt16();
            }
            else if (id == "data")
            {
                pcmData = br.ReadBytes(size);
            }

            fs.Position = next;
        }

        if (sampleRate == 0) throw new InvalidDataException("Missing fmt chunk.");
        if (pcmData.Length == 0) throw new InvalidDataException("Missing data chunk.");

        int bytesPerSample = bitsPerSamp / 8;
        int sampleCount    = pcmData.Length / bytesPerSample;
        var samples        = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            int o = i * bytesPerSample;
            samples[i] = bitsPerSamp switch
            {
                8  => (pcmData[o] - 128) / 128f,
                16 => BitConverter.ToInt16(pcmData, o) / 32768f,
                24 => ((pcmData[o] | (pcmData[o+1] << 8) | (pcmData[o+2] << 16)) << 8) / 2147483648f,
                32 => BitConverter.ToInt32(pcmData, o) / 2147483648f,
                _  => throw new InvalidDataException($"Unsupported bit depth: {bitsPerSamp}")
            };
        }

        return samples;
    }

    private static void WriteReadme()
    {
        var readme = Path.Combine(SoundsFolder, "README.txt");
        if (File.Exists(readme)) return;
        File.WriteAllText(readme, SoundMap.README);
    }
}