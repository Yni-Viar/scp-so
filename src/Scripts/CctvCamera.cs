using Godot;
using System;
/// <summary>
/// CCTV camera system. Is available since 0.6.0.
/// </summary>
public partial class CctvCamera : Node3D
{
    Scp2522PlayerScript computer;
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (active)
        {
            if (Input.IsActionPressed("move_left_alt") || Input.IsActionPressed("move_right_alt"))
            {
                rotatingCamera.RotateY(Input.GetAxis("move_right_alt", "move_left_alt") * 0.025f);
            }
            else if (Input.IsActionPressed("move_forward_alt") || Input.IsActionPressed("move_backward_alt"))
            {
                if (rotatingCamera.GetNode<Camera3D>("Camera3D").RotationDegrees.X < -120.01f)
                {
                    rotatingCamera.GetNode<Camera3D>("Camera3D").RotationDegrees = new Vector3(-120f, 0f, 0f);
                }
                else if (rotatingCamera.GetNode<Camera3D>("Camera3D").RotationDegrees.X > -74.99f)
                {
                    rotatingCamera.GetNode<Camera3D>("Camera3D").RotationDegrees = new Vector3(-75f, 0f, 0f);
                }
                else
                {
                    rotatingCamera.GetNode<Camera3D>("Camera3D").RotateX(Input.GetAxis("move_backward_alt", "move_forward_alt") * 0.01f);
                }
            }

            //Interacting.
            if (ray.IsColliding() && Input.IsActionJustPressed("interact"))
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is InteractableCommon)
                {
                    collidedWith.Call("Interact", this);
                }
                if (collidedWith is DoorStaticOpener)
                {
                    collidedWith.Call("CallOpen");
                }
            }
            if (ray.IsColliding() && Input.IsActionJustPressed("door_lock") && computer != null)
            {
                var collidedWith = ray.GetCollider();
                if (collidedWith is DoorStaticOpener && computer.energy - 10f >= 0f)
                {
                    collidedWith.Call("CallLock");
                    computer.energy -= 10f;
                }
            }

            if (Input.IsActionJustPressed("scp2522_blackout") && computer != null)
            {
                if (computer.energy - 50f >= 0f)
                {
                    computer.energy -= 50f;
                    Rpc("RoomBlackout");
                }
            }
        }
	}
    /// <summary>
    /// Initialize the camera.
    /// </summary>
    /// <param name="isCurrent">Is the camera current.</param>
    internal void Initialize(bool isCurrent, Node3D player)
    {
        if (isCurrent)
        {
            if (player is Scp2522PlayerScript c)
            {
                computer = c;
            }
            rotatingCamera.Visible = true;
            rotatingCamera.GetNode<Camera3D>("Camera3D").Current = true;
            active = true;
        }
        else
        {
            if (computer != null)
            {
                computer = null;
            }
            rotatingCamera.Visible = false;
            rotatingCamera.GetNode<Camera3D>("Camera3D").Current = false;
            active = false;
        }
    }
    /// <summary>
    /// Room blackout controller.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    async void RoomBlackout()
    {
        foreach (Node item in GetParent().GetChildren())
        {
            if (item.IsInGroup("Lamp"))
            {
                if (item.GetNodeOrNull<LightSystem>("OmniLight3D") != null)
                {
                    item.GetNodeOrNull<LightSystem>("OmniLight3D").TurnLightsOff();
                }
            }
            /*if (item.IsInGroup("Light"))
            {
                if (item is OmniLight3D light)
                {
                    light.Visible = false;
                }
            }*/
        }
        await ToSignal(GetTree().CreateTimer(10.0), "timeout");
        foreach (Node item in GetParent().GetChildren())
        {
            if (item.IsInGroup("Lamp"))
            {
                if (item.GetNodeOrNull<LightSystem>("OmniLight3D") != null)
                {
                    item.GetNodeOrNull<LightSystem>("OmniLight3D").TurnLightsOn();
                }
            }
            /*if (item.IsInGroup("Light"))
            {
                if (item is OmniLight3D light)
                {
                    light.Visible = true;
                }
            }*/
        }
    }
}
