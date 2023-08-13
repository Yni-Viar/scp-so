using Godot;
using System;

public partial class Scp3199PlayerScript : Node3D
{
    RayCast3D ray;
    AudioStreamPlayer3D interactSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetParent().GetParent<PlayerScript>().CanMove = true;
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var state = GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation;
        if (GetParent().GetParent<PlayerScript>().Velocity.IsZeroApprox())
        {
            Rpc("SetState", "3199_Idle");
        }
        else
        {
            Rpc("SetState", "3199_Walk");
        }
        if (Input.IsActionJustPressed("attack") && ray.IsColliding())
        {
            var collidedWith = ray.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == -1)
                {
                    Rpc("SetState", "3199_Hurt");
                    interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/SCPCommon/SCPAttack.ogg");
                    interactSound.Play();
                    player.RpcId(int.Parse(player.Name), "HealthManage", -35);
                }
            }
        }
	}

    //Set animation to an entity.
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation == s)
        {
            return; //if this animation already applied, then no action.
        }
        //Change the animation.
        GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
    }
}
