using Godot;
using System;

public partial class Facility : Node3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	CharacterBody3D playerScene;

    CharacterBody3D scp650Scene;
	string[] spawnLocation = new string[] {"MapGen/LC_room1_archive/entityspawn", "MapGen/RZ_room2_offices/entityspawn"};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //Spawn player
		playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
		playerScene.Position = GetNode<Marker3D>(spawnLocation[rng.RandiRange(0, 1)]).GlobalPosition;
		GetNode<Node3D>("Entities").AddChild(playerScene);

        //Spawn SCP-650 (Note, that SCP-650 MUST be in null position, else it won't work properly)
		scp650Scene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://Assets/NPC/scp650npc.tscn").Instantiate();
		GetNode<Node3D>("Entities").AddChild(scp650Scene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
