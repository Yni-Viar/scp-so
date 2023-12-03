using Godot;
using System;
/// <summary>
/// This script is created to debloat PlayerScript and is suitable for actions with player e.g. spawning items, forceclassing... Available since 0.7.0-dev
/// </summary>
public partial class PlayerAction : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	/// <summary>
	/// Sapwns an object, such as item or ammo...
	/// </summary>
	/// <param name="path">Object path</param>
	/// <param name="type">Type of object(0 is item, 1 is ammo)</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	void SpawnObject(string path, int type)
	{
		switch (type)
		{
			case 0:
                Item item = ResourceLoader.Load<Item>(path);
                Pickable pickable = ResourceLoader.Load<PackedScene>(item.PickablePath).Instantiate<Pickable>();
				pickable.Position = GetNode<Marker3D>("ItemSpawn").GlobalPosition;
				GetParent().GetParent().GetNode<Node3D>("Items").AddChild(pickable);
				break;
			case 1:
                LootableAmmo ammo = ResourceLoader.Load<PackedScene>(path).Instantiate<LootableAmmo>();
				ammo.Position = GetNode<Marker3D>("ItemSpawn").GlobalPosition;
                GetParent().GetParent().GetNode<Node3D>("Items").AddChild(ammo);
                break;
			default:
				GD.Print("Unknown object. Won't spawn...");
				break;
        }
    }
}
