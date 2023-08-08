using Godot;
using System;

public partial class Scp173PlayerScript : Node3D
{
    // [Signal]
    // public delegate void WatchingAtMeEventHandler();

    double blinkTimer = 0d;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (blinkTimer < 5.2)
        {
            blinkTimer += delta;
        }
        else //move freely
        {
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
	}

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    async void Scp173()
    {

        //If SCP-173 is not moving, it should stand still!
        if (GetParent().GetParent<PlayerScript>().CanMove && blinkTimer < 5.2d)
        {
            GetParent().GetParent<PlayerScript>().CanMove = false;
        }
        else //move while blinking.
        {
            GetParent().GetParent<PlayerScript>().CanMove = true;
            await ToSignal(GetTree().CreateTimer(0.3), "timeout");
            blinkTimer = 0d;
        }
	}
}
