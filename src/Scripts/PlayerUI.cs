using Godot;
using System;

public partial class PlayerUI : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI/PauseMenu").Visible || GetTree().Root.GetNode<InGameConsole>("Main/CanvasLayer/InGameConsole").Visible)
        {
            SpecialScreen(true);
        }
        else
        {
            SpecialScreen(false);
        }
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI/PauseMenu").Visible = !(GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI/PauseMenu").Visible);
        }

        /*if (Input.IsActionJustPressed("inventory"))
        {
            GetNode<ColorRect>("InventoryContainer").Visible = !(GetNode<ColorRect>("InventoryContainer").Visible);
        }*/
    }

    internal void SpecialScreen(bool enabled = false)
    {
        if (enabled)
        {
            GetNode<TextureRect>("Cursor").Hide();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            GetNode<TextureRect>("Cursor").Show();
            // Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
}
