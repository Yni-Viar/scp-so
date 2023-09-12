using Godot;
using System;

public partial class Inventory : Node
{
    internal Godot.Collections.Dictionary dragData;

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

    internal Resource GetItem(int itemIndex)
    {
        return items[itemIndex];
    }

    //Sets item in specific slot
    internal Resource SetItem(int itemIndex, Resource item)
    {
        Resource previousItem = items[itemIndex];
        items[itemIndex] = item;
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex});
        return previousItem;
    }

    //Swaps items
    internal void SwapItems(int itemIndex, int targetItemIndex)
    {
        Resource targetItem = items[targetItemIndex];
        Resource item = items[itemIndex];
        items[targetItemIndex] = item;
        items[itemIndex] = targetItem;
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex, targetItemIndex});
    }

    //Remove items from inventory, itemSpawn means will be the item spawned or not (to prevent dupe)
    internal Resource RemoveItem(int itemIndex, bool itemSpawn) 
    {
        Resource previousItem = items[itemIndex];
        items[itemIndex] = null;

        //Spawn item ingame
        if (previousItem is Item _item && itemSpawn) // anti-dupe
        {
            Rpc("DropItemRpc", _item.PickablePath);
        }
        EmitSignal(SignalName.ItemsChanged, new Godot.Collections.Array{itemIndex});
        return previousItem;
    }

    //Add items to inventory, using FOR + SetItem()
    internal void AddItem(Resource item)
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

    //Networking functions are used from https://github.com/expressobits/inventory-system
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void AddItemRpc(string itemResource)
    {
        /*if (Multiplayer.IsServer())
        {
            GD.Print("IsServer");
            return;
        }*/
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
        }*/
        RemoveItem(itemIndex, itemSpawn);
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void DropItemRpc(string pickablePath)
    {
        Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(pickablePath).Instantiate();
        // pickable.Position = 
        GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
        GetTree().Root.GetNode<Node3D>("Main/Game/Items/" + pickable.Name).Position = GetParent().GetParent().GetNode<Marker3D>("PlayerHead/ItemSpawn").GlobalPosition;
    }
}
