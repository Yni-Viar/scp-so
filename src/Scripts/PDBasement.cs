using Godot;
using System;
using System.Linq;

public partial class PDBasement : StaticBody3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnActivateExitAreaBodyEntered(Node3D body)
	{
        if (body is PlayerScript player)
		{
			Rpc("Unblock");
		}
    }

    private void OnExitAreaBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
            TeleportTo(player, PlacesForTeleporting.defaultData.Values.ToArray<string>()[rng.RandiRange(0, PlacesForTeleporting.defaultData.Values.ToArray<string>().Length - 1)]);
        }
    }
    /// <summary>
    /// Unblocks the exit
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	void Unblock()
	{
		GetNode<CollisionShape3D>("ExitBlock").Disabled = true;
	}
    /// <summary>
    /// Calls TeleportTo method remotely on FacilityManager. More info in FacilityManager's method.
    /// </summary>
    /// <param name="player">Player to teleport</param>
    /// <param name="position">Position, where player will be teleported</param>
    void TeleportTo(PlayerScript player, string position)
    {
        GetParent().GetParent().GetParent<FacilityManager>().Rpc("TeleportTo", player.Name, position);
    }
}