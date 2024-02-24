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
    /// Spawns an object. (reworked in v.0.8.0-dev)
    /// </summary>
    /// <param name="key">Object key. Reworked in 0.8.0-dev</param>
    /// <param name="type">Type of object(0 is item, 1 is ammo, 2 is NPC)</param>
    /// <param name="playerId">Player ID. Available since 0.7.2-dev</param>
    [Obsolete("SpawnObject is an obsolete script. Use ItemManager instead...")]
    void SpawnObject(int key, int type, int playerId)
    {
        switch (type)
        {
            case 0:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items.Count && key >= 0)
                {
                    GetParent().GetNode<ItemManager>("Items").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
                }
                else
                {
                    GD.Print("Error. Could not spawn an item.");
                }
                break;
            case 1:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo.Count && key >= 0)
                {
                    GetParent().GetNode<ItemManager>("Ammo").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
                }
                else
                {
                    GD.Print("Error. Could not spawn ammo.");
                }
                break;
            case 2:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc.Count && key >= 0)
                {
                    GetParent().GetNode<ItemManager>("NPCs").RpcId(1, "CallAddOrRemoveItem", true, key, GetTree().Root.GetNode<Node3D>("Main/Game/" + Multiplayer.GetUniqueId() + "/PlayerHead/ItemSpawn").GetPath());
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

    /// <summary>
    /// Helper method to teleport. Will be moved to PlayerAction in future versions.
    /// </summary>
    /// <param name="placeToTeleport">Place to teleport</param>
    internal void CallTeleport(string placeToTeleport)
    {
        GetParent<FacilityManager>().Rpc("TeleportTo", Multiplayer.GetUniqueId().ToString(), PlacesForTeleporting.defaultData[placeToTeleport]);
    }

    /// <summary>
    /// Helper method to call FacilityManager for changing player class. Will be moved to PlayerAction in future versions.
    /// </summary>
    /// <param name="to">Player class to become</param>
    internal void CallForceclass(int to, string reason)
    {
        GetParent<FacilityManager>().Rpc("SetPlayerClass", Multiplayer.GetUniqueId().ToString(), to, reason);
    }
}
