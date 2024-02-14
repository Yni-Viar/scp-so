using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// Item manager is used for managing items in world. Available in SCP: Site Online since 0.8.0-dev
/// 
/// Created by Xandromeda
/// </summary>
public partial class ItemManager : Node
{
    [Export] int objectType; //0 is item, 1 is ammo, 2 is npc.
    /// <summary>
    /// Define the structure of a world item
    /// </summary>
    public struct WorldItem
    {
        public int ItemID;
        public Vector3 Position;
        public int type; //0 is item, 1 is ammo, 2 is npc.
        // Add more item properties as needed
    }

    /// <summary>
    /// Server-side list to keep track of world items
    /// </summary>
    private List<WorldItem> worldItems = new List<WorldItem>();

    /// <summary>
    /// Server-side method to add a new item
    /// </summary>
    /// <param name="itemID">ID of the item</param>
    /// <param name="position">Position of the item</param>


    public void AddItem(int itemID, Vector3 position)
    {
        if (!ItemExistanceCheck(itemID))
        {
            return;
        }
        WorldItem newItem;
        newItem.ItemID = itemID;
        newItem.Position = position;
        newItem.type = objectType;
        // Initialize other item properties when in need

        worldItems.Add(newItem);

        // Notify clients about the new item
        Rpc(nameof(ClientAddItem), objectType, itemID, position);
    }
    /// <summary>
    /// Check, does item exists? (item type is determined by objectType of the node)
    /// </summary>
    /// <param name="itemId">ID of item.</param>
    /// <returns>Result.</returns>
    bool ItemExistanceCheck(int itemId)
    {
        switch (objectType)
        {
            case 0:
                if (itemId > GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items.Count || itemId < 0)
                {
                    GD.Print("Error. Could not spawn an item.");
                    return false;
                }
                break;
            case 1:
                if (itemId > GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo.Count || itemId < 0)
                {
                    GD.Print("Error. Could not spawn ammo.");
                    return false;
                }
                break;
            case 2:
                if (itemId > GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc.Count || itemId < 0)
                {
                    GD.Print("Error. Could not spawn an NPC.");
                    return false;
                }
                break;
            default:
                GD.Print("Error. Could not parse type of object. Please, use correct parameters");
                return false;
        }
        return true;
    }

    /// <summary>
    /// Server-side method to remove an item
    /// </summary>
    /// <param name="itemID">ID of the item</param>
    public void RemoveItem(int itemID)
    {
        worldItems.RemoveAll(item => item.ItemID == itemID);

        // Notify clients about the removed item
        ClientRemoveItem(objectType, itemID);
    }
    /// <summary>
    /// Remote Method to add an item on clients
    /// </summary>
    /// <param name="type">Type of item (0 is generic item, 1 is ammo, 2 is npc)</param>
    /// <param name="itemID">ID of the item</param>
    /// <param name="position">Position of the item</param>
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientAddItem(int type, int itemID, Vector3 position)
    {
        switch (type)
        {
            case 0:
                Pickable pickable = ResourceLoader.Load<PackedScene>(GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items[itemID].PickablePath).Instantiate<Pickable>();
                pickable.Position = position;
                AddChild(pickable);
                break;
            case 1:
                LootableAmmo ammo = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Ammo[itemID].Instantiate<LootableAmmo>();
                ammo.Position = position;
                AddChild(ammo);
                break;
            case 2:
                Node3D npc = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Npc[itemID].Instantiate<Node3D>();
                AddChild(npc);
                break;
        }
        
    }

    /// <summary>
    /// Remote Method to remove an item on clients
    /// </summary>
    /// <param name="type">Type of item (0 is generic item, 1 is ammo, 2 is npc)</param>
    /// <param name="itemID">ID of the item</param>
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void ClientRemoveItem(int type, int itemID)
    {
        switch (type)
        { 
            case 0:
                foreach (Node node in GetChildren())
                {
                    if (node is Pickable item)
                    {
                        if (item.item.InternalId == itemID)
                        {
                            node.QueueFree();
                        }
                    }
                    else
                    {
                        node.QueueFree();
                    }
                }
                break;
            case 1:
                foreach (Node node in GetChildren())
                {
                    if (node is LootableAmmo ammo)
                    {
                        if (ammo.objectId == itemID)
                        {
                            node.QueueFree();
                        }
                    }
                    else
                    {
                        node.QueueFree();
                    }
                }
                break;
            case 2:
                foreach (Node node in GetChildren())
                {
                    if (node is BaseNpc npc)
                    {
                        if (npc.objectId == itemID)
                        {
                            node.QueueFree();
                        }
                    }
                    else
                    {
                        node.QueueFree();
                    }
                }
                break;
        }
    }
    /// <summary>
    /// Server-side method to get the current list of items
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public List<WorldItem> GetWorldItems()
    {
        return worldItems;
    }
    /*
    /// <summary>
    /// Calls methods on server
    /// </summary>
    /// <param name="creation">If true creates item, otherwise destroy</param>
    /// <param name="itemId">ID of item</param>
    */
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void CallAddOrRemoveItem(bool creation, int itemId, string pos)
    {
        if (creation)
        {
            AddItem(itemId, GetTree().Root.GetNode<Node3D>("Main/Game/" + pos).GlobalPosition);
        }
        else
        {
            RemoveItem(itemId);
        }
    }
}