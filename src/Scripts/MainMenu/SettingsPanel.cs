using Godot;
using System;

public partial class SettingsPanel : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void OnBackPressed()
	{
		GetParent().GetParent().GetNode<Control>("Settings").Hide();
	}

    private void OnHqSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            Facility.HQSetting = true;
        }
        else
        {
            Facility.HQSetting = false;
        }
    }
}



