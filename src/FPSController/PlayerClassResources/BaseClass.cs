using Godot;
using System;
/// <summary>
/// FacilityManager Class definition.
/// </summary>
public partial class BaseClass : Resource
{
    /// <summary>
    /// Name of the class. Appears on HUD when forceclassing.
    /// </summary>
    [Export]
    public string ClassName { get; set; }
    /// <summary>
    /// Description of the class. Appears on HUD when forceclassing.
    /// </summary>
    [Export(PropertyHint.MultilineText)]
    public string ClassDescription { get; set; }
    /// <summary>
    /// Class' spawnpoints.
    /// </summary>
    [Export]
    public string[] SpawnPoints { get; set; }
    /// <summary>
    /// Model and behaviour source.
    /// </summary>
    [Export]
    public string PlayerModelSource { get; set; }
    /// <summary>
    /// Ragdoll or death anim source.
    /// </summary>
    [Export]
    public string PlayerRagdollSource { get; set; }
    /// <summary>
    /// Speed of class.
    /// </summary>
    [Export]
    public float Speed { get; set; }
    /// <summary>
    /// How much can jump?
    /// </summary>
    [Export]
    public float Jump { get; set; }
    /// <summary>
    /// Enable sprint.
    /// </summary>
    [Export]
    public bool SprintEnabled { get; set; }
    /// <summary>
    /// Enable moving sounds.
    /// </summary>
    [Export]
    public bool MoveSoundsEnabled { get; set; }
    /// <summary>
    /// Footstep sounds. Not required if MoveSounds are disabled.
    /// </summary>
    [Export]
    public string[] FootstepSounds { get; set; }
    /// <summary>
    /// Sprint sounds. Not required if MoveSounds and sprint are disabled.
    /// </summary>
    [Export]
    public string[] SprintSounds { get; set; }
    /// <summary>
    /// Number of SCP. -2 - spectator, -1 - human, 0 and more - SCP.
    /// </summary>
    [Export]
    public int ScpNumber { get; set; }
    /// <summary>
    /// Team of class.
    /// </summary>
    [Export]
    public Globals.Team Team { get; set; }
    /// <summary>
    /// Health
    /// </summary>
    [Export]
    public float Health { get; set; }
    /// <summary>
    /// Sanity
    /// </summary>
    [Export]
    public float Sanity { get; set; }
    /// <summary>
    /// If custom spawn, the player spawns not on spawn point, but on place, where they were forceclassed.
    /// </summary>
    [Export]
    public bool CustomSpawn { get; set; }
    /// <summary>
    /// Position of the camera. Available since 0.6.0.
    /// </summary>
    [Export] 
    public float DefaultCameraPos { get; set; }
    /// <summary>
    /// Custom camera (used by ComputerPlayerScript). Available since 0.6.0.
    /// </summary>
    [Export]
    public bool CustomCamera { get; set; }
    /// <summary>
    /// Default camera's shder. Available since 0.6.0.
    /// </summary>
    [Export]
    public string CustomView { get; set; }
    /// <summary>
    /// Preloaded items.
    /// </summary>
    [Export] // Godot types NEEDS initialization before use, or you will stuck with Nil.
    public string[] PreloadedItems { get; set; }
    /// <summary>
    /// Preloaded ammo. Available since 0.7.0.
    /// </summary>
    [Export]
    public int[] Ammo {  get; set; }
    /// <summary>
    /// Color of class.
    /// </summary>
    [Export]
    public Color ClassColor { get; set; }
    /// <summary>
    /// Player class type. Since 0.8.0-dev this is required, because FacilityManager sorts player classes into categories.
    /// </summary>
    [Export]
    public Globals.ClassType ClassType { get; set; }

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public BaseClass() : this(null, null, null, null, null, 0f, 0f, true, true, null, null, -1,
        Globals.Team.SPT, 100f, 50f, false, false, null, null, 0.969f, null, new Color(1, 1, 1), Globals.ClassType.specialClasses) { }

    public BaseClass(string className, string classDescription, string[] spawnPoints, string playerModelSource, string playerRagdollSource,
        float speed, float jump, bool sprintEnabled, bool moveSoundsEnabled, string[] footstepSounds,
        string[] sprintSounds, int scpNumber, Globals.Team team, float health, float sanity, bool customSpawn, bool customCamera, string customView,
        string[] preloadedItems, float defaultCameraPos, int[] ammo, Color classColor, Globals.ClassType classType)
    {
        ClassName = className;
        ClassDescription = classDescription;
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
        Sanity = sanity;
        CustomSpawn = customSpawn;
        CustomCamera = customCamera;
        CustomView = customView;
        PreloadedItems = preloadedItems ?? System.Array.Empty<string>();
        DefaultCameraPos = defaultCameraPos;
        Ammo = ammo ?? System.Array.Empty<int>();
        ClassColor = classColor;
        ClassType = classType;
    }
}
