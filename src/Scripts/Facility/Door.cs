using Godot;
using System;

public partial class Door : Node3D
{
	bool isOpened = false;
	internal void DoorControl()
	{
		if (isOpened)
		{
			DoorClose();
		}
		else
		{
			DoorOpen();
		}
		isOpened = !isOpened;
	}

	void DoorOpen()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open");
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>("res://Sounds/Door/DoorOpen" + Convert.ToString(rng.RandiRange(1, 3)) + ".ogg");
		sfx.Play();
	}

	void DoorClose()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open", -1, -1, true);
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>("res://Sounds/Door/DoorClose" + Convert.ToString(rng.RandiRange(1, 3)) + ".ogg");
		sfx.Play();
	}

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnButtonInteractInteracted(GodotObject player)
    {
        DoorControl();
    }
}



