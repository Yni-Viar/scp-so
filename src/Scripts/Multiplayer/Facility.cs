using Godot;
using System;

public partial class Facility : Node3D
{
    //graphics settings field
    WorldEnvironment graphics = new WorldEnvironment();
	RandomNumberGenerator rng = new RandomNumberGenerator();
	CharacterBody3D playerScene;
    [Export] internal static bool spawn650 = false;

    // CharacterBody3D scp650Scene;
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

        /*if (SettingsPanel.HQSetting)
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingHighQuality.tres");
        }
        else
        {
            graphics.Environment = ResourceLoader.Load<Godot.Environment>("res://FacilityLightingLowQuality.tres");
        }*/


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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    void AddPlayer(long id)
    {
        playerScene = (CharacterBody3D)ResourceLoader.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate();
        playerScene.Name = id.ToString();
        // playerScene.Position = GetNode<Marker3D>("MapGenLCZ/LC_room1_archive/entityspawn").GlobalPosition;
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
}
