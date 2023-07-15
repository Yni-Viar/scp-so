using Godot;
using System;

public partial class PlayerUI : Control
{
    /* A first attempt to refactor Player Script.
     * As for now (16.05.2023), the player UI will be in separate script. (Except blinking)
     */
    bool specialScreen = false;
    bool visible;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (specialScreen)
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

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            specialScreen = !specialScreen;
            GetNode<Control>("PauseMenu").Visible = !(GetNode<Control>("PauseMenu").Visible);
        }

        if (Input.IsActionJustPressed("inventory"))
        {
            specialScreen = !specialScreen;
            GetNode<ColorRect>("InventoryContainer").Visible = !(GetNode<ColorRect>("InventoryContainer").Visible);
        }

        if (Input.IsActionJustPressed("console"))
        {
            specialScreen = !specialScreen;
            GetNode<InGameConsole>("InGameConsole").Visible = !(GetNode<InGameConsole>("InGameConsole").Visible);
        }
    }
}
