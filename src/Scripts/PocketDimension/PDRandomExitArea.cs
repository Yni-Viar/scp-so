using Godot;
using System;
/// <summary>
/// First pocket dimesion room code.
/// </summary>
public partial class PDRandomExitArea : Area3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	string[] pdRooms = new string[] { "Main/Game/PD/PD_basement/entityspawn" };
    string[] exitValues = new string[PlacesForTeleporting.defaultData.Values.Count];
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PlacesForTeleporting.defaultData.Values.CopyTo(exitValues, 0);
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
			if (i >= 75)
			{
				TeleportTo(player, exitValues[rng.RandiRange(0, exitValues.Length - 1)]);
			}
			else if (i >= 50 && i < 75)
			{
				TeleportTo(player, pdRooms[0]); //will be expanded in future
			}
			else
			{
				player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Decayed at SCP-106's pocket dimension.", 0);
			}
        }
    }

	void TeleportTo(PlayerScript player, string position)
	{
		GetParent().GetParent().GetParent<FacilityManager>().Rpc("TeleportTo", player.Name, position); 
	}
}
