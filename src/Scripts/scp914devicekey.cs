using Godot;
using System;

public partial class scp914devicekey : StaticBody3D
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
        scp914 scp = GetParent().GetParent().GetNode<scp914>("scp914");
        if (!scp.isRefining)
        {
            scp.Rpc("Refine");
        }
    }
}