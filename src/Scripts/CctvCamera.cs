using Godot;
using System;

public partial class CctvCamera : Node3D
{
    bool active = false;
    Node3D rotatingCamera;
    Settings settings;
    RayCast3D ray;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        rotatingCamera = GetNode<Node3D>("RotatingNode");
        ray = GetNode<RayCast3D>("RotatingNode/RayCast3D");
        settings = GetTree().Root.GetNode<Settings>("Settings");
	}

    public override void _Input(InputEvent @event)
    {
        if (IsMultiplayerAuthority() && active)
        {
            if (@event is InputEventMouseMotion)
            {
                InputEventMouseMotion m = (InputEventMouseMotion) @event; 
                rotatingCamera.RotateY(Mathf.DegToRad(-m.Relative.X * settings.MouseSensitivity * 2f));//pretty magic numbers
                rotatingCamera.RotateX(Mathf.Clamp(-m.Relative.Y * settings.MouseSensitivity * 0.01f, -10, 10));
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //Interacting.
        if (ray.IsColliding() && Input.IsActionJustPressed("interact"))
        {
            var collidedWith = ray.GetCollider();
            if (collidedWith is ButtonInteract)
            {
                collidedWith.Call("Interact", this);
            }
            if (collidedWith is DoorStaticOpener)
            {
                collidedWith.Call("CallOpen");
            }
        }
	}

    internal void Initialize(bool isCurrent)
    {
        if (isCurrent)
        {
            rotatingCamera.Visible = true;
            rotatingCamera.GetNode<Camera3D>("Camera3D").Current = true;
            active = true;
        }
        else
        {
            rotatingCamera.Visible = false;
            rotatingCamera.GetNode<Camera3D>("Camera3D").Current = false;
            active = false;
        }
    }
}
