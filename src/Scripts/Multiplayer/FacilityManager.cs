using Godot;
using System;
/// <summary>
/// Facility manager is a gamemode, player class and players manager.
/// </summary>
public partial class FacilityManager : Node3D
{
    internal string localNickname;
    //graphics settings field
    Settings settings;
    internal RandomNumberGenerator rng = new RandomNumberGenerator();
    WorldEnvironment env;

    //player class manager data
    PlayerScript playerScene;
    [Export] internal Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();
    [Export] bool isRoundStarted = false;
    /*
    [Export] Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classes = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
    [Export] Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> rooms = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
    [Export] Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>();
    [Export] Godot.Collections.Dictionary<string, string> ammo = new Godot.Collections.Dictionary<string, string>();
    [Export] Godot.Collections.Dictionary<string, string> npcs = new Godot.Collections.Dictionary<string, string>();
    */
    [Export] internal GameData data;
    internal Godot.Collections.Array<int> singleSpawnClasses = new Godot.Collections.Array<int>();
    internal Godot.Collections.Array<int> multiSpawnClasses = new Godot.Collections.Array<int>();
    internal Godot.Collections.Array<int> arrivingClasses = new Godot.Collections.Array<int>();
    internal Godot.Collections.Array<int> specialClasses = new Godot.Collections.Array<int>();
    internal Godot.Collections.Array<int> bonusSpawnClasses = new Godot.Collections.Array<int>(); //For 131.
    [Export] bool friendlyFireFm;
    [Export] internal int maxSpawnableObjects;
    bool trueBreach;
    internal bool IsRoundStarted
    { 
        get=>isRoundStarted; set=>isRoundStarted = value;
    }
    internal bool FriendlyFire
    {
        get => friendlyFireFm; private set => friendlyFireFm = value;
    }

    
    [Export] internal int targets = 0;
    [Export] internal int[] tickets = new int[] { 0, 0, 0 };

    string currentAmbient = string.Empty;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Root.GetNode<CanvasLayer>("Main/LoadingScreen/").Visible = false;
        settings = GetTree().Root.GetNode<Settings>("Settings");
        env = GetNode<WorldEnvironment>("WorldEnvironment");

        env.Environment.SdfgiEnabled = settings.SdfgiSetting;
        env.Environment.SsaoEnabled = settings.SsaoSetting;
        env.Environment.SsilEnabled = settings.SsilSetting;
        env.Environment.SsrEnabled = settings.SsrSetting;
        env.Environment.VolumetricFogEnabled = settings.FogSetting;
        env.Environment.FogEnabled = !settings.FogSetting;

        // Multiplayer part.
        // For custom classes, rooms, items support.
        if (Multiplayer.IsServer())
        {
            /* old 0.7.x code
            rooms = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json");

            classes = ClassParser.ReadJson("user://playerclasses_" + Globals.classesCompatibility + ".json");

            items = ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", Globals.ItemType.item);

            ammo = ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", Globals.ItemType.ammo);

            npcs = ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", Globals.ItemType.npc);

            maxSpawnableObjects = GetParent<NetworkManager>().maxObjects;
            scpLimit = GetParent<NetworkManager>().maxScps;
            */
            /*
            if (ResourceLoader.Exists("user://save_" + Globals.gameDataCompatibility + ".tres"))
            {
                data = ResourceLoader.Load<GameData>("user://save_" + Globals.gameDataCompatibility + ".tres");
            }
            else
            {
                data = ResourceLoader.Load<GameData>("res://Scripts/GameData/GameData.tres");
                ResourceSaver.Save(data, "user://save_" + Globals.gameDataCompatibility + ".tres");
            }
            */
            ArrangeClasses();
        }
        else
        {
            /* old 0.7.x code
            if (GD.Hash(RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")) != GD.Hash(rooms) && rooms.Count > 0)
            {
                RoomParser.SaveJson("user://rooms_" + Globals.roomsCompatibility + ".json", rooms);
            }
            if (GD.Hash(ClassParser.ReadJson("user://playerclasses_" + Globals.classesCompatibility + ".json")) != GD.Hash(classes) && classes.Count > 0)
            {
                ClassParser.SaveJson("user://playerclasses_" + Globals.classesCompatibility + ".json", classes);
            }
            if (GD.Hash(ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", Globals.ItemType.item)) != GD.Hash(items) && items.Count > 0)
            {
                ItemParser.SaveJson("user://itemlist_" + Globals.itemsCompatibility + ".json", items);
            }
            if (GD.Hash(ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", Globals.ItemType.ammo)) != GD.Hash(ammo) && ammo.Count > 0)
            {
                ItemParser.SaveJson("user://ammotype_" + Globals.itemsCompatibility + ".json", ammo);
            }
            if (GD.Hash(ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", Globals.ItemType.npc)) != GD.Hash(npcs) && npcs.Count > 0)
            {
                ItemParser.SaveJson("user://npcs_" + Globals.itemsCompatibility + ".json", npcs);
            }
            */
            //ResourceSaver.Save(data, "user://save_" + Globals.gameDataCompatibility + ".tres");
            return;
        }
        friendlyFireFm = GetParent<NetworkManager>().friendlyFire;
        trueBreach = false; //NetworkManager.tBrSim;

        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;

        foreach (int id in Multiplayer.GetPeers())
        {
            AddPlayer(id);
        }
        AddPlayer(1);

        OnServerStart();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        OnUpdate(delta);
    }

    internal virtual void OnServerStart()
    {
    }

    internal virtual void OnUpdate(double delta)
    {
    }
    /// <summary>
    /// Arranges classes by it's type. Available since 0.8.0-dev.
    /// </summary>
    void ArrangeClasses()
    {
        for (int i = 0; i < data.Classes.Count; i++)
        {
            switch (data.Classes[i].ClassType)
            {
                case Globals.ClassType.singleSpawnClasses:
                    singleSpawnClasses.Add(i);
                    break;
                case Globals.ClassType.multiSpawnClasses:
                    multiSpawnClasses.Add(i);
                    break;
                case Globals.ClassType.arrivingClasses:
                    arrivingClasses.Add(i);
                    break;
                case Globals.ClassType.specialClasses:
                    specialClasses.Add(i);
                    break;
                case Globals.ClassType.bonusSpawnClasses:
                    bonusSpawnClasses.Add(i);
                    break;
                default:
                    GD.Print("Failed to set up classes. Some functions may work unexpectedly");
                    break;
            }
        }
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
    /// Sets player class through the RPC.
    /// </summary>
    /// <param name="playerName">Player ID, contained in name</param>
    /// <param name="nameOfClass">Class, that will be granted</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    internal void SetPlayerClass(string playerName, int nameOfClass, string reason) //Note: RPC ONLY HANDLES PRIMITIVE TYPES, NOT PLAYERSCRIPT!
    {
        if (nameOfClass < 0 || nameOfClass >= data.Classes.Count)
        {
            GD.PrintErr("For security reasons, you cannot change to class, that is unsupported by this server");
            return;
        }
        //BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + nameOfClass + ".tres");
        BaseClass classData = data.Classes[nameOfClass];

        //tickets and targets
        if (classData.Team == Globals.Team.DSE)
        {
            tickets[0]++;
        }
        else if (GetNode<PlayerScript>(playerName).team == Globals.Team.DSE && classData.Team != Globals.Team.DSE)
        {
            tickets[0]--;
        }
        if (classData.ScpNumber == -1)
        {
            targets++;
        }
        else if (GetNode<PlayerScript>(playerName).scpNumber == -1 && classData.ScpNumber != -1)
        {
            targets--;
        }

        //Applying properties to a player.
        GetNode<PlayerScript>(playerName).classKey = nameOfClass;
        GetNode<PlayerScript>(playerName).className = classData.ClassName;
        GetNode<PlayerScript>(playerName).classDescription = classData.ClassDescription;
        GetNode<PlayerScript>(playerName).scpNumber = classData.ScpNumber;
        GetNode<PlayerScript>(playerName).sprintEnabled = classData.SprintEnabled;
        GetNode<PlayerScript>(playerName).moveSoundsEnabled = classData.MoveSoundsEnabled;
        GetNode<PlayerScript>(playerName).footstepSounds = classData.FootstepSounds;
        GetNode<PlayerScript>(playerName).sprintSounds = classData.SprintSounds;
        GetNode<PlayerScript>(playerName).speed = classData.Speed;
        GetNode<PlayerScript>(playerName).jump = classData.Jump;
        GetNode<PlayerScript>(playerName).health[0] = classData.Health;
        GetNode<PlayerScript>(playerName).health[1] = classData.Sanity;
        GetNode<PlayerScript>(playerName).currentHealth[0] = classData.Health;
        GetNode<PlayerScript>(playerName).currentHealth[1] = classData.Sanity;
        GetNode<PlayerScript>(playerName).team = classData.Team;

        GetNode<AmmoSystem>(playerName + "/AmmoSystem").ammo = classData.Ammo;
        //Server calls
        if (Multiplayer.IsServer())
        {
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "CameraManager", !classData.CustomCamera);
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "UpdateClassUI", classData.ClassColor.ToRgba32());
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "ApplyPlayerHeadPosition", classData.DefaultCameraPos);
            GetNode<PlayerScript>(playerName).RpcId(int.Parse(playerName), "ApplyShader", classData.CustomView);
            RpcId(int.Parse(playerName), "PreloadInventory", playerName, classData.PreloadedItems);
        }
        GetNode<PlayerScript>(playerName).ragdollSource = classData.PlayerRagdollSource;

        // PreloadInventory(playerName, classData.PreloadedItems);
        // RpcId(int.Parse(playerName), "UpdateClassUI", GetNode<PlayerScript>(playerName).className, GetNode<PlayerScript>(playerName).health);
        LoadModels(playerName, nameOfClass);
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
    internal virtual void PostRoundStart(Godot.Collections.Array<string> players, long target)
    {
        Rpc("SetPlayerClass", playerScene.Name, 0, "Post-roundstart arriving.");
        foreach (string playerName in players)
        {
            if (playerName != playerScene.Name)
            {
                RpcId(target, "SetPlayerClass", playerName, GetNode<PlayerScript>(playerName).classKey, "Previous player");
            }
        }
        Rpc("CleanRagdolls");
    }
    /*
    /// <summary>
    /// Loads the models of a player.
    /// </summary>
    /// <param name="playerName">Name of a player</param>
    /// */
    

    void LoadModels(string playerName, int classId)
    {
        PlayerScript playerScript = GetNode<PlayerScript>(playerName);
        //if class exists, then despawn old model, and change it on a new one.
        if (classId < data.Classes.Count && classId >= 0)
        {
            Node modelRoot = playerScript.GetNode("PlayerModel");
            foreach (Node itemUsedBefore in modelRoot.GetChildren())
            {
                itemUsedBefore.QueueFree();
            }
            Node tmpModel = ResourceLoader.Load<PackedScene>(data.Classes[classId].PlayerModelSource).Instantiate();
            modelRoot.AddChild(tmpModel, true);
        }
    }
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
            GetNode<Inventory>(playerName + "/InventoryContainer/Inventory").RemoveItem(i, false);
        }
        foreach (string item in itemsArray)
        {
            GetNode<Inventory>(playerName + "/InventoryContainer/Inventory").AddItem(ResourceLoader.Load(item));
        }
    }
    /// <summary>
    /// Spawns player ragdoll. Will be moved to PlayerAction in later versions.
    /// </summary>
    /// <param name="player">Player name</param>
    /// <param name="ragdollSrc">Source of ragdoll (meaned by class)</param>
    /// <param name="isNotDead">Is it contained (true) or dead (false)</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void SpawnRagdoll(string player, string ragdollSrc, bool isNotDead)
    {
        GameOver ragdoll = GD.Load<PackedScene>(ragdollSrc).Instantiate<GameOver>();
        ragdoll.Position = GetNode<PlayerScript>(player).GlobalPosition;
        ragdoll.isContainable = isNotDead;
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
    /// Teleports object to player. Currently used by SCP-650 NPC.
    /// </summary>
    /// <param name="from">Object to teleport</param>
    /// <param name="destination">Bring the object to player</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void TeleportToPlayer(string from, string destination)
    {
        switch (destination)
        {
            case "random":
                GetNode<Node3D>(from).Position = GetNode<PlayerScript>(playersList[rng.RandiRange(0, playersList.Count - 1)]).GlobalPosition;
                break;
            default:
                GetNode<Node3D>(from).Position = GetNode<PlayerScript>(destination).GlobalPosition;
                break;
        }
    }

    /// <summary>
    /// Teleport-to-room method. Used for administration and for testing. Will be moved in separate script in future.
    /// </summary>
    /// <param name="playerToTeleport">Which player will be teleported</param>
    /// <param name="to">Destination</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void TeleportTo(string playerToTeleport, string to)
    {
        GetNode<PlayerScript>(playerToTeleport).Position = GetTree().Root.GetNode<Marker3D>(to).GlobalPosition;
    }
    /// <summary>
    /// Sets background music. Before 0.6.0, there was only Light Containment music all over facility.
    /// </summary>
    /// <param name="set"></param>
    internal void SetBackgroundMusic(string set)
    {
        if (currentAmbient != set)
        {
            AudioStreamPlayer sfx = GetNode<AudioStreamPlayer>("BackgroundMusic");
            AudioStream audio = ResourceLoader.Load<AudioStream>(set);
            sfx.Stream = audio;
            sfx.Playing = true;
            currentAmbient = set;
        }
    }
}
