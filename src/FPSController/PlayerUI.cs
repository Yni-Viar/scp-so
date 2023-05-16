using Godot;
using System;

public partial class PlayerUI : Control
{
    /* A first attempt to refactor Player Script.
     * As for now (16.05.2023), the player UI will be in separate script. (Except blinking)
     */
    bool specialScreen = false;
    bool paused = false;
    bool invOpened = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (specialScreen)
        {
            if (paused)
            {
                GetNode<Control>("PauseMenu").Show();
            }
            if (invOpened)
            {
                GetNode<ColorRect>("InventoryContainer").Show();
            }
            GetNode<TextureRect>("Cursor").Hide();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            if (!paused)
            {
                GetNode<Control>("PauseMenu").Hide();
            }
            if (!invOpened)
            {
                GetNode<ColorRect>("InventoryContainer").Hide();
            }
            GetNode<TextureRect>("Cursor").Show();
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            specialScreen = !specialScreen;
            paused = !paused;
        }

        if (Input.IsActionJustPressed("inventory"))
        {
            specialScreen = !specialScreen;
            invOpened = !invOpened;
        }
    }
}
