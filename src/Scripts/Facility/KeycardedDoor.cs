using Godot;
using System;
/// <summary>
/// Keycarded door manager. Available since v.0.6.0
/// </summary>
public partial class KeycardedDoor : Door
{
	enum openRequirements { key1, key2, key3, key4, key5, keyomni }
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
            if (IsOpened && !animPlayer.IsPlaying())
            {
                base.DoorClose();
            }
            else if (!animPlayer.IsPlaying())
            {
                DoorOpen();
            }
        }
    }

    private void OnButtonKeycardInteracted(Node3D player, int keycardRequire)
    {
        Rpc("DoorControl", player.GetPath(), keycardRequire);
    }
}



