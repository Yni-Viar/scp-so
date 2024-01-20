using Godot;
using System;
/// <summary>
/// Inventory screen script.
/// </summary>
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
    /*
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    */
    /// <summary>
    /// Refreshes inventory UI.
    /// </summary>
    void UpdateInventoryDisplay()
    {
        for (int itemIndex = 0; itemIndex < inventory.items.Count; itemIndex++)
        {
            UpdateInventorySlotDisplay(itemIndex);
        }
    }
    /// <summary>
    /// Refreshes single item in UI.
    /// </summary>
    /// <param name="itemIndex">Index of the item</param>
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
}
