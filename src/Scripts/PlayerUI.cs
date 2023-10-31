using Godot;
using System;

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
            if (SpecialScreen)
            {
                GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Visible = false;
                SpecialScreen = false;
            }
            else
            {
                GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Visible = true;
                SpecialScreen = true;
            }
        }

        if (Input.IsActionJustPressed("human_inventory"))
        {
            PlayerScript player = GetTree().Root.GetNodeOrNull<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId().ToString());
            if (player != null)
            {
                if (player.IsMultiplayerAuthority() && player.scpNumber == -1)
                {
                    if (SpecialScreen)
                    {
                        GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible = false;
                        SpecialScreen = false;
                    }
                    else
                    {
                        GetTree().Root.GetNodeOrNull<ColorRect>("Main/Game/" + Multiplayer.GetUniqueId().ToString() + "/InventoryContainer").Visible = true;
                        SpecialScreen = true;
                    }
                }
            }
            
        }

        if (Input.IsActionJustPressed("player_list"))
        {
            if (GetNode<Panel>("PlayerListPanel").Visible)
            {
                foreach (Node node in GetNode("PlayerListPanel/PlayerList").GetChildren())
                {
                    node.QueueFree();
                }
                GetNode<Panel>("PlayerListPanel").Visible = false;
                SpecialScreen = false;
            }
            else
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
            
        }
    }
}
