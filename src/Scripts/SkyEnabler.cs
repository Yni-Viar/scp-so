using Godot;
using System;

public partial class SkyEnabler : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node3D body)
	{
        if (body is PlayerScript player)
        {
			GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.BackgroundMode = Godot.Environment.BGMode.Sky;
        }
    }

	private void OnBodyExited(Node3D body)
	{
        if (body is PlayerScript player)
        {
            GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.BackgroundMode = Godot.Environment.BGMode.ClearColor;
        }
    }
}