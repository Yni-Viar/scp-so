using Godot;
using System;

public partial class ItemList : Node
{
    public static Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>{
        {"pda", "res://InventorySystem/Items/pda.tres"},
        {"scp018", "res://InventorySystem/Items/scp018.tres"}
    };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
