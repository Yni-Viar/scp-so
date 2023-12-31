using Godot;
using System;
/// <summary>
/// Inventory core.
/// </summary>
public partial class Inventory : Node
{
    internal Godot.Collections.Dictionary dragData;
    /// <summary>
    /// Item changed signal.
    /// </summary>
    /// <param name="indexes">Item's index</param>
    [Signal]
    public delegate void ItemsChangedEventHandler(Godot.Collections.Array indexes);

    [Export]
    internal Godot.Collections.Array<Resource> items = new Godot.Collections.Array<Resource>{
        null, null, null, null, null, null, null, null, null, null
    };

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
    /// <summary>
    /// Gets item from inventory
    /// </summary>
    /// <param name="itemIndex">Index of the item</param>
    /// <returns>Item resource</returns>
    internal Resource GetItem(int itemIndex)
    {
        return items[itemIndex];
    }
    /// <summary>
    /// Sets item in specific slot
    /// </summary>
    /// <param name="itemIndex">Index to set</param>
    /// <param name="item">Item to set</param>
    /// <returns>Previous item resource</returns>
    internal Resource SetItem(int itemIndex, Resource item)
    {
        Resource previousItem = items[itemIndex];
        items[itemIndex] = item;
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex});
        return previousItem;
    }
    /// <summary>
    /// Swaps items
    /// </summary>
    /// <param name="itemIndex">Previous item index</param>
    /// <param name="targetItemIndex">New item index</param>
    internal void SwapItems(int itemIndex, int targetItemIndex)
    {
        Resource targetItem = items[targetItemIndex];
        Resource item = items[itemIndex];
        items[targetItemIndex] = item;
        items[itemIndex] = targetItem;
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex, targetItemIndex});
    }
    /// <summary>
    /// Remove items from inventory. Please use PlayerAction.SpawnObject for spawning items, this method is for removing items from inventory ONLY!
    /// </summary>
    /// <param name="itemIndex">Which index will be affected</param>
    /// <param name="itemSpawn">Will be the item spawned or not (to prevent dupe)</param>
    /// <returns>Removed item resource</returns>
    internal Resource RemoveItem(int itemIndex, bool itemSpawn) 
    {
        Resource previousItem = items[itemIndex];
        items[itemIndex] = null;
        //do not dupe the item
        GetParent().GetParent<PlayerScript>().Rpc("UpdateItemsInHand", "", "");
        //Spawn item ingame
        if (previousItem is Item _item && itemSpawn) // anti-dupe
        {
            Rpc("DropItemRpc", _item.PickablePath);
        }
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex});
        return previousItem;
    }
    /// <summary>
    /// Add items to inventory, using FOR + SetItem()
    /// </summary>
    /// <param name="item">Resource to add</param>
    internal void AddItem(Resource item)
    {
        if (IsMultiplayerAuthority())
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    SetItem(i, item);
                    break;
                }
            }
        }
        
    }

    //Networking functions are used from https://github.com/expressobits/inventory-system
    //Currently unused
    /*[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void AddItemRpc(string itemResource)
    {
        /*if (Multiplayer.IsServer())
        {
            GD.Print("IsServer");
            return;
        }*//*
        Resource item = ResourceLoader.Load<Resource>(itemResource);
        if (item == null)
        {
            GD.Print("IsNull");
            return;
        }
        AddItem(item);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void RemoveItemRpc(int itemIndex, bool itemSpawn)
    {
        /*if (Multiplayer.IsServer())
        {
            GD.Print("IsServer");
            return;
        }*//*
        RemoveItem(itemIndex, itemSpawn);
    }*/

    /// <summary>
    /// Drops item for all players.
    /// </summary>
    /// <param name="pickablePath">Path ro a pickable</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void DropItemRpc(string pickablePath)
    {
        Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(pickablePath).Instantiate();
        // pickable.Position = 
        GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
        GetTree().Root.GetNode<Node3D>("Main/Game/Items/" + pickable.Name).Position = GetParent().GetParent().GetNode<Marker3D>("PlayerHead/ItemSpawn").GlobalPosition;
    }
}
