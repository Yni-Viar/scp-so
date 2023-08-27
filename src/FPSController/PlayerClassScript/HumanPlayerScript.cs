using Godot;
using System;

public partial class HumanPlayerScript : Node3D
{
    internal bool isWatchingAtScp173 = false;
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
            switch (Name)
            {
                case "mtf":
                    Rpc("SetState", "MTF_Idle");
                    break;
                default:
                    Rpc("SetState", "Idle");
                    break;
            }
        }
        else
        {
            if (Input.IsActionPressed("move_sprint"))
            {
                switch (Name)
                {
                    case "mtf":
                        Rpc("SetState", "MTF_Running");
                        break;
                    default:
                        Rpc("SetState", "Running");
                        break;
                }
            }
            else
            {
                switch (Name)
                {
                    case "mtf":
                        Rpc("SetState", "MTF_Walking");
                        break;
                    default:
                        Rpc("SetState", "Walking");
                        break;
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

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void WatchAtScp173()
    {
        if (vision.IsColliding())
        {
            var collidedWith = vision.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == 173)
                {
                    isWatchingAtScp173 = true;
                }
                else
                {
                    isWatchingAtScp173 = false;
                }
            }
            else
            {
                isWatchingAtScp173 = false;
            }
        }
    }


}
