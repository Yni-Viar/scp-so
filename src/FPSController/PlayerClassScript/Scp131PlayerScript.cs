using Godot;
using System;

public partial class Scp131PlayerScript : Node3D
{
    [Export] internal bool isWatchingAtScp173 = false;
    RayCast3D vision;
    bool scanCooldown = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("SCP131_skeleton").Hide();
            GetNode<Control>("AbilityUI").Show();
        }
        GetParent().GetParent<PlayerScript>().CanMove = true;
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
        GetNode<Label>("AbilityUI/VBoxContainer/ScanHostiles").Text = "Scan hostiles: ready! Click [F] to scan.";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (GetParent().GetParent<PlayerScript>().dir.IsZeroApprox())
        {
            Rpc("SetState", "131_Idle");
        }
        else
        {
            Rpc("SetState", "131_Walk");
        }
        if (Input.IsActionJustPressed("scp131_scan") && !scanCooldown)
        {
            Scan();
        }
        WatchAtScp173();
    }
    /// <summary>
    /// Scans for enemies in a radius.
    /// </summary>
    async void Scan()
    {
        scanCooldown = true;
        GetNode<Label>("AbilityUI/VBoxContainer/ScanHostiles").Text = "Scan hostiles: cooldown...";
        GetNode<Area3D>("Area3D").BodyEntered += OnBodyEntered;
        await ToSignal(GetTree().CreateTimer(5.0), "timeout");
        GetNode<Area3D>("Area3D").BodyEntered -= OnBodyEntered;
        await ToSignal(GetTree().CreateTimer(15.0), "timeout");
        scanCooldown = false;
        GetNode<Label>("AbilityUI/VBoxContainer/ScanHostiles").Text = "Scan hostiles: ready! Click [F] to scan.";
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
            if (player.team == Globals.Team.DSE)
            {
                Scp131Signal(true);
            }
            else
            {
                Scp131Signal(false);
            }
        }
    }
    /// <summary>
    /// Helper method for determining state of SCP-131, while scanning.
    /// </summary>
    /// <param name="isDanger">State of emotion, the animation that will be played</param>
    void Scp131Signal(bool isDanger)
    {
        if (isDanger)
        {
            Rpc("SetState", "131_Panic");
        }
        else
        {
            Rpc("SetState", "131_Emote");
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
    /// Method, that holds watching at SCP-173.
    /// </summary>
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
