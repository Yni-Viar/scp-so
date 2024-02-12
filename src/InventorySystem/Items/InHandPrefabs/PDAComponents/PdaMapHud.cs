using Godot;
using System;

public partial class PdaMapHud : Control
{
	int currentViewList = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetChild<Control>(currentViewList).Visible = true;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Visible)
		{
			if (Input.IsActionJustPressed("move_left_alt") && currentViewList > 0)
			{
				GetChild<Control>(currentViewList).Visible = false;
				currentViewList--;
				GetChild<Control>(currentViewList).Visible = true;
			}
            if (Input.IsActionJustPressed("move_right_alt") && currentViewList < GetChildren().Count - 1)
            {
                GetChild<Control>(currentViewList).Visible = false;
                currentViewList++;
                GetChild<Control>(currentViewList).Visible = true;
            }
        }
	}
}
