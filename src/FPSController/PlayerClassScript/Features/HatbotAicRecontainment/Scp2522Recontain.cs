using Godot;
using System;
/// <summary>
/// SCP-2522 Recontainer.
/// </summary>
public partial class Scp2522Recontain : StaticBody3D
{
	bool recontained = false;
	[Export] internal bool recontaining = false;
	[Export] double recontainValue = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Multiplayer.IsServer())
		{
			if (recontaining)
			{
				if (recontainValue < 100)
				{
					recontainValue += 0.005;
				}
				else
				{
					Recontain();
					Rpc("Eject");
					recontained = true;
				}
			}
			else if (!recontained && recontainValue >= 0)
            {
                recontainValue -= 0.005;
            }
        }
        if (!recontained)
		{
			if (recontainValue > 0.001)
			{
				GetNode<Label3D>("Information").Text = "Recontaining a threat... Done " + Mathf.Round(recontainValue).ToString() + "%";
			}
			else
			{
				GetNode<Label3D>("Information").Text = "I am hatbot.aic :P";
            }
		}
		else
		{
			GetNode<Label3D>("Information").Text = "Target has been contained.\nServers are OK.";
			SetProcess(false);
        }
    }
	/// <summary>
	/// Inserts SCP-2306 in the computer
	/// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Insert()
	{
		if (!recontained)
		{
			recontaining = true;
		}
	}

	/// <summary>
	/// Ejects SCP-2306 from the computer.
	/// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Eject()
	{
		if (!recontained)
		{
			recontaining = false;
            if (Multiplayer.IsServer() && !recontaining)
            {
                GetTree().Root.GetNode<ItemManager>("Main/Game/Items").AddItem(15, GetNode<Marker3D>("spawn").GlobalPosition);
            }
        }
	}

	/// <summary>
	/// Recontains all SCP-2522 instances.
	/// </summary>
    void Recontain()
	{
		foreach (string peer in GetTree().Root.GetNode<FacilityManager>("Main/Game/").playersList)
		{
			if (GetTree().Root.GetNode<PlayerScript>("Main/Game/" + peer).className == "SCP-2522,\"hatbot.aic\"")
			{
				GetTree().Root.GetNode<FacilityManager>("Main/Game/").RpcId(int.Parse(peer), "SetPlayerClass", peer, 0, "Recontained by SCP-2306.", false);
            }
        }
    }
}
