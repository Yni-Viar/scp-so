using Godot;
using System;
using System.Linq;

public partial class PDRandomExitArea : Area3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	string[] pdRooms = new string[] { "PD/PD_basement/entityspawn" };
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
			uint i = rng.Randi() % 100;
			if (i >= 83)
			{
				TeleportTo(player, PlacesForTeleporting.defaultData.Values.ToArray<string>()[rng.RandiRange(0, PlacesForTeleporting.defaultData.Values.ToArray<string>().Length - 1)]);
			}
			else if (i >= 67 && i < 83)
			{
				TeleportTo(player, pdRooms[0]); //will be expanded in future
			}
			else
			{
				player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Decayed at SCP-106's pocket dimension.");
			}
        }
    }

	void TeleportTo(PlayerScript player, string position)
	{
		GetParent().GetParent().GetParent<FacilityManager>().Rpc("TeleportTo", player.Name, position); 
	}
}
