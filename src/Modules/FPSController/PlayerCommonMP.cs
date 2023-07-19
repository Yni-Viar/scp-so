using Godot;
using System;

public partial class PlayerCommonMP : Node3D
{
    //A variable, that is used for SCP-173 to check, are we blinking or not.
    internal static bool isBlinking = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
