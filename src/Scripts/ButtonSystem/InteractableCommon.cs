using Godot;
using System;
/// <summary>
/// The core of buttons.
/// </summary>
public partial class InteractableCommon : Node3D
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

	internal virtual void Interact(Node3D player)
	{
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
        sfx.Play();
    }
}
