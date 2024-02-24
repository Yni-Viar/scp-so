using Godot;
using System;
/// <summary>
/// Contains default data.
/// </summary>
public partial class Globals
{
    /// <summary>
    /// Development stages. Main menu default background is dependent on this enum.
    /// </summary>
    public enum Stages { release, testing, dev };
    /// <summary>
    /// Health type. Available since 0.7.0.
    /// </summary>
    public enum HealthType { health, sanity }

    /// <summary>
    /// Version string. In 0.7.1, the value is changed to development stage, because Godot already has version setter feature.
    /// </summary>
    public static string version = "0.8.0-Release";
    /// <summary>
    /// Game data compatibility. Available since 0.8.0.
    /// </summary>
    public static string gameDataCompatibility = "0.8.0";
    /// <summary>
    /// Rooms compatibility. Available since 0.7.0.
    /// </summary>
    public static string roomsCompatibility = "0.8.0";
    /// <summary>
    /// Server config compatibility. Available since 0.7.0.
    /// </summary>
    public static string serverConfigCompatibility = "0.8.0";
    /// <summary>
    /// Current stage. Used by main menu.
    /// </summary>
    public static Stages currentStage = Stages.release;
    /// <summary>
    /// Type of class. singleSpawnClasses - SCP. multiSpawnClasses - Human, arrivingClasses - MTF, 
    /// SpecialClasses - spectator, bonusSpawnClasses - neutral SCPs, such as 131-A and -B. Available since 0.8.0-dev.
    /// </summary>
    public enum ClassType { singleSpawnClasses, multiSpawnClasses, arrivingClasses, specialClasses, bonusSpawnClasses}

    /// <summary>
    /// Types of spawnable objects. Available since 0.7.0.
    /// </summary>
    public enum ItemType { item, ammo, npc };

    /// <summary>
    /// Teams. Spectators, D-class, Scientists, MTF, Dangerous SCP entities, Normal SCP entities, Chaos Insurgency, Serpent's hand.
    /// </summary>
    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI, SEH }
    /// <summary>
    /// Ammo presets.
    /// </summary>
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    internal static Godot.Collections.Dictionary<string, string> ammo = new Godot.Collections.Dictionary<string, string>
    {
        { "9mm", "res://Assets/Ammo/9_mm.tscn" },
        { "mtf", "res://Assets/Ammo/mtf_ammo.tscn" }
    };
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    internal static Godot.Collections.Dictionary<string, string> npcs = new Godot.Collections.Dictionary<string, string>
    {
        { "scp650", "res://Assets/NPC/scp650npc.tscn" }
    };
}
