using Godot;
using System;

public partial class BallProjectile : Pickable
{
    [Export] bool scp018 = false;
    Vector3 vel = new Vector3(0.04f, 0.04f, 0.04f);
    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(30.0), "timeout");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        KinematicCollision3D collision = MoveAndCollide(vel * (float)delta);
        if (collision != null)
        {
            vel = vel.Bounce(collision.GetNormal());
            if (vel.X < 0.32f && vel.Y < 0.32f && vel.Z < 0.32f && scp018)
            {
                vel.X *= 1.5f;
                vel.Y *= 1.5f;
                vel.Z *= 1.5f;
            }
        }
    }
    
    
    private void OnArea3dBodyEntered(Node3D body)
    {
        if (body is PlayerScript player && scp018)
        {
            player.RpcId(int.Parse(player.Name), "HealthManage", vel.Y * -25, "Hit by SCP-018");
        }
    }
}
