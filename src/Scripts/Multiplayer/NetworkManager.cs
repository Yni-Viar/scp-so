using Godot;
using System;

public partial class NetworkManager : Node
{
    internal static string ipAddress;
    internal static int port;
    internal static int maxPlayers;

    ENetMultiplayerPeer peer;

    Node3D game; // replicated map.
    CharacterBody3D playerScene; // replicated player.
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //You can change settings ONLY by restarting the game.
        if (FileAccess.FileExists("user://serverconfig_0.2.0.ini"))
		{
			LoadIni();
		}
		else
		{
			IniSaver ini = new IniSaver();
			ini.SaveIni("ServerConfig", new Godot.Collections.Array<string>{
				"Port",
				"MaxPlayers"
			}, new Godot.Collections.Array{
				7877,
				20
			}, "user://serverconfig_0.2.0.ini");

            LoadIni();
		}

        peer = new ENetMultiplayerPeer();

        /* foreach (var argument in OS.GetCmdlineArgs()) // unused as for now.
        {
            if (argument == "--server")
            {
                Host();
            }
            else
            {
                continue;
            }
        }
        */
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public static void LoadIni()
	{
		var config = new ConfigFile();

		// Load data from a file.
		Error err = config.Load("user://serverconfig_0.2.0.ini");

		// If the file didn't load, ignore it.
		if (err != Error.Ok)
		{
			return;
		}
		// Fetch the data for each section.
		port = (int)config.GetValue("ServerConfig", "Port");
		maxPlayers = (int)config.GetValue("ServerConfig", "MaxPlayers");
	}

    internal void Host()
    {
        peer.CreateServer(port, maxPlayers);
        Multiplayer.MultiplayerPeer = peer;
        StartGame();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
        GD.Print("Ready for connecting!");
    }

    internal void Join()
    {
        peer.CreateClient(ipAddress, port);
        Multiplayer.MultiplayerPeer = peer;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
    }

    void StartGame()
    {
        if (Multiplayer.IsServer())
        {
            CallDeferred("LoadLevel", GD.Load<PackedScene>("res://Scenes/Facility.tscn"));
        }
    }

    void LoadLevel(PackedScene scene)
    {
        if (GetNodeOrNull("Game") != null)
        {
            foreach (Node n in GetNode("Game").GetChildren())
            {
                GetNode("Game").RemoveChild(n);
                n.QueueFree();
            }
        }
        AddChild(scene.Instantiate());
    }

    void ConnectedToServer()
    {
        GD.Print("Connected to the server!");
    }

    void ConnectionFailed()
    {
        GD.Print("Connection Failed!");
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();
    }

    
    void ServerDisconnected()
    {
        if (GetNodeOrNull("Game") != null)
        {
            GetNode("Game").QueueFree();
        }
        GD.Print("You are disconnected from the server.");
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();

    }
}
