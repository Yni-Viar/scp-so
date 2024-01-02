using Godot;
using System;
/// <summary>
/// This script is handling spawning items, forceclassing... Available since 0.7.0-dev
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
    /// Spawns an object.
    /// </summary>
    /// <param name="key">Object name, as stated in relevant config files</param>
    /// <param name="type">Type of object(0 is item, 1 is ammo, 2 is NPC)</param>
    /// <param name="playerId">Player ID. Available since 0.7.2-dev</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnObject(string key, int type, int playerId)
    {
        switch (type)
        {
            case 0:
                if (ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type).ContainsKey(key))
                {
                    string path = ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type)[key];

                    Item item = ResourceLoader.Load<Item>(path);
                    Pickable pickable = ResourceLoader.Load<PackedScene>(item.PickablePath).Instantiate<Pickable>();
                    pickable.Position = GetNode<Marker3D>(playerId + "/PlayerHead/ItemSpawn").GlobalPosition;
                    GetParent().GetNode<Node3D>("Items").AddChild(pickable);
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
                    ammo.Position = GetNode<Marker3D>(playerId + "/PlayerHead/ItemSpawn").GlobalPosition;
                    GetParent().GetNode<Node3D>("Items").AddChild(ammo);
                }
                else
                {
                    GD.Print("Error. Could not spawn ammo.");
                }
                break;
            case 2:
                if (ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type).ContainsKey(key))
                {
                    string path = ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", (Globals.ItemType)type)[key];

                    Node3D npc = ResourceLoader.Load<PackedScene>(path).Instantiate<Node3D>();
                    GetParent().GetNode<Node3D>("NPCs").AddChild(npc);
                }
                else
                {
                    GD.Print("Error. Could not spawn an NPC.");
                }
                break;
            default:
                GD.Print("Error. Could not parse type of object. Please, use correct parameters");
                break;
        }
    }
}
