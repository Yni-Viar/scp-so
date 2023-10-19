using Godot;
using System;

public partial class ItemAction : Node3D
{
	[Export] internal bool oneTimeUse;
	[Export] internal int index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	internal virtual void OnUse(PlayerScript player)
	{
		if (oneTimeUse)
		{
            player.GetNode<Inventory>("InventoryContainer/Inventory").RemoveItem(index, false);
        }
	}
}
