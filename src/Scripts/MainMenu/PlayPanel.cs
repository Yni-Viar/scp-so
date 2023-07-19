using Godot;
using System;

public partial class PlayPanel : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnHostPressed()
    {
    	GetTree().Root.GetNode<NetworkManager>("Main").Host();
    }

    private void OnConnectPressed()
    {
    	GetTree().Root.GetNode<NetworkManager>("Main").Join();
    }

    private void OnIpAddressTextChanged(string new_text)
    {
    	NetworkManager.ipAddress = new_text;
        if (String.IsNullOrEmpty(NetworkManager.ipAddress))
        {
            NetworkManager.ipAddress = "127.0.0.1";
        }
    }

    private void OnPortTextChanged(string new_text)
    {
    	if (String.IsNullOrEmpty(new_text))
        {
            NetworkManager.port = 7877;
        }
        else
        {
            NetworkManager.port = Convert.ToInt32(new_text);
        }
    }

    private void OnMaxClientsTextChanged(string new_text)
    {
    	if (String.IsNullOrEmpty(new_text))
        {
            NetworkManager.maxPlayers = 20;
        }
        else
        {
            NetworkManager.maxPlayers = Convert.ToInt32(new_text);
        }
    }

    private void OnSingleplayerPressed()
    {
    	GetTree().ChangeSceneToFile("res://Scenes/LoadingScreen.tscn");
    }
}