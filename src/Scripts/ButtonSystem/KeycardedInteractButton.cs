using Godot;
using System;
/// <summary>
/// Button with keycard.
/// </summary>
public partial class KeycardedInteractButton : InteractableCommon
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

    internal override void Interact(Node3D _player)
    {
        if (GetParent().HasMethod("InteractKeycard"))
        {
            if (_player is PlayerScript player)
            {
                //Yni's shitcode below :(
                // Regular SCPs can open up doors up to level 3, upper levels of keycards can be opened by SCP-079
                if (GetNode<PlayerScript>(player.GetPath()).scpNumber >= 0)
                {
                    KeycardPass(2, _player);
                }
                if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("keyomni") != null)
                {
                    KeycardPass(5, _player);
                }
                else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key5") != null)
                {
                    KeycardPass(4, _player);
                }
                else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key4") != null)
                {
                    KeycardPass(3, _player);
                }
                else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key3") != null)
                {
                    KeycardPass(2, _player);
                }
                else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key2") != null)
                {
                    KeycardPass(1, _player);
                }
                else if (GetNode(player.GetPath()).GetNode("PlayerHead/PlayerHand").GetNodeOrNull("key1") != null)
                {
                    KeycardPass(0, _player);
                }
                else
                {
                    KeycardPass(-1, _player);
                }
            }
            if (_player is CctvCamera cam)
            {
                KeycardPass(5, _player);
            }
        }
    }
    /*
    /// <summary>
    /// Plays the door sound if enableSound is on.
    /// </summary>
    /// <param name="enable">Confirm or deny sound</param>
    /// */


    /// <summary>
    /// Helper method to call the system for opening doors with keycards.
    /// </summary>
    /// <param name="keycardLevel">Keycard level</param>
    /// <param name="playerScript">Player, which called the button</param>
    void KeycardPass(int keycardLevel, Node3D playerScript)
    {
        /* sample for recreating sound scheme.
        if (enableSounds)
        {
            AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("ButtonSound");
            sfx.Stream = keycardPassed ? GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse1.ogg") : GD.Load<AudioStream>("res://Sounds/Interact/KeycardUse2.ogg");
            sfx.Play();
        }
        */
        GetParent().Call("InteractKeycard", playerScript, keycardLevel);
    }
}
