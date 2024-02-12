using Godot;
using System;
/// <summary>
/// Base for compurer-based player.
/// </summary>
public partial class ComputerPlayerScript : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		OnUpdate(delta);
	}

	internal virtual void OnStart()
	{

	}

    internal virtual void OnUpdate(double delta)
	{

	}

	internal virtual void SwitchCamera(string zone, string cam)
	{

	}

    internal virtual void Reveal(string zone, string room)
	{

	}
}
