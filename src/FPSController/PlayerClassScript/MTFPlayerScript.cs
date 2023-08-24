using Godot;
using System;

public partial class MTFPlayerScript : Node3D
{
    // PlayerScript scp650;
    RayCast3D vision;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("Armature").Hide();
        }
        GetParent().GetParent<PlayerScript>().CanMove = true;
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            Rpc("SetState", "MTF_Idle");
        }
        else
        {
            if (Input.IsActionPressed("move_sprint"))
            {
                Rpc("SetState", "MTF_Running");
            }
            else
            {
                Rpc("SetState", "MTF_Walking");
            }
        }
        if (vision.IsColliding())
        {
            var collidedWith = vision.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == 173)
                {
                    player.GetNode<Scp173PlayerScript>("PlayerModel/scp173").RpcId(int.Parse(player.Name), "Scp173");
                }
            }
                /* THIS SECTION IS BUGGY!
                if (player.scpNumber == 650)
                {
                    player.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(player.Name), "Scp650", true);
                    scp650 = player; //if watched at 650, invoke its method and save it in var.
                }
                else
                {
                    if (scp650 != null) //free 650 from being watched.
                    {
                        scp650.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(scp650.Name), "Scp650", false);
                    }
                }
            }
            else
            {
                if (scp650 != null) //free 650 from being watched.
                {
                    scp650.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(scp650.Name), "Scp650", false);
                }
            }*/
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
