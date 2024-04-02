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
    public static string version = "0.8.1-Dev";
    /// <summary>
    /// Game data compatibility. Available since 0.8.0. Currently has no usage, but will be useful for mods.
    /// </summary>
    public static string gameDataCompatibility = "0.8.1";
    /// <summary>
    /// Rooms compatibility. Available since 0.7.0.
    /// </summary>
    [Obsolete("Rooms compatibility is deprecated since 0.8.1. Please use gameDataCompatibility for mods instead")]
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
    /// In future this enum will be deprecated in favor of more customisation.
    /// </summary>
    [Obsolete("This API is obsolete, and will be replaced with integer value.")]
    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI, SEH }
}
