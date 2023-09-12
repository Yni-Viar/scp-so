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
        if (GetNode<Control>("PauseMenu").Visible || GetTree().Root.GetNode<InGameConsole>("Main/CanvasLayer/InGameConsole").Visible ||
            GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible)
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
            GetNode<Control>("PauseMenu").Visible = !GetNode<Control>("PauseMenu").Visible;
        }

        if (Input.IsActionJustPressed("human_inventory"))
        {
            PlayerScript player = GetTree().Root.GetNodeOrNull<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId().ToString());
            if (player != null)
            {
                if (player.IsMultiplayerAuthority() && player.scpNumber == -1)
                {
                    GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible =
                        !GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible;
                }
            }
            
        }
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
            if (!OS.IsDebugBuild()) //this method checks is this a debug build.
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
}
