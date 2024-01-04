using Godot;
using System;

public partial class NetworkManager : Node
{
    internal static string ipAddress;
    internal static int port;
    internal static int maxPlayers;
    internal bool friendlyFire;
    internal bool spawnNpcs;
    internal int maxObjects;
    internal uint maxScps;
    string moderatorPassword = "";
    string adminPassword;
    internal int GetModerator
    {
        get => moderatorPassword.GetHashCode();
    }
    internal int GetAdmin
    {
        get => adminPassword.GetHashCode();
    }
    //True breach simulation - if enabled, second SCP will be 2522 and if currentPlayerCounter % 8 equals 4 - then a new SCP is present in round.
    //Else, players becomes SCP only if is second player or multiple of 8. Will be changed in future updates.
    internal static bool tBrSim; 

    ENetMultiplayerPeer peer;

    internal static Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();

    Node3D game; // replicated map.
    //CharacterBody3D playerScene; // replicated player.
    bool loading = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //You can change settings ONLY by restarting the game.
        if (FileAccess.FileExists("user://serverconfig_" + Globals.serverConfigCompatibility + ".ini"))
        {
            LoadIni();
        }
        else
        {
            IniSaver ini = new IniSaver();
            ini.SaveIni("ServerConfig", new Godot.Collections.Array<string>{
                "Port",
                "MaxPlayers",
                "FriendlyFire", 
                "TrueBreachSimulation",
                "AdminPassword",
                "ModeratorPassword",
                "MaxSpawnableObjects",
                "MaxScps",
                "SpawnNpcs"
            }, new Godot.Collections.Array{
                7877,
                20,
                false, 
                false,
                "changeitplease",
                "",
                12,
                3,
                true
            }, "user://serverconfig_" + Globals.serverConfigCompatibility + ".ini");
            LoadIni();
        }

        if (!FileAccess.FileExists("user://ipbans.txt"))
        {
            TxtParser.Save("user://ipbans.txt", "");
        }

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
        if (loading)
        {
            Godot.Collections.Array progress = new Godot.Collections.Array();
            var status = ResourceLoader.LoadThreadedGetStatus("res://Scenes/Facility.tscn", progress);
            if (status == ResourceLoader.ThreadLoadStatus.InProgress) //change the loading progress value
            {
                //Loading screen.
                GetTree().Root.GetNode<ProgressBar>("Main/LoadingScreen/MainPanel/ProgressBar").Value = (double)progress[0] * 100;
            }
            else if (status == ResourceLoader.ThreadLoadStatus.Loaded) //spawn a loaded scene!
            {
                GetTree().Root.GetNode<ProgressBar>("Main/LoadingScreen/MainPanel/ProgressBar").Value = 100;
                PrepareLevel(ResourceLoader.LoadThreadedGet("res://Scenes/Facility.tscn") as PackedScene);
                loading = false;
            }
        }
    }

    /// <summary>
    /// Loads INI File.
    /// </summary>
    public void LoadIni()
    {
        var config = new ConfigFile();

        // Load data from a file.
        Error err = config.Load("user://serverconfig_" + Globals.serverConfigCompatibility + ".ini");

        // If the file didn't load, ignore it.
        if (err != Error.Ok)
        {
            return;
        }
        // Fetch the data for each section.
        port = (int)config.GetValue("ServerConfig", "Port");
        maxPlayers = (int)config.GetValue("ServerConfig", "MaxPlayers");
        friendlyFire = (bool)config.GetValue("ServerConfig", "FriendlyFire");
        tBrSim = (bool)config.GetValue("ServerConfig", "TrueBreachSimulation");
        adminPassword = (string)config.GetValue("ServerConfig", "AdminPassword");
        moderatorPassword = (string)config.GetValue("ServerConfig", "ModeratorPassword");
        maxObjects = (int)config.GetValue("ServerConfig", "MaxSpawnableObjects");
        maxScps = (uint)config.GetValue("ServerConfig", "MaxScps");
        spawnNpcs = (bool)config.GetValue("ServerConfig", "SpawnNpcs");
    }

    /// <summary>
    /// General host method.
    /// </summary>
    internal void Host()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateServer(port, maxPlayers);
        Multiplayer.MultiplayerPeer = peer;
        LoadGame();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
        GD.Print("Server started. Ready for connecting!");
    }

    /// <summary>
    /// General join method.
    /// </summary>
    internal void Join()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(ipAddress, port);
        Multiplayer.MultiplayerPeer = peer;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Hide();
        GetTree().Root.GetNode<CanvasLayer>("Main/LoadingScreen/").Visible = true;
        loading = true;
    }

    /// <summary>
    /// Loads the game server-side.
    /// NOT to be confused with LoadLevel, LoadGame is a serverside function, while LoadLevel - clientside.
    /// Both needed to spawn a level to every player via Multiplayer Spawner.
    /// </summary>
    void LoadGame()
    {
        if (Multiplayer.IsServer())
        {
            CallDeferred("LoadLevel");
        }
    }

    /// <summary>
    /// First part of client-side loading
    /// </summary>
    void LoadLevel()
    {
        //Loading screen.
        GetTree().Root.GetNode<CanvasLayer>("Main/LoadingScreen/").Visible = true;

        ResourceLoader.LoadThreadedRequest("res://Scenes/Facility.tscn");
        loading = true;
    }
    /// <summary>
    /// Second part of client-side loading.
    /// </summary>
    /// <param name="scene">Scene to load</param>
    void PrepareLevel(PackedScene scene)
    {
        if (!Multiplayer.IsServer())
        {
            RpcId(1, "CheckIfBanned", Multiplayer.GetUniqueId());
        }
        if (GetNodeOrNull("Game") != null)
        {
            foreach (Node n in GetNode("Game").GetChildren())
            {
                GetNode("Game").RemoveChild(n);
                n.QueueFree();
            }
        }
        AddChild(scene.Instantiate());
        //Loading screen.
        GetTree().Root.GetNode<CanvasLayer>("Main/LoadingScreen/").Visible = false;
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
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Hide();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    /// <summary>
    /// Emitted when server is disconnected.
    /// </summary>
    internal void ServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        peer.Close();
        if (GetNodeOrNull("Game") != null)
        {
            GetNode("Game").QueueFree();
        }
        loading = false;
        GD.Print("You are disconnected from the server.");
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/MainMenu").Show();
        GetTree().Root.GetNode<Control>("Main/CanvasLayer/PauseMenu").Hide();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    /// <summary>
    /// Gets player's IP-address. PLEASE, CHECK CONSOLE FOR UNAUTHORIZED ACCESS
    /// </summary>
    /// <param name="id">Peer id</param>
    /// <returns>Player's IP address</returns>
    internal string GetPeer(int id)
    {
        GD.Print("SECURITY WARNING!!! SOMEONE GOT IP-ADDRESS");
        return peer.GetPeer(id).GetRemoteAddress();
    }
    /// <summary>
    /// Kicks a player. PLEASE, CHECK CONSOLE FOR UNAUTHORIZED ACCESS
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void Kick()
    {
        GD.Print("SECURITY WARNING!!! SOMEONE TRIED TO KICK A PLAYER");
        ServerDisconnected();
    }
    /// <summary>
    /// Server-side method, that checks if this IP banned
    /// </summary>
    /// <param name="id">Player's peer</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void CheckIfBanned(int id)
    {
        if (TxtParser.Load("user://ipbans.txt").Contains(GetPeer(id)))
        {
            RpcId(id, "Kick");
        }
    }
}
