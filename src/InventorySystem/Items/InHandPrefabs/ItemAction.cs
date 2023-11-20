using Godot;
using System;

public partial class ItemAction : Node3D
{
	[Export] internal bool oneTimeUse;
	[Export] internal int index;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		OnUpdate(delta);
	}

	internal virtual void OnStart()
	{

	}

	internal virtual void OnUpdate(double delta)
	{

	}
	/// <summary>
	/// Base method, which is default for first-person items.
	/// </summary>
	/// <param name="player"></param>
	internal virtual void OnUse(PlayerScript player)
	{
		if (oneTimeUse)
		{
            player.GetNode<Inventory>("InventoryContainer/Inventory").RemoveItem(index, false);
        }
	}
}
