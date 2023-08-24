using Godot;
using System;

public partial class scp914 : AnimatableBody3D
{
    Random rnd = new Random();
	internal enum Modes
	{
		ROUGH,
		COARSE,
		ONETOONE,
		FINE,
		VERYFINE
	}

    // Godot.Collections.Array<Pickable> itemsToRefine = new Godot.Collections.Array<Pickable>();
    Godot.Collections.Array<string> playersToRefine  = new Godot.Collections.Array<string>();

	internal bool isRefining = false;
	internal static Modes currentMode = Modes.ONETOONE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

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
        /*for (int i = 0; i < playersToRefine.Count; i++) //not working
        {
            if (playersToRefine.Count == 0)
            {
                break;
            }
            else
            {
                switch (currentMode)
                {
                    case Modes.ROUGH:
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).RpcId(int.Parse(GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Name), "HealthManage", -16777216);
                        break;
                    case Modes.COARSE:
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).RpcId(int.Parse(GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Name), "HealthManage", -50);
                        break;
                    case Modes.ONETOONE:
                        GD.Print("To be implemented, need to forceclass to another human");
                        break;
                    case Modes.FINE:
                        GD.Print("To be implemented, need to forceclass to invincible human. Currently is working like ROUGH");
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).RpcId(int.Parse(GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Name), "HealthManage", -16777216);
                        break;
                    case Modes.VERYFINE:
                        GD.Print("To be implemented, need to forceclass to invincible human. Currently is working like ROUGH");
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Position = GetNode<Marker3D>("SpawnRefinedItems").GlobalPosition;
                        GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).RpcId(int.Parse(GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToRefine[i]).Name), "HealthManage", -16777216);
                        break;
                }
            }
        }
        */
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
            playersToRefine.Add(player.Name);
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
            playersToRefine.Remove(player.Name);
        }
    }
}



