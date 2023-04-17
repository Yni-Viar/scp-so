using Godot;
using System;

public partial class Facility : Node3D
{
    //RandomNumberGenerator rng = new RandomNumberGenerator();
    CharacterBody3D playerScene;

    string[] spawnLocation = new string[] {"MapGen/LC_room1_archive/playerspawn", "MapGen/RZ_room2_offices/playerspawn"};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Position = playerScene.Position = GetNode<Marker3D>("MapGen/LC_cont2_650/playerspawn").GlobalPosition;
        AddChild(playerScene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
