using Godot;
using System;
/// <summary>
/// Common door manager.
/// </summary>
public partial class Door : Node3D
{
    [Export] bool canOpen = true;
	[Export] bool isOpened = false;
    [Export] string[] openDoorSounds;
    [Export] string[] closeDoorSounds;
    [Export] string lockDoorSound;

    bool CanOpen { get=>canOpen; set=>canOpen = value; }
    bool IsOpened { get=>isOpened; set=>isOpened = value; }
    AnimationPlayer animPlayer;

    /// <summary>
    /// Main control method, which checks - is the door opened.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	internal void DoorControl()
	{
        if (CanOpen)
        {
            if (IsOpened && !animPlayer.IsPlaying())
            {
                DoorClose();
            }
            else if (!animPlayer.IsPlaying())
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
	/// <summary>
	/// Helper method, opens the door.
	/// </summary>
	void DoorOpen()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        animPlayer.Play("door_open");
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>(openDoorSounds[rng.RandiRange(0, openDoorSounds.Length - 1)]);
        sfx.Play();
        IsOpened = true;
    }
    /// <summary>
    /// Helper method, close the door.
    /// </summary>
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
    /// <summary>
    /// Temporary solution - every door currently has 5 second lock cooldown.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    async void DoorLock()
    {
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>(lockDoorSound);
        sfx.Play();
        canOpen = false;
        await ToSignal(GetTree().CreateTimer(5.0), "timeout");
        canOpen = true;
    }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        if (isOpened)
        {
            DoorOpen();
        }
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



