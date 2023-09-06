using Godot;
using System;

public partial class Globals
{
    public static string version = "0.5.2";
    public static string milestone = "0.5.1";
    //Milestone has only two digits in version, unlike a normal version. Currently, the game is unstable, and milestone equals version, as for now.


    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI }
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classData =
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "spawnableHuman", new Godot.Collections.Array<string>{"classd1", "classd2", "scientist", "guard" } },
            { "arrivingHuman", new Godot.Collections.Array<string>{ "mtfe11" } },
            { "spawnableScps", new Godot.Collections.Array<string> { "scp106", "scp173", "scp650", "scp3199" } }
        };
}
