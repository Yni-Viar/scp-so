using Godot;
using System;

public partial class Globals
{
    public static string version = "0.6.0";
    public static string milestone = "0.6.0";
    //Milestone is a minimal version, compatible with current release.


    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI }
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "spawnableHuman", new Godot.Collections.Array<string>{"classd1", "classd2", "scientist", "guard" } },
            { "arrivingHuman", new Godot.Collections.Array<string>{ "mtfe11" } },
            { "spawnableScps", new Godot.Collections.Array<string> { "scp106", "scp131a", "scp131b", "scp173", "scp650", "scp3199" } }
        };

    public static Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>{
        { "key1", "res://InventorySystem/Items/key1.tres" },
        { "key2", "res://InventorySystem/Items/key2.tres" },
        { "key3", "res://InventorySystem/Items/key3.tres" },
        { "key4", "res://InventorySystem/Items/key4.tres" },
        { "key5", "res://InventorySystem/Items/key5.tres" },
        { "keyomni", "res://InventorySystem/Items/keyomni.tres" },
        { "pda", "res://InventorySystem/Items/pda.tres" }
    };
}
