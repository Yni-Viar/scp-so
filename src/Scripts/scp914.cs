using Godot;
using System;
using System.Xml.Linq;

public partial class scp914 : AnimatableBody3D
{
    //Random rnd = new Random(); //for items only

    /// <summary>
    /// All SCP-914 modes
    /// </summary>
	internal enum Modes
	{
		ROUGH,
		COARSE,
		ONETOONE,
		FINE,
		VERYFINE
	}

    // Godot.Collections.Array<Pickable> itemsToRefine = new Godot.Collections.Array<Pickable>();
    Godot.Collections.Array<string> playersToRefine = new Godot.Collections.Array<string>();

	[Export] internal bool isRefining = false;
	[Export] internal Modes currentMode = Modes.ONETOONE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    /// <summary>
    /// Main refine method. Processes players (and in future - also items) and refines them.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public async void Refine()
	{
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AudioStreamPlayer3D refineAudio = GetNode<AudioStreamPlayer3D>("RefineSound");
        isRefining = true;
		GetNode<CollisionShape3D>("DoorBlockIn").Disabled = false;
		GetNode<CollisionShape3D>("DoorBlockOut").Disabled = false;
		if (refineAudio.Playing == false)
		{
			refineAudio.Play();
		}
		animPlayer.Play("Armature|Armature|Armature|Armature|Event|Armature|Event|Armatu");
		await ToSignal(GetTree().CreateTimer(6.0), "timeout");
        for (int i = 0; i < playersToRefine.Count; i++) //don't forget about layers, Yni...
        {
            if (playersToRefine.Count == 0)
            {
                break;
            }
            else
            {
                if (GetTree().Root.GetNodeOrNull("Main/Game/" + playersToRefine[i]) is PlayerScript player)
                {
                    switch (currentMode)
                    {
                        case Modes.ROUGH:
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
                            break;
                        case Modes.COARSE:
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            player.RpcId(int.Parse(player.Name), "HealthManage", -30);
                            break;
                        case Modes.ONETOONE:
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GD.Print("To be implemented, need to forceclass to another human");
                            break;
                        case Modes.FINE:
                            GD.Print("To be implemented, need to forceclass to invincible human. Currently is working like ROUGH");
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
                            break;
                        case Modes.VERYFINE:
                            GD.Print("To be implemented, need to forceclass to invincible human. Currently is working like ROUGH");
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
                            break;
                    }
                }
            }
        }
        await ToSignal(GetTree().CreateTimer(6.0), "timeout");
        // itemsToRefine.Clear();
        playersToRefine.Clear();
		GetNode<CollisionShape3D>("DoorBlockIn").Disabled = true;
		GetNode<CollisionShape3D>("DoorBlockOut").Disabled = true;
        isRefining = false;
	}

    private void OnAddItemsAreaBodyEntered(Node3D body)
    {
        /*if (body is Pickable pickable)
        {
            itemsToRefine.Add(pickable);
            GD.Print("Added item to 914");
        }*/
        if (body is PlayerScript player)
        {
            Rpc("AddPlayer", player.Name);
        }
    }


    private void OnAddItemsAreaBodyExited(Node3D body)
    {
        /*if (body is Pickable pickable)
        {
            itemsToRefine.Remove(pickable);
            GD.Print("Removed item from 914");
        }*/
        if (body is PlayerScript player)
        {
            Rpc("RemovePlayer", player.Name);
        }
    }

    /// <summary>
    /// Adds player to array for all players.
    /// </summary>
    /// <param name="name">Player name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void AddPlayer(string name)
    {
        playersToRefine.Add(name);
    }
    /// <summary>
    /// Removes player from array for all players.
    /// </summary>
    /// <param name="name">Player name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void RemovePlayer(string name)
    {
        foreach (string item in playersToRefine)
        {
            if (item == name)
            {
                playersToRefine.Remove(item);
            }
        }
    }
}



