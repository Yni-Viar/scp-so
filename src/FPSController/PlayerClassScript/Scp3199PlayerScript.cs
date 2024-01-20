using Godot;
using System;
/// <summary>
/// SCP-3199 main script.
/// </summary>
public partial class Scp3199PlayerScript : Node3D
{
    RayCast3D ray;
    AudioStreamPlayer3D interactSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("Armature").Hide();
            GetNode<Control>("AbilityUI").Show();
        }
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        GetParent().GetParent<PlayerScript>().CanMove = true;
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != "3199_Idle")
            {
                Rpc("SetState", "3199_Idle");
            }
        }
        else
        {
            if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != "3199_Walk")
            {
                Rpc("SetState", "3199_Walk");
            }
        }
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (Input.IsActionJustPressed("attack") && ray.IsColliding())
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is PlayerScript player)
                {
                    if (player.scpNumber == -1 || player.team == Globals.Team.NSE)
                    {
                        Rpc("SetState", "3199_Hurt");
                        interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/SCPCommon/SCPAttack.ogg");
                        interactSound.Play();
                        player.RpcId(int.Parse(player.Name), "HealthManage", -35, "Hit by SCP-3199", 0);
                    }
                }
                if (collidedWith is Scp2522Recontain recon)
                {
                    recon.Rpc("Eject");
                }
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
}
