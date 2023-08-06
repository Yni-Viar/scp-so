using Godot;
using System;

public partial class FacilityManager : Node3D
{
    // The facility manager not only manages the facility - it is a player class manager too!

    //graphics settings field
    WorldEnvironment graphics = new WorldEnvironment();
	RandomNumberGenerator rng = new RandomNumberGenerator();
	CharacterBody3D playerScene;
    Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();

	// string[] spawnLocation = new string[] {"MapGenLCZ/LC_room1_archive/entityspawn", "MapGenRZ/RZ_room2_offices/entityspawn", "MapGenHCZ/HC_cont1_173/entityspawn"};

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

    void AddPlayer(long id)
    {
        playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Name = id.ToString();
        AddChild(playerScene, true);
        GD.Print("Player " + id.ToString() + " has joined the server!");
    }

    void RemovePlayer(long id)
    {
        if (GetNodeOrNull(id.ToString()) != null)
        {
            GetNode(id.ToString()).QueueFree();
            GD.Print("Player " + id.ToString() + " has left the server!");
        }
    }

    async void WaitForBeginning()
    {
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        BeginGame();
    }

    void BeginGame()
    {
        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
        foreach (Node player in players)
        {
            if (player is PlayerScript playerScript)
            {
                playersList.Add(playerScript.Name);
                //playerScript.RpcId(int.Parse(player.Name), "SetPlayerClass", "default");
                //playerScript.Rpc("GetPlayerModel");
                Rpc("SetPlayerClass", playerScript.Name, TossPlayerClass());
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    internal void SetPlayerClass(string playerName, string nameOfClass) //Note: RPC ONLY HANDLES PRIMITIVE TYPES, NOT PLAYERSCRIPT!
    {
        GetNode<PlayerScript>(playerName).classKey = nameOfClass;
        GetNode<PlayerScript>(playerName).className = (string)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["className"];
        GetNode<PlayerScript>(playerName).scpNumber = (int)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["scpNumber"];
        GetNode<PlayerScript>(playerName).footstepSounds = (Godot.Collections.Array<string>)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["footstepSounds"];
        GetNode<PlayerScript>(playerName).speed = (float)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["speed"];
        GetNode<PlayerScript>(playerName).jump = (float)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["jump"];
        GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>((string)ClassParser.ReadJson("user://playerclass_0.3.0.json")[nameOfClass]["spawnPoint"]).GlobalPosition;
        Rpc("LoadModels", playersList);
    }

    /*internal void ForceClass(string playerName, string nameOfClass)
    {
        Rpc("SetPlayerClass", playerName, nameOfClass);
        Rpc("LoadModels", playersList);
    }*/

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void LoadModels(Godot.Collections.Array<string> players)
    {
        foreach (string playerName in players)
        {
            PlayerScript playerScript = GetNode<PlayerScript>(playerName);
            if (ClassParser.ReadJson("user://playerclass_0.3.0.json").Keys.Contains(playerScript.classKey))
            {
                Node ModelRoot = playerScript.GetNode("PlayerModel");
                if (ModelRoot.GetChild(0) != null)
                {
                    ModelRoot.GetChild(0).QueueFree();
                }
                Node tmpModel = ResourceLoader.Load<PackedScene>((string)ClassParser.ReadJson("user://playerclass_0.3.0.json")[playerScript.classKey]["playerModelSource"]).Instantiate();
                ModelRoot.AddChild(tmpModel, true);
            }
        }
    }

    string TossPlayerClass()
    {
        Godot.Collections.Array<string> tossClass = new Godot.Collections.Array<string>();
        foreach(string val in ClassParser.ReadJson("user://playerclass_0.3.0.json").Keys)
        {
            tossClass.Add(val);
        }
        rng.Randomize();
        return tossClass[rng.RandiRange(0, tossClass.Count - 1)];
    }
}
