using Godot;
using System;

public partial class HumanPlayerScript : Node3D
{
    [Export] string armatureName;
    [Export] internal bool isWatchingAtScp173 = false;
    bool isBlinking = false;
    RayCast3D vision;
    double blinkTimer = 0d;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>(armatureName).Hide();
        }
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        GetParent().GetParent<PlayerScript>().CanMove = true;
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
        OnStart();
    }

    internal virtual void OnStart()
    {

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
        /* DEPRECATED, since 0.7.0-dev
         * if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            if (Name == "mtf" || Name == "hazmat_scientist")
            {
                Rpc("SetState", "MTF_Idle");
            }
            else
            {
                Rpc("SetState", "Idle");
            }
        }
        else
        {
            if (Input.IsActionPressed("move_sprint"))
            {
                if (Name == "mtf" || Name == "hazmat_scientist")
                {
                    Rpc("SetState", "MTF_Running");
                }
                else
                {
                    Rpc("SetState", "Running");
                }
            }
            else
            {
                if (Name == "mtf" || Name == "hazmat_scientist")
                {
                    Rpc("SetState", "MTF_Walking");
                }
                else
                {
                    Rpc("SetState", "Walking");
                }
            }
        }*/
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            if (blinkTimer < 5.2d)
            {
                blinkTimer += delta;
            }
            else
            {
                isBlinking = true;
                await ToSignal(GetTree().CreateTimer(0.3), "timeout");
                blinkTimer = 0d;
                isBlinking = false;
            }
            WatchAtScp173();
        }
        
    }

    internal virtual void AnimationCycle()
    {

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
    /// Method, that holds watching at SCP-173.
    /// </summary>
    void WatchAtScp173()
    {
        if (vision.IsColliding())
        {
            var collidedWith = vision.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == 173 && !isBlinking)
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
