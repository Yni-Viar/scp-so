using Godot;
using System;

public partial class EscapeArea : Area3D
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
			if (player.team == DefaultClassList.Team.CDP || player.team == DefaultClassList.Team.SCI)
			{
				player.CallForceclass("mtfe11");
			}
		}
    }
}