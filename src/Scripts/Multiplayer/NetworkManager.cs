using Godot;
using System;

public partial class NetworkManager : Node
{
    internal static string ipAddress;
    internal static int port;
    internal static int maxPlayers;

    ENetMultiplayerPeer peer;

    internal static Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();

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
        if (Input.IsActionJustPressed("console"))
        {
            GetNode<InGameConsole>("CanvasLayer/InGameConsole").Visible = !(GetNode<InGameConsole>("CanvasLayer/InGameConsole").Visible);
        }
	}

    /// <summary>
    /// Loads INI File.
    /// </summary>
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

    /// <summary>
    /// General host method.
    /// </summary>
    internal void Host()
    {
        peer.CreateServer(port, maxPlayers);
        Multiplayer.MultiplayerPeer = peer;
        LoadGame();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
        // GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI").Show();
        GD.Print("Ready for connecting!");
    }

    /// <summary>
    /// General join method.
    /// </summary>
    internal void Join()
    {
        peer.CreateClient(ipAddress, port);
        Multiplayer.MultiplayerPeer = peer;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
        // GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI").Show();
    }

    // NOT to be confused with LoadLevel, LoadGame is a serverside function, while LoadLevel - clientside.
    // Both needed to spawn a level to every player via Multiplayer Spawner.

    /// <summary>
    /// Loads the game server-side.
    /// </summary>
    void LoadGame()
    {
        if (Multiplayer.IsServer())
        {
            CallDeferred("LoadLevel", GD.Load<PackedScene>("res://Scenes/Facility.tscn"));
        }
    }

    /// <summary>
    /// Loads the game client-side.
    /// </summary>
    /// <param name="scene">Scene to load</param>
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

    /// <summary>
    /// Emitted when successfully connected to server.
    /// </summary>
    void ConnectedToServer()
    {

        GD.Print("Connected to the server!");
    }

    /// <summary>
    /// Emitted when connection is failed.
    /// </summary>
    void ConnectionFailed()
    {
        Multiplayer.MultiplayerPeer = null;
        GD.Print("Connection Failed!");
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI").Hide();
    }

    /// <summary>
    /// Emitted when server is disconnected.
    /// </summary>
    void ServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        if (GetNodeOrNull("Game") != null)
        {
            GetNode("Game").QueueFree();
        }
        GD.Print("You are disconnected from the server.");
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/PlayerUI").Hide();

    }
}
