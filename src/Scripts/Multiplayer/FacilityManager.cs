using Godot;
using System;

public partial class FacilityManager : Node3D
{
    // The facility manager not only manages the facility - it is a player class manager too!

    //graphics settings field
    WorldEnvironment graphics = new WorldEnvironment();
	RandomNumberGenerator rng = new RandomNumberGenerator();

    //player class manager data
	CharacterBody3D playerScene;
    [Export] Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        graphics = GetNode<WorldEnvironment>("WorldEnvironment");

        if (SettingsPanel.MusicSetting)
        {
            AudioStreamPlayer sfx = GetNode<AudioStreamPlayer>("BackgroundMusic");
            sfx.Playing = true;
        }

        if (SettingsPanel.HQSetting)
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingHighQuality.tres");
        }
        else
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingLowQuality.tres");
        }


        //Multiplayer part.
        if (!Multiplayer.IsServer())
        {
            return;
        }
        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;

        foreach (int id in Multiplayer.GetPeers())
        {
            AddPlayer(id);
        }
        AddPlayer(1);
        WaitForBeginning();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    /// <summary>
    /// Adds player to server.
    /// </summary>
    /// <param name="id">Unique multiplayer ID</param>
    void AddPlayer(long id)
    {
        playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Name = id.ToString();
        AddChild(playerScene, true);
        GD.Print("Player " + id.ToString() + " has joined the server!");
    }

    /// <summary>
    /// Removes the player from server
    /// </summary>
    /// <param name="id">Unique multiplayer ID</param>
    void RemovePlayer(long id)
    {
        if (GetNodeOrNull(id.ToString()) != null)
        {
            GetNode(id.ToString()).QueueFree();
            GD.Print("Player " + id.ToString() + " has left the server!");
        }
    }

    /// <summary>
    /// Waits for people gather before the round starts.
    /// </summary>
    async void WaitForBeginning()
    {
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        BeginGame();
    }

    /// <summary>
    /// Round start. Adds the players in the list and tosses their classes.
    /// </summary>
    void BeginGame()
    {
        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
        foreach (Node player in players)
        {
            if (player is PlayerScript playerScript)
            {
                playersList.Add(playerScript.Name);
                Rpc("SetPlayerClass", playerScript.Name, "default");
            }
        }
    }

    /// <summary>
    /// Sets player class through the RPC.
    /// </summary>
    /// <param name="playerName">Player ID, contained in name</param>
    /// <param name="nameOfClass">Class, that will be granted</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    internal void SetPlayerClass(string playerName, string nameOfClass) //Note: RPC ONLY HANDLES PRIMITIVE TYPES, NOT PLAYERSCRIPT!
    {
        GetNode<PlayerScript>(playerName).classKey = nameOfClass;
        GetNode<PlayerScript>(playerName).className = (string)ClassData.playerClasses[nameOfClass]["className"];
        GetNode<PlayerScript>(playerName).scpNumber = (int)ClassData.playerClasses[nameOfClass]["scpNumber"];
        GetNode<PlayerScript>(playerName).sprintEnabled = (bool)ClassData.playerClasses[nameOfClass]["sprintEnabled"];
        GetNode<PlayerScript>(playerName).moveSoundsEnabled = (bool)ClassData.playerClasses[nameOfClass]["moveSoundsEnabled"];
        GetNode<PlayerScript>(playerName).footstepSounds = (Godot.Collections.Array<string>)ClassData.playerClasses[nameOfClass]["footstepSounds"];
        GetNode<PlayerScript>(playerName).sprintSounds = (Godot.Collections.Array<string>)ClassData.playerClasses[nameOfClass]["sprintSounds"];
        GetNode<PlayerScript>(playerName).speed = (float)ClassData.playerClasses[nameOfClass]["speed"];
        GetNode<PlayerScript>(playerName).jump = (float)ClassData.playerClasses[nameOfClass]["jump"];
        GetNode<PlayerScript>(playerName).health = (float)ClassData.playerClasses[nameOfClass]["health"];
        GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>((string)ClassData.playerClasses[nameOfClass]["spawnPoint"]).GlobalPosition;
        RpcId(int.Parse(playerName), "UpdateClassUI", GetNode<PlayerScript>(playerName).className, GetNode<PlayerScript>(playerName).health);
        if (IsMultiplayerAuthority()) //YESSS!!! IsMultiplayerAuthority() is NECESSARY for NOT duplicating player models!
        {
            Rpc("LoadModels", playersList);
        }
    }

    /// <summary>
    /// Loads the models of players. Note, that you must use ISMultiplayerAuthority() before calling, because of duplicated player model problem.
    /// </summary>
    /// <param name="players">Player list, where each player is getting the model.</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void LoadModels(Godot.Collections.Array<string> players)
    {
        foreach (string playerName in players)
        {
            PlayerScript playerScript = GetNode<PlayerScript>(playerName);
            if (ClassData.playerClasses.Keys.Contains(playerScript.classKey))
            {
                Node ModelRoot = playerScript.GetNode("PlayerModel");
                if (ModelRoot.GetChild(0) != null)
                {
                    ModelRoot.GetChild(0).QueueFree();
                }
                Node tmpModel = ResourceLoader.Load<PackedScene>((string)ClassData.playerClasses[playerScript.classKey]["playerModelSource"]).Instantiate();
                ModelRoot.AddChild(tmpModel, true);
            }
        }
    }

    /// <summary>
    /// Update the class UI, when forceclassing.
    /// </summary>
    /// <param name="className">Name of the class</param>
    /// <param name="health">Health of the class</param>

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void UpdateClassUI(string className, float health)
    {
        GetNode<Label>("PlayerUI/ClassInfo").Text = className;
        GetNode<Label>("PlayerUI/HealthInfo").Text = Mathf.Ceil(health).ToString();
    }

    string TossPlayerClass() //to be reworked.
    {
        Godot.Collections.Array<string> tossClass = new Godot.Collections.Array<string>();
        foreach(string val in ClassData.playerClasses.Keys)
        {
            tossClass.Add(val);
        }
        rng.Randomize();
        return tossClass[rng.RandiRange(1, tossClass.Count - 1)];
    }

    /// <summary>
    /// Specific usage. Is only used by SCP-650 (and SCP-106 in future) to teleport to another player.
    /// </summary>
    /// <param name="playerName">Name of the teleporting player</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void RandomTeleport(string playerName)
    {
        GetNode<PlayerScript>(playerName).Position = GetNode<PlayerScript>(playersList[rng.RandiRange(0, playersList.Count - 1)]).GlobalPosition;
    }
}
