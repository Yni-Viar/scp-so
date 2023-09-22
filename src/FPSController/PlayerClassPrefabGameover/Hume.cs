using Godot;
using System;

public partial class Hume : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
        await ToSignal(GetTree().CreateTimer(7.0), "timeout");
		GetNode<CpuParticles3D>("CPUParticles3D").Emitting = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
