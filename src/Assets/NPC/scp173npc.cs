using Godot;
using System;

public partial class scp173npc : CharacterBody3D
{
	Node3D target;
	VisibleOnScreenNotifier3D look;
	//PlayerScript player = new PlayerScript();
	[Export] float speed = 20.0f;
	internal bool canMove = false;

	public override void _Ready()
	{
		look = GetNode<VisibleOnScreenNotifier3D>("CanSee");
	}

	public override void _PhysicsProcess(double delta)
	{
		/*if (target != null)
		{
			if (player.ray.IsColliding())
			{
				canMove = false;
			}
			else
			{
				canMove = true;
			}
		}*/
		if (canMove && target != null)
		{
			var playerDirection = (target.GlobalPosition - this.GlobalPosition).Normalized();
			Velocity += speed * playerDirection * (float)delta;
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









