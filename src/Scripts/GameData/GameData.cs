using Godot;
using System;
/// <summary>
/// A replacement for JSON files. (thanks Xandromeda for giving an advice)
/// </summary>
public partial class GameData : Resource
{
    /// <summary>
    /// Class list.
    /// </summary>
    [Export]
    public Godot.Collections.Array<BaseClass> Classes { get; set; } = new Godot.Collections.Array<BaseClass> ();
    /// <summary>
    /// Item list.
    /// </summary>
    [Export]
    public Godot.Collections.Array<Item> Items { get; set; } = new Godot.Collections.Array<Item>();
    /// <summary>
    /// Ammo list.
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Ammo { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Npc list.
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Npc { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Rooms inside Zones to generate.
    /// </summary>
    [Export]
    public Godot.Collections.Array<Rooms> MapGenRooms { get; set; } = new Godot.Collections.Array<Rooms> ();
    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public GameData() : this(null, null, null, null, null) { }

    public GameData(Godot.Collections.Array<BaseClass> classes, Godot.Collections.Array<Item> items,
        Godot.Collections.Array<PackedScene> ammo, Godot.Collections.Array<PackedScene> npc, Godot.Collections.Array<Rooms> mapGenRooms)
    {
        Classes = classes;
        Items = items;
        Ammo = ammo;
        Npc = npc;
        MapGenRooms = mapGenRooms;
    }
}
