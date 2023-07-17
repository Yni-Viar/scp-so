using Godot;
using System;

public partial class Scp018Activated : RigidBody3D
{
    Vector3 vel = new Vector3(0.01f, 0.01f, 0.01f);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        KinematicCollision3D collision = MoveAndCollide(vel * (float)delta);

        if (collision != null)
        {
            vel = vel.Bounce(collision.GetNormal());
            if ((vel.X < 0.64f && vel.Y < 0.64f && vel.Z < 0.64f))
            {
                vel.X *= 2;
                vel.Y *= 2;
                vel.Z *= 2;
            }
            else
            {
                vel.X *= 1;
                vel.Y *= 1;
                vel.Z *= 1;
            }
        }
    }
}
