using Godot;
using System;

public partial class OmegaButton : Node3D
{
	/*
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	*/

	internal void Interact(PlayerScript player)
	{
		if (GetTree().Root.GetNode<WarheadController>("Main/Game/OmegaWarhead").detonationProgress)
		{
			GetTree().Root.GetNode<WarheadController>("Main/Game/OmegaWarhead").Rpc("Cancel");
        }
	}
}
