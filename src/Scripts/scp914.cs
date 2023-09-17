using Godot;
using System;
using System.Xml.Linq;

public partial class scp914 : AnimatableBody3D
{
    Random rnd = new Random(); //for items only

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

    Godot.Collections.Array<string> itemsToRefine = new Godot.Collections.Array<string>();
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
        for (int i = 0; i < itemsToRefine.Count; i++)
        {
            GD.Print("FOR works.");
            if (itemsToRefine.Count == 0)
            {
                break;
            }
            if (GetTree().Root.GetNodeOrNull("Main/Game/Items/" + itemsToRefine[i]) is Pickable inputPickable)
            {
                Item item = inputPickable.itemResource;
                GD.Print("IS an item");
                GD.Print(item.Rough.Length);
                GD.Print(item.Coarse.Length);
                GD.Print(item.OneToOne.Length);
                GD.Print(item.Fine.Length);
                GD.Print(item.VeryFine.Length);
                switch (currentMode)
                {
                    case Modes.ROUGH:
                        inputPickable.QueueFree();
                        if (item.Rough.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(item.Rough[rnd.Next(0, item.Rough.Length)]).Instantiate();
                            pickable.GlobalPosition = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                            GD.Print("Rough");
                        }
                        break;
                    case Modes.COARSE:
                        inputPickable.QueueFree();
                        if (item.Coarse.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(item.Coarse[rnd.Next(0, item.Coarse.Length)]).Instantiate();
                            pickable.GlobalPosition = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                            GD.Print("Coarse");
                        }
                        break;
                    case Modes.ONETOONE:
                        inputPickable.QueueFree();
                        if (item.OneToOne.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(item.OneToOne[rnd.Next(0, item.OneToOne.Length)]).Instantiate();
                            pickable.GlobalPosition = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                            GD.Print("1:1");
                        }
                        break;
                    case Modes.FINE:
                        inputPickable.QueueFree();
                        if (item.Fine.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(item.Fine[rnd.Next(0, item.Fine.Length)]).Instantiate();
                            pickable.GlobalPosition = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                            GD.Print("Fine");
                        }
                        break;
                    case Modes.VERYFINE:
                        inputPickable.QueueFree();
                        if (item.VeryFine.Length == 0)
                        {
                            break;
                        }
                        else
                        {
                            Pickable pickable = (Pickable)ResourceLoader.Load<PackedScene>(item.VeryFine[rnd.Next(0, item.VeryFine.Length)]).Instantiate();
                            pickable.GlobalPosition = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GetParent().GetNode<Node3D>("Items").AddChild(pickable);
                            GD.Print("VeryFine");
                        }
                        break;
                }
            }
        }
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
        itemsToRefine.Clear();
        playersToRefine.Clear();
		GetNode<CollisionShape3D>("DoorBlockIn").Disabled = true;
		GetNode<CollisionShape3D>("DoorBlockOut").Disabled = true;
        isRefining = false;
	}

    private void OnAddItemsAreaBodyEntered(Node3D body)
    {
        if (body is Pickable pickable)
        {
            Rpc("AddItem", pickable.Name);
        }
        if (body is PlayerScript player)
        {
            Rpc("AddPlayer", player.Name);
        }
    }


    private void OnAddItemsAreaBodyExited(Node3D body)
    {
        if (body is Pickable pickable)
        {
            Rpc("RemoveItem", pickable.Name);
        }
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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void AddItem(string name)
    {
        itemsToRefine.Add(name);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void RemoveItem(string name)
    {
        foreach (string item in itemsToRefine)
        {
            if (item == name)
            {
                itemsToRefine.Remove(item);
            }
        }
    }
}



