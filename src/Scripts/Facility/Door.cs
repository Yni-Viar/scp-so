using Godot;
using System;

public partial class Door : Node3D
{
    [Export] bool canOpen = true;
	[Export] bool isOpened = false;
    [Export] string[] openDoorSounds;
    [Export] string[] closeDoorSounds;

    bool CanOpen { get=>canOpen; set=>canOpen = value; }
    bool IsOpened { get=>isOpened; set=>isOpened = value; }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	internal void DoorControl()
	{
        if (CanOpen)
        {
            if (IsOpened)
            {
                DoorClose();
            }
            else
            {
                DoorOpen();
            }
        }
		else
        {
            AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
            sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/Button2.ogg");
            sfx.Play();
        }
	}

	void DoorOpen()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open");
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>(openDoorSounds[rng.RandiRange(0, openDoorSounds.Length - 1)]);
		sfx.Play();
        IsOpened = true;
	}

	void DoorClose()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open", -1, -1, true);
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>(closeDoorSounds[rng.RandiRange(0, closeDoorSounds.Length - 1)]);
		sfx.Play();
        IsOpened = false;
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
        Rpc("DoorControl");
    }
}



