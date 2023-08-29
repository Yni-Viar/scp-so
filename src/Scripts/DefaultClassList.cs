using Godot;
using System;


public class DefaultClassList
{
    public enum Team { SPT, CDP, SCI, MTF, DSE, NSE, CHI}
    internal static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classData = 
        new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> {
            { "spawnableHuman", new Godot.Collections.Array<string>{"classd1", "classd2", "guard" } },
            { "arrivingHuman", new Godot.Collections.Array<string>{ "mtfe11" } },
            { "spawnableScps", new Godot.Collections.Array<string> { "scp173", "scp650", "scp3199" } }
        };
}