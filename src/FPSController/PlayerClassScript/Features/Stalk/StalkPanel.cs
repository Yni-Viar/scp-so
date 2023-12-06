using Godot;
using System;
using System.Linq;

public partial class StalkPanel : GridContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        string[] allStalkingPos = PlacesForTeleporting.defaultData.Keys.ToArray<string>();
        for (int i = 0; i < PlacesForTeleporting.defaultData.Count; i++)
        {
            StalkButton button = ResourceLoader.Load<PackedScene>("res://FPSController/PlayerClassScript/Features/Stalk/StalkButton.tscn").Instantiate<StalkButton>();
            button.Text = allStalkingPos[i];
            AddChild(button);
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
