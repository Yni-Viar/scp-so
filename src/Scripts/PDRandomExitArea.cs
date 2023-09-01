using Godot;
using System;

public partial class PDRandomExitArea : Area3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	string[] rooms = new string[] { "PD/PD_basement/entityspawn" };
	string[] exitLocation = new string[] {"MapGenLCZ/LC_room1_archive/entityspawn", "MapGenRZ/RZ_room2_offices/entityspawn", "MapGenHCZ/HC_cont1_173/entityspawn"};
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
				TeleportTo(player, exitLocation[rng.RandiRange(0, 3)]);
			}
			else if (i >= 67 && i < 83)
			{
				TeleportTo(player, rooms[0]); //will be expanded in future
			}
			else
			{
				player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
			}
        }
    }

	void TeleportTo(PlayerScript player, string position)
	{
		GetParent().GetParent().GetParent<FacilityManager>().Rpc("TeleportTo", player.Name, position); 
	}
}
