using Godot;
using System;

public partial class Scp650PlayerScript : Node3D
{
    bool isWatching = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetParent().GetParent<PlayerScript>().CanMove = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("scp650teleport") && !isWatching)
        {
            GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("RandomTeleport", GetParent().GetParent<PlayerScript>().Name);
        }
        else
        {
            GD.Print("Humans are watching!");
        }
	}

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    void Scp650(bool watching)
    {
        if (watching)
        {
            isWatching = true;
        }
        else
        {
            isWatching = false;
        }
	}
}
