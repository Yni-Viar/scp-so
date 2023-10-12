using Godot;
using System;

public partial class CctvCamera : Node3D
{
    [Export] bool active = false;
    Node3D rotatingCamera;
    Settings settings;
    RayCast3D ray;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        rotatingCamera = GetNode<Node3D>("RotatingNode");
        ray = GetNode<RayCast3D>("RotatingNode/Camera3D/RayCast3D");
        settings = GetTree().Root.GetNode<Settings>("Settings");
	}

    /*public override void _Input(InputEvent @event)
    {
        if (IsMultiplayerAuthority() && active)
        {
            if (@event is InputEventMouseMotion)
            {
                InputEventMouseMotion m = (InputEventMouseMotion) @event;
                rotatingCamera.Rotate(Vector3.Up, Mathf.DegToRad(m.Relative.X * settings.MouseSensitivity * 2f)); //pretty magic numbers
                
                rotatingCamera.RotateObjectLocal(Vector3.Right, Mathf.DegToRad(m.Relative.X * settings.MouseSensitivity * 2f));
                Vector3 cameraRot = rotatingCamera.RotationDegrees;
                cameraRot.X = Mathf.Clamp(rotatingCamera.RotationDegrees.X, 45f, 75f);
                rotatingCamera.RotationDegrees = cameraRot;

                // deprecated values from v.0.6.0-dev.
                //rotatingCamera.RotateY(Mathf.DegToRad(-m.Relative.X * settings.MouseSensitivity * 2f));
                //rotatingCamera.RotateX(Mathf.Clamp(-m.Relative.Y * settings.MouseSensitivity * 0.125f, -90, 90));
                //rotatingCamera.RotateX(Mathf.DegToRad(-m.Relative.X * settings.MouseSensitivity * 2f));

                //rotatingCamera.RotateObjectLocal(Vector3.Right, Mathf.Clamp(m.Relative.Y * settings.MouseSensitivity * 0.125f, 35, 46));
            }
            

        }
    }*/

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (IsMultiplayerAuthority() && active)
        {
            if (Input.IsActionPressed("move_left_alt") || Input.IsActionPressed("move_right_alt"))
            {
                rotatingCamera.RotateY(Input.GetAxis("move_right_alt", "move_left_alt") * 0.025f);
            }
            else if ((Input.IsActionPressed("move_forward_alt") || Input.IsActionPressed("move_backward_alt"))
                && rotatingCamera.Rotation.X <= 1.309f && rotatingCamera.Rotation.X >= 0.785398f)
            {
                rotatingCamera.GetNode<Camera3D>("Camera3D").RotateX(Input.GetAxis("move_backward_alt", "move_forward_alt") * 0.01f);
            }
            Vector3 cameraRot = rotatingCamera.RotationDegrees;
            cameraRot.X = Mathf.Clamp(rotatingCamera.RotationDegrees.X, 45f, 75f);
            rotatingCamera.RotationDegrees = cameraRot;

            //Interacting.
            if (ray.IsColliding() && Input.IsActionJustPressed("interact"))
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is ButtonInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is ButtonKeycardInteract)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is DoorStaticOpener)
                {
                    collidedWith.Call("CallOpen");
                }
            }
            if (ray.IsColliding() && Input.IsActionJustPressed("door_lock"))
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is DoorStaticOpener)
                {
                    collidedWith.Call("CallLock");
                }
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
