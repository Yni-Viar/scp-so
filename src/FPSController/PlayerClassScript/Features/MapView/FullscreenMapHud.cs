using Godot;
using System;

public partial class FullscreenMapHud : Control
{
    OptionButton mapOptions;
    GridContainer rz, lcz, hcz;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        mapOptions = GetNode<OptionButton>("OptionButton");
        rz = GetNode<GridContainer>("GridContainerRz");
        lcz = GetNode<GridContainer>("GridContainerLcz");
        hcz = GetNode<GridContainer>("GridContainerHcz");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        switch (mapOptions.GetSelectedId())
        {
            case 0:
                rz.Show();
                lcz.Hide();
                hcz.Hide();
                break;
            case 1:
                rz.Hide();
                lcz.Show();
                hcz.Hide();
                break;
            case 2:
                rz.Hide();
                lcz.Hide();
                hcz.Show();
                break;
        }
	}
}
