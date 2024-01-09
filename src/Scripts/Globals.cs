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
    public static string version = "0.8.0-dev";
    /// <summary>
    /// Compatibility of settings file. In later versions will be changed to a new format, similar to SCPSL one.
    /// </summary>
    public static string settingsCompatibility = "0.7.0";
    /// <summary>
    /// Game data compatibility. Available since 0.8.0.
    /// </summary>
    public static string gameDataCompatibility = "0.8.0-dev";
    /// <summary>
    /// Items compatibility. Available since 0.7.0. Deprecated at 0.8.0
    /// </summary>
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    public static string itemsCompatibility = "0.7.2";
    /// <summary>
    /// Rooms compatibility. Available since 0.7.0.
    /// </summary>
    public static string roomsCompatibility = "0.8.0";
    /// <summary>
    /// Server config compatibility. Available since 0.7.0.
    /// </summary>
    public static string serverConfigCompatibility = "0.7.2";
    /// <summary>
    /// Current stage. Used by main menu.
    /// </summary>
    public static Stages currentStage = Stages.dev;
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
    /// Default class presets.
    /// </summary>
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "spawnableHuman", new Godot.Collections.Array<string>{"classd", "scientist", "guard" } },
            { "arrivingHuman", new Godot.Collections.Array<string>{ "mtfe11" } },
            { "spawnableScps", new Godot.Collections.Array<string> { "scp106", "scp173", "scp3199" } },
            { "friendlyScps", new Godot.Collections.Array<string> { "scp131a", "scp131b" } },
            { "special", new Godot.Collections.Array<string> {"spectator" } },
        };
    /// <summary>
    /// Default item presets.
    /// </summary>
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    internal static Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>{
        { "key1", "res://InventorySystem/Items/key1.tres" },
        { "key2", "res://InventorySystem/Items/key2.tres" },
        { "key3", "res://InventorySystem/Items/key3.tres" },
        { "key4", "res://InventorySystem/Items/key4.tres" },
        { "key5", "res://InventorySystem/Items/key5.tres" },
        { "keyomni", "res://InventorySystem/Items/keyomni.tres" },
        { "pda", "res://InventorySystem/Items/pda.tres" },
        { "medkit", "res://InventorySystem/Items/medkit.tres" },
        { "scp018", "res://InventorySystem/Items/scp018.tres" },
        { "com16", "res://InventorySystem/Items/com16.tres" },
        { "mtfrifle", "res://InventorySystem/Items/mtfrifle.tres" },
        { "cage", "res://InventorySystem/Items/cage.tres" },
        { "cage_contained", "res://InventorySystem/Items/cage_contained.tres" },
        { "tranquilizer", "res://InventorySystem/Items/tranquilizer.tres" }
    };

    /// <summary>
    /// Default room presets.
    /// </summary>
    [Obsolete("Since SCP: Site Online has moved from JSONs to Resources, this parameter is deprecated since 0.8.0-dev.")]
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> roomData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "RzCommon1", new Godot.Collections.Array<string>{ "lc_room_1_endroom" } },
            { "RzCommon2", new Godot.Collections.Array<string>{ "rz_room_2"} },
            { "RzCommon2C", new Godot.Collections.Array<string>{ "rz_room_2c" } },
            { "RzCommon3", new Godot.Collections.Array<string>{ "rz_room_3" } },
            { "RzCommon4", new Godot.Collections.Array<string>{ "rz_room_4" } },
            { "RzSingle1", new Godot.Collections.Array<string>{ } },
            { "RzSingle2", new Godot.Collections.Array<string>{ "rz_room_2_offices", "rz_room_2_offices_2", "rz_room_2_poffices", "rz_room_2_toilets",
                "rz_room_2_cafeteria", "rz_room_2_servers" } },
            { "RzSingle2C", new Godot.Collections.Array<string>{ } },
            { "RzSingle3", new Godot.Collections.Array<string>{ } },
            { "RzSingle4", new Godot.Collections.Array<string>{ } },
            { "LczCommon1", new Godot.Collections.Array<string>{ "lc_room_1_endroom" } },
            { "LczCommon2", new Godot.Collections.Array<string>{ "lc_room_2", "lc_room_2_vent", "lc_room_2_offices"} },
            { "LczCommon2C", new Godot.Collections.Array<string>{ "lc_room_2c" } },
            { "LczCommon3", new Godot.Collections.Array<string>{ "lc_room_3" } },
            { "LczCommon4", new Godot.Collections.Array<string>{ "lc_room_4" } },
            { "LczSingle1", new Godot.Collections.Array<string>{ "lc_cont_1_testroom", "lc_cont_1_079", "lc_room_1_archive" } },
            { "LczSingle2", new Godot.Collections.Array<string>{ "lc_cont_2_650", "lc_cont_2_scps", "lc_room_2_sl" } },
            { "LczSingle2C", new Godot.Collections.Array<string>{ } },
            { "LczSingle3", new Godot.Collections.Array<string>{ } },
            { "LczSingle4", new Godot.Collections.Array<string>{ } },
            { "HczCommon1", new Godot.Collections.Array<string>{ "hc_room_1_endroom" } },
            { "HczCommon2", new Godot.Collections.Array<string>{ "hc_room_2"} },
            { "HczCommon2C", new Godot.Collections.Array<string>{ "hc_room_2c" } },
            { "HczCommon3", new Godot.Collections.Array<string>{ "hc_room_3" } },
            { "HczCommon4", new Godot.Collections.Array<string>{ "hc_room_4" } },
            { "HczSingle1", new Godot.Collections.Array<string>{ "hc_cont_1_173", "hc_cont_1_3199", "hc_cont_1_106", "hc_cont_1_049" } },
            { "HczSingle2", new Godot.Collections.Array<string>{ "hc_room_2_nuke", "hc_cont_2_testroom" } },
            { "HczSingle2C", new Godot.Collections.Array<string>{ } },
            { "HczSingle3", new Godot.Collections.Array<string>{ } },
            { "HczSingle4", new Godot.Collections.Array<string>{ } },
        };
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
