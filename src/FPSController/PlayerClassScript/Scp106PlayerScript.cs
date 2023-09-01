using Godot;
using System;

public partial class Scp106PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    RayCast3D ray;
    AudioStreamPlayer3D interactSound;
    AudioStreamPlayer3D breathSound;
    AudioStreamPlayer3D emergeSound;
    bool stalkCooldown = true;
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("106_Rig").Hide();
        }
        GetParent().GetParent<PlayerScript>().CanMove = true;
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        emergeSound = GetNode<AudioStreamPlayer3D>("TeleportSound");
        breathSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("BreathSound");
        breathSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Breathing.ogg");
        breathSound.Play();
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        stalkCooldown = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            Rpc("SetState", "106_Idle");
        }
        else
        {
            Rpc("SetState", "106_Walking");
        }
        if (Input.IsActionJustPressed("attack") && ray.IsColliding())
        {
            var collidedWith = ray.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == -1)
                {
                    interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Laugh.ogg");
                    interactSound.Play();
                    GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("TeleportTo", player.Name, PlacesForTeleporting.defaultData["pd_start"]);
                }
            }
        }
        if (Input.IsActionJustPressed("scp106_teleport") && !stalkCooldown)
        {
            Stalk();
        }
    }

    /// <summary>
    /// Set animation to an entity.
    /// </summary>
    /// <param name="s">Animation name</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s)
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }

    /// <summary>
    /// Teleporting SCP-106.
    /// </summary>
    async void Stalk()
    {
        GetParent().GetParent<PlayerScript>().CanMove = false;
        Rpc("SetState", "106_FloorDespawn");
        emergeSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Decay" + rng.RandiRange(1, 3) + ".ogg");
        emergeSound.Play();
        await ToSignal(GetTree().CreateTimer(3.0), "timeout");
        GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("RandomTeleport", GetParent().GetParent<PlayerScript>().Name);
        Rpc("SetState", "106_FloorEmerge2");
        emergeSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Decay0.ogg");
        emergeSound.Play();
        await ToSignal(GetTree().CreateTimer(3.0), "timeout");
        GetParent().GetParent<PlayerScript>().CanMove = true;
        stalkCooldown = true;
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
        stalkCooldown = false;
    }
}
