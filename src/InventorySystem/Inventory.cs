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

    //Remove items from inventory
    internal Resource RemoveItem(int itemIndex)
    {
        Resource previousItem = items[itemIndex];
        items[itemIndex] = null;

        //Spawn item ingame
        if (previousItem is Item _item) // anti-dupe
        {
            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(_item.PickablePath).Instantiate();
            pickable.Position = GetParent().GetParent().GetParent().GetNode<Marker3D>("PlayerHead/ItemSpawn").GlobalPosition;
            GetTree().Root.GetNode<Node3D>("Game/Items").AddChild(pickable);
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
}
