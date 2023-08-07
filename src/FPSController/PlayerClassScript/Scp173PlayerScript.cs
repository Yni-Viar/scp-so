using Godot;
using System;

public partial class Scp173PlayerScript : Node3D
{
    [Export] bool canMove = true;
    internal bool CanMove {get=>canMove; set=>canMove = value;}
    double timer = 0d;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Scp173(delta);
	}

    async void Scp173(double delta)
    {
        //If SCP-173 is not moving, it should stand still!
        if (!CanMove || timer < 5.2d)
        {
            GetParent().GetParent<PlayerScript>().Velocity = Vector3.Zero;
            timer += delta;
        }
        else //move while blinking.
        {
            await ToSignal(GetTree().CreateTimer(0.3), "timeout");
            timer = 0d;
        }
    }
}
