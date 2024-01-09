using Godot;
using System;

public partial class Rooms : Resource
{
    /// <summary>
    /// Single endrooms
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Room1 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Single hallways
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Room2 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Single corners
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Room2C { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Single T-Ways
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Room3 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Single crosses
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Room4 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Generic endrooms
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Rooms1 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Generic hallways
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Rooms2 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Generic corners
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Rooms2C { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Generic T-Ways
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Rooms3 { get; set; } = new Godot.Collections.Array<PackedScene>();
    /// <summary>
    /// Generic crosses
    /// </summary>
    [Export]
    public Godot.Collections.Array<PackedScene> Rooms4 { get; set; } = new Godot.Collections.Array<PackedScene>();

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public Rooms() : this(null, null, null, null, null, null, null, null, null, null)
    { }

    public Rooms(Godot.Collections.Array<PackedScene> room1, Godot.Collections.Array<PackedScene> room2,
        Godot.Collections.Array<PackedScene> room2C, Godot.Collections.Array<PackedScene> room3,
        Godot.Collections.Array<PackedScene> room4, Godot.Collections.Array<PackedScene> rooms1,
        Godot.Collections.Array<PackedScene> rooms2, Godot.Collections.Array<PackedScene> rooms2C,
        Godot.Collections.Array<PackedScene> rooms3, Godot.Collections.Array<PackedScene> rooms4)
    {
        Room1 = room1;
        Room2 = room2;
        Room2C = room2C;
        Room3 = room3;
        Room4 = room4;
        Rooms1 = rooms1;
        Rooms2 = rooms2;
        Rooms2C = rooms2C;
        Rooms3 = rooms3;
        Rooms4 = rooms4;
    }
}
