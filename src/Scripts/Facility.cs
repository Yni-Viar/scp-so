using Godot;
using System;

public partial class Facility : Node3D
{
    //graphics settings field
    WorldEnvironment graphics = new WorldEnvironment();
    [Export] public static bool HQSetting = true;

	RandomNumberGenerator rng = new RandomNumberGenerator();
	CharacterBody3D playerScene;
    [Export] bool spawn650 = true;

    CharacterBody3D scp650Scene;
	string[] spawnLocation = new string[] {"MapGen/LC_room1_archive/entityspawn", "MapGen/RZ_room2_offices/entityspawn", "MapGen/HC_cont1_173/entityspawn"};

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        graphics = GetNode<WorldEnvironment>("WorldEnvironment");
        //Spawn player
        playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Position = GetNode<Marker3D>(spawnLocation[rng.RandiRange(0, 2)]).GlobalPosition;
        GetNode<Node3D>("Entities").AddChild(playerScene);

        if (spawn650)
        {
            //Spawn SCP-650 (Note, that SCP-650 MUST be in null position, else it won't work properly)
            scp650Scene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://Assets/NPC/scp650npc.tscn").Instantiate();
            GetNode<Node3D>("Entities").AddChild(scp650Scene);
        }

        if (HQSetting)
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingHighQuality.tres");
        }
        else
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingLowQuality.tres");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
