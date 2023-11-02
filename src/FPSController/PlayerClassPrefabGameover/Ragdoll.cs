using Godot;
using System;

public partial class Ragdoll : Skeleton3D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		PhysicalBonesStartSimulation();
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
		PhysicalBonesStopSimulation();
        QueueFree();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
