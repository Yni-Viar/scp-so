using Godot;
using System;

public partial class BallProjectile : Pickable
{
    [Export] bool scp018 = false;
    Vector3 vel = new Vector3(0.02f, 0.02f, 0.02f);
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        KinematicCollision3D collision = MoveAndCollide(vel);
        if (collision != null)
        {
            vel = vel.Bounce(collision.GetNormal());
            if (vel.X < 0.16f && vel.Y < 0.16f && vel.Z < 0.16f && scp018)
            {
                vel.X *= 2f;
                vel.Y *= 2f;
                vel.Z *= 2f;
            }
        }
    }
    
    
    private void OnArea3dBodyEntered(Node3D body)
    {
        if (body is PlayerScript player && scp018)
        {
            player.RpcId(int.Parse(player.Name), "HealthManage", vel.Y * -4, "Hit by SCP-018", 0);
        }
    }
}
