using Godot;
using System;
/// <summary>
/// Button for Pocket Dimension quiz.
/// </summary>
public partial class BasementButton : InteractableCommon
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

	internal override void Interact(Node3D player)
	{
		EntryCode(Name);
	}

	void EntryCode(string name)
	{
        if (GetParent().HasMethod("Interact"))
        {
            GetParent().Call("Interact", name);
        }
    }
}