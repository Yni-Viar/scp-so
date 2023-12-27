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
    void SpawnObject(string key, int type)
    {
        switch (type)
        {
            case 0:
                if (ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type).ContainsKey(key))
                {
                    string path = ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type)[key];

                    Item item = ResourceLoader.Load<Item>(path);
                    Pickable pickable = ResourceLoader.Load<PackedScene>(item.PickablePath).Instantiate<Pickable>();
                    pickable.Position = GetNode<Marker3D>("ItemSpawn").GlobalPosition;
                    GetParent().GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                }
                else
                {
                    GD.Print("Error. Could not spawn an item.");
                }
                break;
            case 1:
                if (ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type).ContainsKey(key))
                {
                    string path = ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type)[key];

                    LootableAmmo ammo = ResourceLoader.Load<PackedScene>(path).Instantiate<LootableAmmo>();
                    ammo.Position = GetNode<Marker3D>("ItemSpawn").GlobalPosition;
                    GetParent().GetParent().GetNode<Node3D>("Items").AddChild(ammo);
                }
                else
                {
                    GD.Print("Error. Could not spawn ammo.");
                }
                break;
            default:
                GD.Print("Error. Could not parse type of object. Please, use right parameters");
                break;
        }
    }
}
