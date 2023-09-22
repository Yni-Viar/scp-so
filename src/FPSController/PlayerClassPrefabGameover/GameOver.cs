using Godot;
using System;

public partial class GameOver : Node
{
    [Export] string state;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SetState(state);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    internal void SetState(string s)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").CurrentAnimation != s)
        {
            //Change the animation.
            GetNode<AnimationPlayer>("AnimationPlayer").Play(s);
        }
    }
}
