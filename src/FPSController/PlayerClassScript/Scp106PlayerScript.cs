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
            GetNode<Control>("AbilityUI").Show();
        }
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, false);
        GetParent().GetParent<PlayerScript>().CanMove = true;
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        emergeSound = GetNode<AudioStreamPlayer3D>("TeleportSound");;
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: cooldown...";
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: ready! Click [Tab] to teleport to the player.";
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
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (Input.IsActionJustPressed("attack") && ray.IsColliding())
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is PlayerScript player)
                {
                    if (player.scpNumber == -1 && GetParent().GetParent<PlayerScript>().CanMove)
                    {
                        interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Laugh.ogg");
                        interactSound.Play();
                        if (GlobalPosition.Y < -1500)
                        {
                            player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
                        }
                        else
                        {
                            GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("TeleportTo", player.Name, PlacesForTeleporting.defaultData["pd_start"]);
                        }
                    }
                }
            }
        }
        
        if (Input.IsActionJustPressed("scp106_teleport") && !stalkCooldown)
        {
            if (GetParent().GetParent().GetParent().GetParent().GetNode<PlayerUI>("Game/PlayerUI").SpecialScreen)
            {
                GetParent().GetParent().GetParent().GetParent().GetNode<PlayerUI>("Game/PlayerUI").SpecialScreen = false;
                GetNode<StalkPanel>("AbilityUI/StalkPanel").Hide();
            }
            else
            {
                GetParent().GetParent().GetParent().GetParent().GetNode<PlayerUI>("Game/PlayerUI").SpecialScreen = true;
                GetNode<StalkPanel>("AbilityUI/StalkPanel").Show();
            }
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
    /// Teleporting SCP-106. Deprecated. Will be reworked in future.
    /// </summary>
    /*[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
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
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: cooldown...";
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
        stalkCooldown = false;
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: ready! Click [Tab] to teleport to the player.";
    }*/

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    async void Stalk(string room)
    {
        //Part 1. Stop movement, change camera and show the model.
        GetParent().GetParent<PlayerScript>().CanMove = false;
        GetParent().GetParent<PlayerScript>().GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = false;
        GetNode<Camera3D>("TeleportCamera").Current = true;
        GetNode<Node3D>("106_Rig").Show();
        //Part 2. Play the animation with sound, and wait for 3 seconds.
        Rpc("SetState", "106_FloorDespawn");
        emergeSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Decay" + rng.RandiRange(1, 3) + ".ogg");
        emergeSound.Play();
        await ToSignal(GetTree().CreateTimer(3.0), "timeout");
        //Part 3. Rotate 106's camera, teleport to a new room, play the sound, and wait again 3 seconds.
        GetNode<Camera3D>("TeleportCamera").RotateY(180);
        GetParent().GetParent().GetParent().GetParent().GetNode<FacilityManager>("Game").Rpc("TeleportTo", Multiplayer.GetUniqueId().ToString(), PlacesForTeleporting.defaultData[room]);
        Rpc("SetState", "106_FloorEmerge2");
        emergeSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/106/Decay0.ogg");
        emergeSound.Play();
        await ToSignal(GetTree().CreateTimer(3.0), "timeout");
        //Part 4. Revert part 1. changes and enable cooldown.
        GetNode<Node3D>("106_Rig").Hide();
        GetNode<Camera3D>("TeleportCamera").RotateY(-180);
        GetNode<Camera3D>("TeleportCamera").Current = false;
        GetParent().GetParent<PlayerScript>().GetNode<Camera3D>("PlayerHead/PlayerCamera").Current = true;
        GetParent().GetParent<PlayerScript>().CanMove = true;
        stalkCooldown = true;
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: cooldown...";
        //Part 5. Wait 30 seconds until stalk cooldown will be reverted.
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
        if (GlobalPosition.Y < -1500)
        {
            Stalk("hcz_106spawn");
        }
        stalkCooldown = false;
        GetNode<Label>("AbilityUI/VBoxContainer/Stalk").Text = "Stalk: ready! Click [Tab] to teleport to the player.";
    }
}
