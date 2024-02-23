using Godot;
using System;

public partial class SkyEnabler : Area3D
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
	private void OnBodyEntered(Node3D body)
	{
        if (body is PlayerScript player)
        {
			if (player.IsMultiplayerAuthority())
			{
				GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.BackgroundMode = Godot.Environment.BGMode.Sky;
                if (GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.FogEnabled && !GetTree().Root.GetNode<Settings>("Settings").FogSetting)
				{
					GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.FogEnabled = false;
                }

            }
        }
    }

	private void OnBodyExited(Node3D body)
	{
        if (body is PlayerScript player)
        {
			if (player.IsMultiplayerAuthority())
			{
				GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.BackgroundMode = Godot.Environment.BGMode.Color;
				if (!GetTree().Root.GetNode<Settings>("Settings").FogSetting)
				{
					GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment.FogEnabled = true;
                }
			}
        }
    }
}