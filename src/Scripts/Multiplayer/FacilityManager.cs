using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// The facility manager not only manages the facility - it is a player class manager too!
/// </summary>
public partial class FacilityManager : Node3D
{
    internal string localNickname;
    //graphics settings field
    Settings settings;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    WorldEnvironment env;

    //player class manager data
    PlayerScript playerScene;
    [Export] internal Godot.Collections.Array<string> playersList = new Godot.Collections.Array<string>();
    [Export] bool isRoundStarted = false;
    [Export] Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> classes = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
    [Export] Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> rooms = new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>>();
    [Export] Godot.Collections.Dictionary<string, string> items = new Godot.Collections.Dictionary<string, string>();
    [Export] Godot.Collections.Dictionary<string, string> ammo = new Godot.Collections.Dictionary<string, string>();
    [Export] Godot.Collections.Dictionary<string, string> npcs = new Godot.Collections.Dictionary<string, string>();
    [Export] bool friendlyFireFm;
    [Export] internal int maxSpawnableObjects;
    bool trueBreach;
    internal bool IsRoundStarted
    { 
        get=>isRoundStarted; private set=>isRoundStarted = value;
    }
    internal bool FriendlyFire
    {
        get => friendlyFireFm; private set => friendlyFireFm = value;
    }

    [Export] uint scpLimit = 4; //SCP Limit
    Godot.Collections.Array<int> usedScps = new Godot.Collections.Array<int>(); //Already spawned SCPs
    [Export] internal int targets = 0;
    [Export] internal int[] tickets = new int[] { 0, 0, 0 };

    string currentAmbient = string.Empty;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
            rooms = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json");

            classes = ClassParser.ReadJson("user://playerclasses_" + Globals.classesCompatibility + ".json");

            items = ItemParser.ReadJson("user://itemlist_" + Globals.itemsCompatibility + ".json", Globals.ItemType.item);

            ammo = ItemParser.ReadJson("user://ammotype_" + Globals.itemsCompatibility + ".json", Globals.ItemType.ammo);

            npcs = ItemParser.ReadJson("user://npcs_" + Globals.itemsCompatibility + ".json", Globals.ItemType.npc);

            maxSpawnableObjects = GetParent<NetworkManager>().maxObjects;
            scpLimit = GetParent<NetworkManager>().maxScps;

            if (GetParent<NetworkManager>().spawnNpcs)
            {
                string[] temp = new string[npcs.Keys.Count];
                npcs.Keys.CopyTo(temp, 0);
                //Spawn NPC with 67% chance
                if (rng.RandiRange(0, 100) <= 67)
                {
                    GetTree().Root.GetNode<PlayerAction>("Main/Game/PlayerAction").Rpc("SpawnObject", temp[rng.RandiRange(0, temp.Length - 1)], 2, 1);
                }
            }
        }
        else
        {
            friendlyFireFm = GetParent<NetworkManager>().friendlyFire;
            trueBreach = false; //NetworkManager.tBrSim;
            RoomParser.SaveJson("user://rooms_" + Globals.roomsCompatibility + ".json", rooms);
            ClassParser.SaveJson("user://playerclasses_" + Globals.classesCompatibility + ".json", classes);
            ItemParser.SaveJson("user://itemlist_" + Globals.itemsCompatibility + ".json", items);
            ItemParser.SaveJson("user://ammotype_" + Globals.itemsCompatibility + ".json", ammo);
            return;
        }
        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;

        foreach (int id in Multiplayer.GetPeers())
        {
            AddPlayer(id);
        }
        AddPlayer(1);

        //Start round
        WaitForBeginning();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Multiplayer.IsServer())
        {
            if (isRoundStarted)
            {
                RespawnMTF();
                CheckRoundEnd();
            }
        }
        if (!isRoundStarted)
        {
            GetNode<Label>("PreRoundStartPanel/PreRoundStart/Amount").Text = playersList.Count.ToString();
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
    /// Waits for people gather before the round starts.
    /// </summary>
    async void WaitForBeginning()
    {
        await ToSignal(GetTree().CreateTimer(25.0), "timeout");
        if (!IsRoundStarted)
        {
            BeginGame();
        }
    }

    /// <summary>
    /// Round start. Adds the players in the list and tosses their classes.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void BeginGame()
    {
        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Players");
        uint i = 1;
        isRoundStarted = true;
        foreach (Node player in players)
        {
            if (player is PlayerScript playerScript)
            {
                Rpc("SetPlayerClass", playerScript.Name, TossPlayerClass(i), "Round start.");
                i++;
            }
        }
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
                Rpc("SetPlayerClass", item, "mtfe11", "MTF arrive.");
            }
        }
    }

    /// <summary>
    /// Sets player class through the RPC.
    /// </summary>
    /// <param name="playerName">Player ID, contained in name</param>
    /// <param name="nameOfClass">Class, that will be granted</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    internal void SetPlayerClass(string playerName, string nameOfClass, string reason) //Note: RPC ONLY HANDLES PRIMITIVE TYPES, NOT PLAYERSCRIPT!
    {
        if (!CheckForExistence(nameOfClass))
        {
            GD.PrintErr("For security reasons, you cannot change to class, that is unsupported by this server");
            return;
        }
        BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + nameOfClass + ".tres");

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
        GetNode<PlayerScript>(playerName).health = classData.Health;
        GetNode<PlayerScript>(playerName).sanity = classData.Sanity;
        GetNode<PlayerScript>(playerName).currentHealth = classData.Health;
        GetNode<PlayerScript>(playerName).currentSanity = classData.Sanity;
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
    /// Check class if is existing on server
    /// </summary>
    /// <param name="nameOfClass">Class name</param>
    /// <returns>True if exist, otherwise false</returns>
    bool CheckForExistence(string nameOfClass)
    {
        foreach (string item in classes.Keys)
        {
            for (int i = 0; i < classes[item].Count; i++)
            {
                if (nameOfClass == classes[item][i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Recall player classes for player, which got connected to ongoing round.
    /// </summary>
    /// <param name="players">Player list to process</param>
    /// <param name="target">A player connected mid-round</param>
    void PostRoundStart(Godot.Collections.Array<string> players, long target)
    {
        Rpc("SetPlayerClass", playerScene.Name, "spectator", "Post-roundstart arriving.");
        foreach (string playerName in players)
        {
            if (playerName != playerScene.Name)
            {
                RpcId(target, "SetPlayerClass", playerName, GetNode<PlayerScript>(playerName).classKey, "Previous player");
            }
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
    /*
        /// <summary>
        /// Update the class UI, when forceclassing. Deprecated since 0.7.0-dev
        /// </summary>
        /// <param name="className">Name of the class</param>
        /// <param name="health">Health of the class</param>

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
        void UpdateClassUI(string className, float health)
        {
            GetNode<Label>("PlayerUI/ClassInfo").Text = className;
            GetNode<Label>("PlayerUI/HealthInfo").Text = Mathf.Ceil(health).ToString();
        }
    */

    /// <summary>
    /// Tosses player classes at round start. Will be reworked at next update.
    /// </summary>
    /// <param name="i">Counter</param>
    /// <returns>A random class</returns>
    string TossPlayerClass(uint i)
    {
        if (i == 2 && trueBreach) //Since 0.7.0, SCP-2522 is ALWAYS the second player.
        {
            int scp2522 = 0;
            usedScps.Add(scp2522);
            return classes["spawnableScps"][scp2522];
        }
        if (trueBreach ? (i % 8 == 4) : (i == 2 || i % 8 == 0)) // check method of SCP spawning, based on TrueBreach value.
        {
            if (usedScps.Count > scpLimit) //if there are more SCPs than exist - spawn a human instead.
            {
                return classes["spawnableHuman"][rng.RandiRange(0, classes["spawnableHuman"].Count - 1)];
            }
            int randomScpClass;
            if (usedScps.Count > 1 && rng.RandiRange(0, 7) < 2) //Spawn a friendly SCP by some chance.
            {
                randomScpClass = rng.RandiRange(0, classes["friendlyScps"].Count - 1);
                usedScps.Add(randomScpClass);
                return classes["friendlyScps"][randomScpClass];
            }
            randomScpClass = rng.RandiRange(0, classes["spawnableScps"].Count - 1);
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
    /// Round end checker.
    /// </summary>
    async void CheckRoundEnd()
    {
        await ToSignal(GetTree().CreateTimer(2.0), "timeout");
        if (playersList.Count > 1)
        {
            if (targets == 0 && tickets[0] > 0)
            {
                Rpc("RoundEnd", 0);
            }
            if (tickets[0] == 0)
            {
                //more will be added in future
                foreach (string item in playersList)
                {
                    switch (GetNode<PlayerScript>(item).team)
                    {
                        case Globals.Team.SCI:
                            tickets[1]++;
                            break;
                        case Globals.Team.CDP:
                            tickets[2]++;
                            break;
                        case Globals.Team.MTF:
                            tickets[1]++;
                            break;
                    }
                }
                if (tickets[2] == 0 && tickets[1] > 0)
                {
                    Rpc("RoundEnd", 1);
                }
                else if (tickets[2] > 0 && tickets[1] == 0)
                {
                    Rpc("RoundEnd", 2);
                }
                else
                {
                    Rpc("RoundEnd", 3);
                }
            }
        }
    }
    /// <summary>
    /// Round end scenario. After 15 seconds shutdowns the server.
    /// </summary>
    /// <param name="whoWon">Team, that won.</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    async void RoundEnd(int whoWon)
    {
        switch (whoWon)
        {
            case 0:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "SCPs won!\nThe server will be turned off soon...";
                break;
            case 1:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Foundation won!\nThe server will be turned off soon...";
                break;
            case 2:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Class-D won!\nThe server will be turned off soon...";
                break;
            case 3:
                GetNode<RichTextLabel>("PlayerUI/GameEnd").Text = "Stalemate!\nThe server will be turned off soon...";
                break;
            default:
                break;
        }
        GetNode<AnimationPlayer>("PlayerUI/AnimationPlayer").Play("roundend");
        SetProcess(false);
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        GetParent<NetworkManager>().ServerDisconnected();
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
    /// <summary>
    /// Asks for moderator privilegies.  Will be moved in separate script in future.
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <param name="password">Given password</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    void AskForModeratorPrivilegies(int id, string password)
    {
        if (password.GetHashCode() == GetTree().Root.GetNode<NetworkManager>("Main").GetModerator)
        {
            GetNode<PlayerScript>(id.ToString()).RpcId(id, "GrantModeratorPrivilegies");
        }
    }
    /// <summary>
    /// Forces round start. Available since 0.7.2
    /// </summary>
    void ForceRoundStart()
    {
        if (!IsRoundStarted)
        {
            IsRoundStarted = true;
            if (Multiplayer.IsServer())
            {
                BeginGame();
            }
            else
            {
                RpcId(1, "BeginGame");
            }
        }
    }
}
