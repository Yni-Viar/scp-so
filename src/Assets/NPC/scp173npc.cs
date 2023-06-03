using Godot;
using System;

public partial class scp173npc : CharacterBody3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	Node3D target;
	AudioStreamPlayer3D walkSound;

	[Export] float speed = 80.0f;
	internal bool canMove = false;

	public override void _Ready()
	{
		walkSound = GetNode<AudioStreamPlayer3D>("WalkSound");
	}

	public override void _PhysicsProcess(double delta)
	{
        //basic SCP-173 rules - player don't see - 173 is closer.
		if ((canMove || PlayerCommon.isBlinking) && target != null)
		{
			var playerDirection = (target.GlobalPosition - this.GlobalPosition).Normalized();
			Velocity += speed * playerDirection * (float)delta;
            this.LookAt(target.GlobalPosition);

			walkSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/Rattle" + rng.RandiRange(1, 3) + ".ogg");
			walkSound.Play();
		}
        //else SCP-173 must stay still!
        else
        {
            Velocity = Vector3.Zero;
        }
		MoveAndSlide();
	}

	private void OnFindPlayerAreaBodyEntered(Node3D body)
	{
		if (body.IsInGroup("Players"))
		{
			target = body;
		}
	}


	private void OnFindPlayerAreaBodyExited(Node3D body)
	{
		if (body.IsInGroup("Players"))
		{
			target = null;
		}
	}

	private void OnCanSeeScreenEntered()
	{
		canMove = false;
	}

	private void OnCanSeeScreenExited()
	{
        canMove = true;
	}
}