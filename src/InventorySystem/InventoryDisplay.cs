using Godot;
using System;

public partial class InventoryDisplay : GridContainer
{
    Inventory inventory = new Inventory();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //The main inventory script is a node, so you need to assign inventory to a node...
        inventory = GetParent().GetParent().GetNode<Inventory>("Inventory");
        inventory.ItemsChanged += OnItemsChanged;
        GD.Print(inventory.items);
        //First items update
        UpdateInventoryDisplay();
        // GetTree().Root.GetNode<GDShellSharp>("GdShellSharp").AddCommand("give", new Callable(this, "AddItemAlias"), "Gives an item to inventory (experimental)");
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void UpdateInventoryDisplay()
    {
        for (int itemIndex = 0; itemIndex < inventory.items.Count; itemIndex++)
        {
            UpdateInventorySlotDisplay(itemIndex);
        }
    }

    void UpdateInventorySlotDisplay(int itemIndex)
    {
        InventorySlotDisplay inventorySlotDisplay = GetChild<InventorySlotDisplay>(itemIndex);
        Resource item = inventory.items[itemIndex];
        inventorySlotDisplay.DisplayItem(item);
    }

    void OnItemsChanged(Godot.Collections.Array indexes)
    {
        foreach (int itemIndex in indexes)
        {
            UpdateInventorySlotDisplay(itemIndex);
        }
    }

    /*string AddItemAlias(string[] args) //buggy
    {
        if (args.Length == 1)
        {
            if (ItemParser.ReadJson("user://itemlist.json").ContainsKey(args[0]))
            {
                //inventory.AddItem(ResourceLoader.Load(JsonParser.ReadJson("user://itemlist.json")[args[0]]));
                //gives the string for adding an item, rpc will convert this in resource
                AddItemCmd(ItemParser.ReadJson("user://itemlist.json")[args[0]]);
                return "Item " + args[0] + " added to inventory";
            }
            else
            {
                return "Unknown item. Cannot add to inventory.";
            }
        }
        else
        {
            return "Unknown item. Cannot add to inventory. Did you input the item?";
        }
        
    }

    void AddItemCmd(string v)
    {
        if (IsMultiplayerAuthority())
        {
            inventory.AddItem(ResourceLoader.Load(v));
        }
        /*if (!Multiplayer.IsServer())
        {
            inventory.Rpc("AddItemRpc", v);
        }
        else
        {
            
        }*/
    //}
}
