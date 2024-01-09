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
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnObject(int key, int type, int playerId)
    {
        switch (type)
        {
            case 0:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items.Count && key >= 0)
                {
                    Pickable pickable = ResourceLoader.Load<PackedScene>(GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items[key].PickablePath).Instantiate<Pickable>();
                    pickable.Position = GetParent().GetNode<Marker3D>(playerId + "/PlayerHead/ItemSpawn").GlobalPosition;
                    GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                }
                else
                {
                    GD.Print("Error. Could not spawn an item.");
                }
                break;
            case 1:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo.Count && key >= 0)
                {
                    LootableAmmo ammo = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo[key].Instantiate<LootableAmmo>();
                    ammo.Position = GetParent().GetNode<Marker3D>(playerId + "/PlayerHead/ItemSpawn").GlobalPosition;
                    GetParent().GetNode<Node3D>("Items").AddChild(ammo);
                }
                else
                {
                    GD.Print("Error. Could not spawn ammo.");
                }
                break;
            case 2:
                if (key < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc.Count && key >= 0)
                {
                    Node3D npc = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc[key].Instantiate<Node3D>();
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
