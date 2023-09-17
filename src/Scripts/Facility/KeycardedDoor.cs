using Godot;
using System;

public partial class KeycardedDoor : Node3D
{
	enum openRequirements { key1, key2, key3, key4, key5, keyomni }
	[Export] openRequirements requirements = openRequirements.key1;
    [Export] bool canOpen = true;
	[Export] bool isOpened = false;
    [Export] string[] openDoorSounds;
    [Export] string[] closeDoorSounds;

    bool CanOpen { get=>canOpen; set=>canOpen = value; }
    bool IsOpened { get=>isOpened; set=>isOpened = value; }
    AnimationPlayer animPlayer;

    /// <summary>
    /// Main control method, which checks if the door can be opened, or has the player a keycard.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
	internal void DoorControl(string playerPath)
	{
        if (CanOpen) //Regular SCPs can open up doors up to level 3
        {
            if (requirements == openRequirements.key1 && 
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key1") != null) || (GetNode<PlayerScript>(playerPath).scpNumber > 0))
			{
				DoorController();
			}
            else if (requirements == openRequirements.key2 &&
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key2") != null) || (GetNode<PlayerScript>(playerPath).scpNumber > 0))
            {
                DoorController();
            }
            else if (requirements == openRequirements.key3 && 
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key3") != null) || (GetNode<PlayerScript>(playerPath).scpNumber > 0))
            {
                DoorController();
            }//Upper levels of keycards can be opened by upcoming SCP-079
            else if (requirements == openRequirements.key4 && 
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key4") != null) || (GetNode<PlayerScript>(playerPath).scpNumber == 079))
            {
                DoorController();
            }
            else if (requirements == openRequirements.key5 && 
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key5") != null) || (GetNode<PlayerScript>(playerPath).scpNumber == 079))
            {
                DoorController();
            }
            else if (requirements == openRequirements.keyomni && 
                (GetNode(playerPath).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("keyomni") != null) || (GetNode<PlayerScript>(playerPath).scpNumber == 079))
            {
                DoorController();
            }
            else
            {
                AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
                sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse2.ogg");
                sfx.Play();
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
	/// If DoorControl check is successful, open the door (or close)
	/// </summary>
	void DoorController()
	{
        /*AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg");
        sfx.Play();*/
        if (IsOpened && !animPlayer.IsPlaying())
        {
            DoorClose();
        }
        else if (!animPlayer.IsPlaying())
        {
            DoorOpen();
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

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    private void OnButtonKeycardInteracted(CharacterBody3D player)
    {
        Rpc("DoorControl", player.GetPath());
    }
}



