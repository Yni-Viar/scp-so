using Godot;
using System;
/// <summary>
/// Common door manager. Available since v.0.7.0
/// </summary>
public partial class CommonDoor : Door
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


    internal override void DoorController(int keycard)
    {
        base.DoorController(keycard);
        if (IsOpened && !animPlayer.IsPlaying())
        {
            DoorClose();
        }
        else if (!animPlayer.IsPlaying())
        {
            DoorOpen();
        }
    }

    private void OnButtonInteractInteracted(PlayerScript player)
    {
        Rpc("DoorControl", player.GetPath(), -1);
    }
}
