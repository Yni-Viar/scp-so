using Godot;
using System;

public partial class scp650npc : CharacterBody3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();

    Node3D target;
    float rotationSpeed = 0.01f;
    float gravity = 9.8f;

    double timer = 0f;
    float waiting;
    internal int pose;

    Vector3 addPos = new Vector3(0, 0, 2);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        waiting = 7.5f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        var state = GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation;
        if (target != null)
        {
            timer += delta;
            if (state == "Default")
            {
                if (this.Transform.Origin.DistanceTo(target.Transform.Origin) < 4)
                {
                    timer = 0d;
                }
                else
                {
                    if (timer > waiting)
                    {
                        SetState("Pose 1");
                    }
                }
            }
            if (state != "Default")
            {
                if (this.Transform.Origin.DistanceTo(target.Transform.Origin) < 4)
                {
                    timer = 0d;
                }
                else
                {
                    if (timer > waiting)
                    {
                        this.RotateY((float)Math.PI);
                        //Godot.Collections.Array<Godot.Node> scp650Spawns = GetTree().GetNodesInGroup("650Spawns");
                        //Marker3D behindPos = (Marker3D)scp650Spawns[rng.RandiRange(0, (scp650Spawns.Count - 1))];
                        //this.Position = behindPos.Position; //partly working solution;
                        
                        //Marker3D behindPos = target.GetNode<Marker3D>("650Spawn");
                        this.Position = target.Transform.Origin + addPos; //partly working solution;
                        switch (pose)
                        {
                            case 0:
                                SetState("Pose 1");
                                break;
                            case 1:
                                SetState("Pose 4");
                                break;
                            case 2:
                                SetState("Pose 5");
                                break;
                            case 3:
                                SetState("Pose 6");
                                break;
                            case 4:
                                SetState("Pose 7");
                                break;
                            case 5:
                                SetState("Pose 8");
                                break;
                            case 6:
                                SetState("Pose 9");
                                break;
                            case 7:
                                SetState("Pose 10");
                                break;
                        }
                    }
                }
            }
        }
    }

    //Set animation to an entity.
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

    private void OnTeleportAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("Players"))
        {
            target = body;
        }
    }

    private void OnTeleportAreaBodyExited(Node3D body)
    {
        if (body.IsInGroup("Players"))
        {
            target = null;
        }
    }

    private void OnCanSeeScreenEntered()
    {
        timer = 0d;
    }
}



