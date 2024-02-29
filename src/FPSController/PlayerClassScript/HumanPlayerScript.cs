using Godot;
using System;
/// <summary>
/// Human classes main script.
/// </summary>
public partial class HumanPlayerScript : Node3D
{
    [Export] internal string armatureName;
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
            GetParent().GetParent<PlayerScript>().GetNode<Marker3D>("PlayerHead/PlayerHand").Show();
        }
        GetParent().GetParent<PlayerScript>().SetCollisionMaskValue(3, true);
        GetParent().GetParent<PlayerScript>().CanMove = true;
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/PlayerRecoil/VisionRadius");
        OnStart();
    }

    internal virtual void OnStart()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
    {
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
        }
        OnUpdate(delta);
    }

    internal virtual void OnUpdate(double delta)
    {

    }
    /*
    /// <summary>
    /// Set animation to an entity. Deprecated as for 0.7.0-dev.
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
    */
    /// <summary>
    /// Set animation to an entity via Animation Tree. Available since 0.7.0-dev
    /// </summary>
    /// <param name="anim">Name of animation tree node</param>
    /// <param name="action">AnimationTree node parameters</param>
    /// <param name="amount">Value (e.g. for Blend3 is a float, that ranges from -1.0 to 1.0 and for Blend2 is 0.0 to 1.0)</param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void SetState(string anim, string action, Variant amount)
    {
        //for Blend* parameter is blend_amount

        //check if state changed
        //if (!(GetNode<AnimationTree>("AnimationTree").Get("parameters/" + anim + "/" + action).AsDouble() == amount.AsDouble()))
        //{
            AnimationTree animation = GetNode<AnimationTree>("AnimationTree");
            animation.Set("parameters/" + anim + "/" + action, amount);
        //}
    }
    /*
    /// <summary>
    /// Method, that holds watching at SCP-173. Deprecated since 0.7.1-dev.
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
    */
}
