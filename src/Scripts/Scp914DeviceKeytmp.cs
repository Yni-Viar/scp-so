using Godot;
using System;

public partial class Scp914DeviceKey : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    internal void RefineCall()
    {
        Scp914 scp = GetParent().GetParent().GetNode<Scp914>("scp914");
        if (!scp.isRefining)
        {
            scp.Rpc("Refine");
        }
    }
}
