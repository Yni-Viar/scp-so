using Godot;
using System;

public partial class MapGenHudRz : GridContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        for (int i = 0; i < 144; i++)
        {
            CenterContainer slot = ResourceLoader.Load<PackedScene>("res://MapGen/HUD/SlotDisplay.tscn").Instantiate<CenterContainer>();
            AddChild(slot);
        }
        UpdateItems();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    /// <summary>
    /// Calls DisplayData() with values, retrieved from map generator.
    /// </summary>
    void UpdateItems()
    {
        MapGenerator rz = GetTree().Root.GetNode<MapGenerator>("Main/Game/MapGenRz");
        for (int i = 0; i < 144; i++)
        {
            GetChild<SlotDisplay>(i).DisplayData(rz.GetMapData()[i][0], rz.GetMapData()[i][1], "Rz");
        }
    }
}
