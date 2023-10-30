using Godot;
using System;

public partial class PauseMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    
    private void OnExitButtonPressed()
    {
        Multiplayer.MultiplayerPeer = null;
        if (GetTree().Root.GetNodeOrNull("Main/Game") != null)
        {
            GetTree().Root.GetNode("Main/Game").QueueFree();
        }
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Hide();
    }
}



