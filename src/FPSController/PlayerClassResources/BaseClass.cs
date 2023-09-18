using Godot;
using System;

public partial class BaseClass : Resource
{
    [Export]
    public string ClassName { get; set; }

    [Export]
    public string[] SpawnPoints { get; set; }

    [Export]
    public string PlayerModelSource { get; set; }

    [Export]
    public string PlayerRagdollSource { get; set; }

    [Export]
    public float Speed { get; set; }

    [Export]
    public float Jump { get; set; }

    [Export]
    public bool SprintEnabled { get; set; }

    [Export]
    public bool MoveSoundsEnabled { get; set; }

    [Export]
    public string[] FootstepSounds { get; set; }

    [Export]
    public string[] SprintSounds { get; set; }

    [Export]
    public int ScpNumber { get; set; }

    [Export]
    public Globals.Team Team { get; set; }

    [Export]
    public float Health { get; set; }

    [Export]
    public bool CustomSpawn { get; set; }

    [Export] // Godot types NEEDS initialization before use, or you will stuck with Nil.
    public string[] PreloadedItems { get; set; }

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public BaseClass() : this(null, null, null, null, 0f, 0f, true, true, null, null, -1, Globals.Team.SPT, 100f, false, null) { }

    public BaseClass(string className, string[] spawnPoints, string playerModelSource, string playerRagdollSource, 
        float speed, float jump, bool sprintEnabled, bool moveSoundsEnabled, string[] footstepSounds,
        string[] sprintSounds, int scpNumber, Globals.Team team, float health, bool customSpawn, string[] preloadedItems)
    {
        ClassName = className;
        SpawnPoints = spawnPoints ?? System.Array.Empty<string>();
        PlayerModelSource = playerModelSource;
        PlayerRagdollSource = playerRagdollSource;
        Speed = speed;
        Jump = jump;
        SprintEnabled = sprintEnabled;
        MoveSoundsEnabled = moveSoundsEnabled;
        FootstepSounds = footstepSounds ?? System.Array.Empty<string>();
        SprintSounds = sprintSounds ?? System.Array.Empty<string>();
        ScpNumber = scpNumber;
        Team = team;
        Health = health;
        CustomSpawn = customSpawn;
        PreloadedItems = preloadedItems ?? System.Array.Empty<string>();
    }
}
