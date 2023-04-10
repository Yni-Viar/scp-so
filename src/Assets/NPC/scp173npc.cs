using Godot;
using System;

public partial class scp173npc : CharacterBody3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
	Node3D target;
	VisibleOnScreenNotifier3D look;

    AudioStreamPlayer3D walkSound;
    AudioStreamPlayer3D crunchSound;


	[Export] float speed = 20.0f;
	internal bool canMove = false;

	public override void _Ready()
	{
		look = GetNode<VisibleOnScreenNotifier3D>("CanSee");
        walkSound = GetNode<AudioStreamPlayer3D>("WalkSound");
        crunchSound = GetNode<AudioStreamPlayer3D>("CrunchSound");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (canMove && target != null)
		{
			var playerDirection = (target.GlobalPosition - this.GlobalPosition).Normalized();
			Velocity += speed * playerDirection * (float)delta;

            walkSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/Rattle" + rng.RandiRange(1, 3) + ".ogg");
            walkSound.Play();
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

    private void OnCrunchAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("Players"))
        {
            crunchSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/NeckSnap" + rng.RandiRange(1, 3) + ".ogg");
            crunchSound.Play();
        }
    }
}


