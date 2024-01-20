using Godot;
using System;
/// <summary>
/// Main SCP-914 script.
/// </summary>
public partial class Scp914 : AnimatableBody3D
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
    /// Main refine method. Processes players and items and refines them.
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
                Item item = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.Items[inputPickable.item.InternalId];
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
                            GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
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
                            GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
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
                            GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
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
                            GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
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
                            GetTree().Root.GetNode<Node3D>("Main/Game/Items").AddChild(pickable);
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
                            if (Multiplayer.IsServer())
                            {
                                GD.Print("Rough");
                                player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Died at Rough setting at SCP-914", 0);
                            }
                            break;
                        case Modes.COARSE:
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            if (Multiplayer.IsServer())
                            {
                                GD.Print("Coarse");
                                player.RpcId(int.Parse(player.Name), "HealthManage", -30, "Died at Coarse setting at SCP-914", 0);
                            }
                            break;
                        case Modes.ONETOONE:
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            GD.Print("To be implemented, need to forceclass to another class");
                            break;
                        case Modes.FINE:
                            GD.Print("To be implemented, need to forceclass to invincible.");
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            /*if (Multiplayer.IsServer())
                            {
                                player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Not implemented.");
                            }*/
                            break;
                        case Modes.VERYFINE:
                            GD.Print("To be implemented, need to forceclass to invincible.");
                            player.Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                            /*if (Multiplayer.IsServer())
                            {
                                player.RpcId(int.Parse(player.Name), "HealthManage", -16777216, "Not implemented.");
                            }*/
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
    /// Adds player to refining queue.
    /// </summary>
    /// <param name="name">Player name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void AddPlayer(string name)
    {
        playersToRefine.Add(name);
    }
    /// <summary>
    /// Removes player from refining queue.
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
    /// <summary>
    /// Adds item to refining queue.
    /// </summary>
    /// <param name="name">Item name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void AddItem(string name)
    {
        itemsToRefine.Add(name);
    }
    /// <summary>
    /// Removes item from refining queue.
    /// </summary>
    /// <param name="name">Item name</param>
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



