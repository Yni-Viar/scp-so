using Godot;
using System;
/// <summary>
/// SCP-650 NPC code
/// </summary>
public partial class Scp650Npc : AnimatableBody3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    [Export] bool stare = false;
    [Export] int stareCounter = 0;
    internal int StareCounter
    {
        get => stareCounter; set => stareCounter = value;
    }
    //needed to change pose
    double timer = 0f;
    string[] poses = new string[] { "Default", "Pose 1", "Pose 4", "Pose 5", "Pose 6", "Pose 7", "Pose 8", "Pose 9", "Pose 10" };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timer += delta;
        if (timer > 7.5f && !stare)
        {
            //teleporting
            GetTree().Root.GetNode<FacilityManager>("Main/Game/").Rpc("TeleportToPlayer", "NPCs/" + this.Name, "random");
            SetState(poses[rng.RandiRange(0, poses.Length - 1)]);
        }
    }

    /// <summary>
    /// Set animation.
    /// </summary>
    /// <param name="s">Animation name</param>
    private void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation == s)
        {
            return; //if this animation already applied, then no action.
        }
        //Change the animation.
        GetNode<AnimationPlayer>("AnimationPlayer").Play(s, 0.3d);
        timer = 0;
    }

    private void OnCanSeeScreenEntered()
    {
        stare = true;
    }

    private void OnCanSeeScreenExited()
    {
        stare = false;
    }

    private void OnStareAreaBodyEntered(Node3D body)
    {
        if (body is PlayerScript player && player.scpNumber == -1)
        {
            StareCounter++;
        }
        if (stareCounter > 0)
        {
            GetNode<VisibleOnScreenNotifier3D>("CanSee").Visible = true;
        }
    }

    private void OnStareAreaBodyExited(Node3D body)
    {
        if (body is PlayerScript player && player.scpNumber == -1)
        {
            StareCounter--;
        }
        if (stareCounter <= 0)
        {
            GetNode<VisibleOnScreenNotifier3D>("CanSee").Visible = false;
        }
    }
}