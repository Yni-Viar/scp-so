using Godot;
using System;
/// <summary>
/// Contains default data.
/// </summary>
public partial class Globals
{
    public enum Stages { release, testing, dev };
    public static string version = "0.7.0-dev";
    //Milestone is a minimal version, compatible with current release.
    public static string settingsCompatibility = "0.7.0-dev";
    public static string classesCompatibility = "0.7.0-dev";
    public static string itemsCompatibility = "0.7.0-dev";
    public static string roomsCompatibility = "0.7.0-dev";
    public static Stages currentStage = Stages.dev;


    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI }
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "spawnableHuman", new Godot.Collections.Array<string>{"classd1", "classd2", "scientist", "guard" } },
            { "arrivingHuman", new Godot.Collections.Array<string>{ "mtfe11" } },
            { "spawnableScps", new Godot.Collections.Array<string> { "scp079", "scp106", "scp131a", "scp131b", "scp173", "scp3199" } },
            { "special", new Godot.Collections.Array<string> {"spectator" } },
        };

    public static Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>{
        { "key1", "res://InventorySystem/Items/key1.tres" },
        { "key2", "res://InventorySystem/Items/key2.tres" },
        { "key3", "res://InventorySystem/Items/key3.tres" },
        { "key4", "res://InventorySystem/Items/key4.tres" },
        { "key5", "res://InventorySystem/Items/key5.tres" },
        { "keyomni", "res://InventorySystem/Items/keyomni.tres" },
        { "pda", "res://InventorySystem/Items/pda.tres" },
        { "medkit", "res://InventorySystem/Items/medkit.tres" },
        { "scp018", "res://InventorySystem/Items/scp018.tres" }
    };

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
            { "LczSingle1", new Godot.Collections.Array<string>{ "lc_room_1_archive" } },
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
}
