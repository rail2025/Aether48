using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace Aether48;

public enum Difficulty
{
    Easy,
    Hard,
    Insanity
}

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public int HighScore { get; set; }
    public int ThemeId { get; set; }

    public bool IsSfxMuted { get; set; }
    public bool IsBgmMuted { get; set; }
    public float MusicVolume { get; set; } = 0.5f;

    public List<int> UnlockedBonusTracks { get; set; } = new();
    public Dictionary<Difficulty, float> HighScores { get; set; } = new();
    public bool IsGameWindowLocked { get; set; } = false;
    public bool OpenOnDeath { get; set; } = false;
    public bool OpenInQueue { get; set; } = false;
    public bool OpenInPartyFinder { get; set; } = false;
    public bool OpenDuringCrafting { get; set; } = false;

    [NonSerialized]
    private IDalamudPluginInterface? _pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;
    }

    public void Save()
    {
        _pluginInterface!.SavePluginConfig(this);
    }
}
