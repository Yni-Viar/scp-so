using Godot;
using System;
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
			classes = ClassParser.ReadJson("user://playerclasses_0.5.0.json");
		}
		else
		{
			ClassParser.SaveJson("user://playerclasses_0.5.0.json", classes);
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
            Rpc("SetPlayerClass", playerScene.Name, "spectator");
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
		RpcId(int.Parse(playerName), "UpdateClassUI", GetNode<PlayerScript>(playerName).className, GetNode<PlayerScript>(playerName).health);
		if (IsMultiplayerAuthority()) //YESSS!!! IsMultiplayerAuthority() is NECESSARY for NOT duplicating player models!
		{
			Rpc("LoadModels", playersList);
		}
		if (GetTree().Root.GetNodeOrNull(classData.SpawnPoint) != null) //SCP CB Multiplayer moment (:
		{
			GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>(classData.SpawnPoint).GlobalPosition;
		}
		else //if simpler, if there is no spawnroom this round - force spawn in HCZ testroom.
		{
			GetNode<PlayerScript>(playerName).Position = GetTree().Root.GetNode<Marker3D>("Main/Game/MapGenHcz/HC_cont2_testroom/entityspawn").GlobalPosition;
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
			//if class exists, then despawn old model, and change it on a new one.
			if (ResourceLoader.Exists("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres")) 
			{
				Node ModelRoot = playerScript.GetNode("PlayerModel");
				if (ModelRoot.GetChild(0) != null)
				{
					ModelRoot.GetChild(0).QueueFree();
				}
				BaseClass classData = GD.Load<BaseClass>("res://FPSController/PlayerClassResources/" + playerScript.classKey + ".tres");
				Node tmpModel = ResourceLoader.Load<PackedScene>(classData.PlayerModelSource).Instantiate();
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
