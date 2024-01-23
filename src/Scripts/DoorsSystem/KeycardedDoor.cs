using Godot;
using System;
/// <summary>
/// Keycarded door manager. Available since v.0.6.0
/// </summary>
public partial class KeycardedDoor : Door
{
	enum openRequirements { key1, key2, key3, key4, key5, keyomni, scp005 }
	[Export] openRequirements requirements = openRequirements.key1;

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

    internal override void DoorController(int keycard)
    {
        base.DoorController(keycard);
        if (keycard >= (int)requirements)
        {
            PlayButtonSound(true);
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
            PlayButtonSound(false);
        }
    }

    void PlayButtonSound(bool enable)
    {
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonKeycard/ButtonSound");
        sfx.Stream = enable ? GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg") : GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse2.ogg"); ;
        sfx.Play();
        AudioStreamPlayer3D sfx2 = GetNode<AudioStreamPlayer3D>("ButtonKeycard2/ButtonSound");
        sfx2.Stream = enable ? GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg") : GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse2.ogg"); ;
        sfx2.Play();
    }

    private void InteractKeycard(Node3D player, int keycardRequire)
    {
        Rpc("DoorControl", player.GetPath(), keycardRequire);
    }
}



