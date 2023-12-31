using Godot;
using System;
/// <summary>
/// Player UI script.
/// </summary>
public partial class PlayerUI : Control
{
    internal bool SpecialScreen = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (SpecialScreen)
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

        if (GetParent<FacilityManager>().IsRoundStarted)
        {
            GetParent().GetNode<Panel>("PreRoundStartPanel").Visible = false;
        }
        else
        {
            GetParent().GetNode<Panel>("PreRoundStartPanel").Visible = true;
        }
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            InputValues("exitgame");
        }

        if (Input.IsActionJustPressed("human_inventory"))
        {
            InputValues("inventory");
        }

        if (Input.IsActionJustPressed("player_list"))
        {
            InputValues("playerslist");
        }
        if (Input.IsActionJustPressed("console"))
        {
            InputValues("console");
        }
    }

    void InputValues(string state)
    {
        switch (state)
        {
            case "console":
                if (!SpecialScreen)
                {
                    GetNode<Control>("AdminConsole").Visible = true;
                    SpecialScreen = true;
                }
                else
                {
                    GetNode<Control>("AdminConsole").Visible = false;
                    GetTree().Root.GetNode<InGameConsole>("Main/CanvasLayer/InGameConsole").Visible = false;
                    SpecialScreen = false;
                }
                break;
            case "playerlist":
                if (GetNode<Panel>("PlayerListPanel").Visible)
                {
                    foreach (Node node in GetNode("PlayerListPanel/PlayerList").GetChildren())
                    {
                        node.QueueFree();
                    }
                    GetNode<Panel>("PlayerListPanel").Visible = false;
                    SpecialScreen = false;
                }
                else if (!SpecialScreen)
                {
                    foreach (string item in GetParent<FacilityManager>().playersList)
                    {
                        Label label = new Label();
                        label.Text = GetParent().GetNode<PlayerScript>(item).playerName;
                        GetNode("PlayerListPanel/PlayerList").AddChild(label);
                    }
                    GetNode<Panel>("PlayerListPanel").Visible = true;
                    SpecialScreen = true;
                }
                break;
            case "inventory":
                PlayerScript player = GetTree().Root.GetNodeOrNull<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId().ToString());
                if (player != null)
                {
                    if (player.IsMultiplayerAuthority() && player.scpNumber == -1)
                    {
                        if (!SpecialScreen)
                        {
                            GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible = true;
                            SpecialScreen = true;
                        }
                        else
                        {
                            GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible = false;
                            SpecialScreen = false;
                        }
                    }
                }
                break;
            case "exitgame":
                if (!SpecialScreen)
                {
                    GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Visible = true;
                    SpecialScreen = true;
                }
                else
                {
                    GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Visible = false;
                    SpecialScreen = false;
                }
                break;
        }
    }
}
