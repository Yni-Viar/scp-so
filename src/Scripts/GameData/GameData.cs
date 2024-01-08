using Godot;
using System;
/// <summary>
/// A replacement for JSON files. (thanks Xandromeda for giving an advice)
/// </summary>
public partial class GameData : Resource
{
    [Export]
    public Godot.Collections.Array<BaseClass> Classes { get; set; } = new Godot.Collections.Array<BaseClass>();

    [Export]
    public Godot.Collections.Array<Item> Items { get; set; }

    [Export]
    public string[] RoomsRz { get; set; }

    [Export]
    public string[] RoomsLcz { get; set; }

    [Export]
    public string[] RoomsHcz { get; set; }

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public GameData() : this(null, new Godot.Collections.Array<Item>(), null, null, null) { }

    public GameData(Godot.Collections.Array<BaseClass> classes, Godot.Collections.Array<Item> items,
        string[] roomsRz,
        string[] roomsLcz, string[] roomsHcz)
    {
        Classes = classes;
        Items = items;
        RoomsRz = roomsRz ?? System.Array.Empty<string>();
        RoomsLcz = roomsLcz ?? System.Array.Empty<string>();
        RoomsHcz = roomsHcz ?? System.Array.Empty<string>();
    }
}
