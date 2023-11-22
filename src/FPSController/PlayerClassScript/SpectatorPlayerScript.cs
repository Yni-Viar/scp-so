using Godot;
using System;

public partial class SpectatorPlayerScript : Node3D
{
    // Left for future.
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, false);
        GetParent().GetParent<PlayerScript>().SetCollisionLayerValue(5, false);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
