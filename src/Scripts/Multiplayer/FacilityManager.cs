using Godot;
using System;
using System.ComponentModel.Design;
using System.Formats.Asn1;

public partial class FacilityManager : Node3D
{
    // The facility manager not only manages the facility - it is a player class manager too!

    //graphics settings field
    Settings settings;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    WorldEnvironment env;

    //player class manager data
    PlayerScript playerScene;
    [Export] Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();
    [Export] bool isRoundStarted = false;
    Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classes;
    Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> rooms;
    Godot.Collections.Dictionary<string, string> items;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        settings = GetTree().Root.GetNode<Settings>("Settings");
        env = GetNode<WorldEnvironment>("WorldEnvironment");

        if (settings.MusicSetting)
        {
            AudioStreamPlayer sfx = GetNode<AudioStreamPlayer>("BackgroundMusic");
            sfx.Playing = true;
        }

        env.Environment.SdfgiEnabled = settings.SdfgiSetting;
        env.Environment.SsaoEnabled = settings.SsaoSetting;
        env.Environment.SsilEnabled = settings.SsilSetting;
        env.Environment.SsrEnabled = settings.SsrSetting;
        env.Environment.VolumetricFogEnabled = settings.FogSetting;

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


        // For support custom classes.
        if (Multiplayer.IsServer())
        {
            if (FileAccess.FileExists("user://version.txt"))
            {
                if (TxtParser.Load("user://version.txt") == Globals.milestone)
                {
                    rooms = RoomParser.ReadJson("user://rooms.json");
                    classes = ClassParser.ReadJson("user://playerclasses.json");
                    items = ItemParser.ReadJson("user://itemlist.json");
                }
                else
                {
                    TxtParser.Save("user://version.txt", Globals.milestone);
                    RoomParser.SaveJson("user://rooms.json", Globals.roomData);
                    ClassParser.SaveJson("user://playerclasses.json", Globals.classData);
                    ItemParser.SaveJson("user://itemlist.json", items);
                }
            }
            else
            {
                TxtParser.Save("user://version.txt", Globals.milestone);
                RoomParser.SaveJson("user://rooms.json", Globals.roomData);
                ClassParser.SaveJson("user://playerclasses.json", Globals.classData);
                ItemParser.SaveJson("user://itemlist.json", items);
            }
        }
        else
        {
            RoomParser.SaveJson("user://rooms.json", rooms);
            ClassParser.SaveJson("user://playerclasses.json", classes);
            ItemParser.SaveJson("user://itemlist.json", items);
        }

        //Start round
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
        playerScene = (PlayerScript)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Name = id.ToString();
        AddChild(playerScene, true);
        playersList.Add(playerScene.Name);
        if (isRoundStarted)
        {
            PostRoundStart(playersList, id);
        }
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
            playersList.Remove(id.ToString());
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
        RespawnMTF();
    }

    /// <summary>
    /// Round start. Adds the players in the list and tosses their classes.
    /// </summary>
    void BeginGame()
    {
        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
        uint i = 1;
        foreach (Node player in players)
        {
            if (player is PlayerScript playerScript)
            {
                Rpc("SetPlayerClass", playerScript.Name, TossPlayerClass(i));
                i++;
            }
        }
        isRoundStarted = true;
    }
    /// <summary>
    /// Respawns MTF e-11 "Nine-Tailed Fox" every 5 minutes.
    /// </summary>
    async void RespawnMTF()
    {
        await ToSignal(GetTree().CreateTimer(300.0), "timeout");
        foreach (string item in playersList)
        {
            PlayerScript player = GetNode<PlayerScript>(item);
            if (player.classKey == "spectator")
            {
                Rpc("SetPlayerClass", item, "mtfe11");
            }
        }
        RespawnMTF();
    }

    /// <summary>
    /// Sets player class through the RPC.
    /// </summary>
    /// <param name="playerName">Player ID, contained in name</param>
    /// <param name="nameOfClass">Class, that will be granted</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    internal void SetPlayerClass(string playerName, string nameOfClass) //Note: RPC ONLY HANDLES PRIMITIVE TYPES, NOT PLAYERSCRIPT!
    {
        BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + nameOfClass + ".tres");
        GetNode<PlayerScript>(playerName).classKey = nameOfClass;
        GetNode<PlayerScript>(playerName).className = classData.ClassName;
        GetNode<PlayerScript>(playerName).scpNumber = classData.ScpNumber;
        GetNode<PlayerScript>(playerName).sprintEnabled = classData.SprintEnabled;
        GetNode<PlayerScript>(playerName).moveSoundsEnabled = classData.MoveSoundsEnabled;
        GetNode<PlayerScript>(playerName).footstepSounds = classData.FootstepSounds;
        GetNode<PlayerScript>(playerName).sprintSounds = classData.SprintSounds;
        GetNode<PlayerScript>(playerName).speed = classData.Speed;
        GetNode<PlayerScript>(playerName).jump = classData.Jump;
        GetNode<PlayerScript>(playerName).health = classData.Health;
        GetNode<PlayerScript>(playerName).team = classData.Team;
        if (Multiplayer.IsServer())
        {
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "CameraManager", !classData.CustomCamera);
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "ApplyPlayerHeadPosition", classData.DefaultCameraPos);
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "ApplyShader", classData.CustomView);
        }
        //Currently, ragdolls are unstable. (Or give me a sign, that they are working). So these "ragdolls" are just death animations.
        GetNode<PlayerScript>(playerName).ragdollSource = classData.PlayerRagdollSource;

        
        RpcId(int.Parse(playerName), "PreloadInventory", playerName, classData.PreloadedItems);
        // PreloadInventory(playerName, classData.PreloadedItems);
        RpcId(int.Parse(playerName), "UpdateClassUI", GetNode<PlayerScript>(playerName).className, GetNode<PlayerScript>(playerName).health);
        LoadModels(playerName);
        if (GetTree().Root.GetNodeOrNull(classData.SpawnPoints[rng.RandiRange(0, classData.SpawnPoints.Length - 1)]) != null) //SCP CB Multiplayer moment (:
        {
            GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>(classData.SpawnPoints[rng.RandiRange(0, classData.SpawnPoints.Length - 1)]).GlobalPosition;
        }
        else //if simpler, if there is no spawnroom this round - force spawn in HCZ testroom.
        {
            GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>("Main/Game/MapGenHcz/HC_cont2_testroom/entityspawn").GlobalPosition;
        }
    }
    /// <summary>
    /// Recall player classes for player, which got connected to ongoing round.
    /// </summary>
    /// <param name="players">Player list to process</param>
    /// <param name="target">A player connected mid-round</param>
    void PostRoundStart(Godot.Collections.Array<string> players, long target)
    {
        Rpc("SetPlayerClass", playerScene.Name, "spectator");
        foreach (string playerName in players)
        {
            RpcId(target, "SetPlayerClass", playerName, GetNode<PlayerScript>(playerName).classKey);
        }
        Rpc("CleanRagdolls");
    }
    /// <summary>
    /// Loads the models of a player.
    /// </summary>
    /// <param name="playerName">Name of a player</param>
    void LoadModels(string playerName)
    {
        PlayerScript playerScript = GetNode<PlayerScript>(playerName);
        //if class exists, then despawn old model, and change it on a new one.
        if (ResourceLoader.Exists("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres"))
        {
            Node modelRoot = playerScript.GetNode("PlayerModel");
            foreach (Node itemUsedBefore in modelRoot.GetChildren())
            {
                itemUsedBefore.QueueFree();
            }
            BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres");
            Node tmpModel = ResourceLoader.Load<PackedScene>(classData.PlayerModelSource).Instantiate();
            modelRoot.AddChild(tmpModel, true);
        }
    }
    /*
    /// <summary>
    /// Loads the models of players. Note, that you must use ISMultiplayerAuthority() before calling, because of duplicated player model problem.
    /// Deprecated in 0.6.0-dev
    /// </summary>
    /// <param name="players">Player list, where each player is getting the model.</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void LoadModels(Godot.Collections.Array<string> players)
    {
        foreach (string playerName in players)
        {
            PlayerScript playerScript = GetNode<PlayerScript>(playerName);
            //if class exists, then despawn old model, and change it on a new one.
            if (ResourceLoader.Exists("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres")) 
            {
                Node modelRoot = playerScript.GetNode("PlayerModel");
                foreach (Node itemUsedBefore in modelRoot.GetChildren())
                {
                    itemUsedBefore.QueueFree();
                }
                BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres");
                Node tmpModel = ResourceLoader.Load<PackedScene>(classData.PlayerModelSource).Instantiate();
                modelRoot.AddChild(tmpModel, true);
            }
        }
    }*/

    /// <summary>
    /// Helper method to preload inventory while changing class
    /// </summary>
    /// <param name="playerName">Name of the player</param>
    /// <param name="itemsArray">Preloaded items</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void PreloadInventory(string playerName, string[] itemsArray)
    {
        for (int i = 0; i < GetNode<Inventory>(playerName + "/InventoryContainer/Inventory").items.Count; i++)
        {
            GetNode<Inventory>(playerName + "/InventoryContainer/Inventory").items[i] = null;
        }
        foreach (string item in itemsArray)
        {
            GetNode<Inventory>(playerName + "/InventoryContainer/Inventory").AddItem(ResourceLoader.Load(item));
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

    /// <summary>
    /// Tosses player classes at round start.
    /// </summary>
    /// <param name="i">Counter</param>
    /// <returns>A random class</returns>
    string TossPlayerClass(uint i)
    {
        uint scpLimit = 3; //SCP Limit
        Godot.Collections.Array<int> usedScps = new Godot.Collections.Array<int>(); //Already spawned SCPs
        if (i == 2 || i % 8 == 0)
        {
            if (usedScps.Count > scpLimit) //if there are more SCPs than exist - spawn a human instead.
            {
                return classes["spawnableHuman"][rng.RandiRange(0, classes["spawnableHuman"].Count - 1)];
            }
            int randomScpClass = rng.RandiRange(0, classes["spawnableScps"].Count - 1);
            while (usedScps.Contains(randomScpClass))
            {
                randomScpClass = rng.RandiRange(0, classes["spawnableScps"].Count - 1);
            } //Spawn SCP
            usedScps.Add(randomScpClass);
            return classes["spawnableScps"][randomScpClass];
        }
        else //Spawn a human.
        {
            return classes["spawnableHuman"][rng.RandiRange(0, classes["spawnableHuman"].Count - 1)];
        }
    }
    /// <summary>
    /// Spawns player ragdoll.
    /// </summary>
    /// <param name="player">Player name</param>
    /// <param name="ragdollSrc">source of ragdoll (meaned by class)</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnRagdoll(string player, string ragdollSrc)
    {
        Node3D ragdoll = GD.Load<PackedScene>(ragdollSrc).Instantiate<Node3D>();
        ragdoll.Position = GetNode<PlayerScript>(player).GlobalPosition;
        GetNode("Ragdolls").AddChild(ragdoll);
    }
    /// <summary>
    /// Cleans ragdolls (used when a new player connects)
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void CleanRagdolls()
    {
        foreach (Node node in GetNode("Ragdolls").GetChildren())
        {
            node.QueueFree();
        }
    }

    /// <summary>
    /// Specific usage. Is only used by SCP-650 and SCP-106 to teleport to another player.
    /// </summary>
    /// <param name="playerName">Name of the teleporting player</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void RandomTeleport(string playerName)
    {
        GetNode<PlayerScript>(playerName).Position = GetNode<PlayerScript>(playersList[rng.RandiRange(0, playersList.Count - 1)]).GlobalPosition;
    }

    /// <summary>
    /// Teleport-to-room method. Used for administration and for testing.
    /// </summary>
    /// <param name="playerToTeleport">Which player will be teleported</param>
    /// <param name="to">Destination</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void TeleportTo(string playerToTeleport, string to)
    {
        GetNode<PlayerScript>(playerToTeleport).Position = GetTree().Root.GetNode<Marker3D>(to).GlobalPosition;
    }
}
