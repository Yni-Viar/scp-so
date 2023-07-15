using Godot;
using System;

public partial class Facility : Node3D
{
    //graphics settings field
    WorldEnvironment graphics = new WorldEnvironment();
	RandomNumberGenerator rng = new RandomNumberGenerator();
	CharacterBody3D playerScene;
    [Export] internal static bool spawn650 = false;

    CharacterBody3D scp650Scene;
	string[] spawnLocation = new string[] {"MapGenLCZ/LC_room1_archive/entityspawn", "MapGenRZ/RZ_room2_offices/entityspawn", "MapGenHCZ/HC_cont1_173/entityspawn"};

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

        if (SettingsPanel.MusicSetting)
        {
            AudioStreamPlayer sfx = GetNode<AudioStreamPlayer>("BackgroundMusic");
            sfx.Playing = true;
        }

        if (SettingsPanel.HQSetting)
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
