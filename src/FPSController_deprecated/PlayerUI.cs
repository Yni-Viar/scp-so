using Godot;
using System;

public partial class PlayerUI : Control
{
    /* A first attempt to refactor Player Script.
     * As for now (16.05.2023), the player UI will be in separate script. (Except blinking)
     */
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetNode<Control>("PauseMenu").Visible || GetNode<ColorRect>("InventoryContainer").Visible || GetNode<InGameConsole>("InGameConsole").Visible)
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
            GetNode<Control>("PauseMenu").Visible = !(GetNode<Control>("PauseMenu").Visible);
        }

        if (Input.IsActionJustPressed("inventory"))
        {
            GetNode<ColorRect>("InventoryContainer").Visible = !(GetNode<ColorRect>("InventoryContainer").Visible);
        }

        if (Input.IsActionJustPressed("console"))
        {
            GetNode<InGameConsole>("InGameConsole").Visible = !(GetNode<InGameConsole>("InGameConsole").Visible);
        }
    }

    void SpecialScreen(bool enabled = false)
    {
        if (enabled)
        {
            GetNode<TextureRect>("Cursor").Hide();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            GetNode<TextureRect>("Cursor").Show();
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
}
