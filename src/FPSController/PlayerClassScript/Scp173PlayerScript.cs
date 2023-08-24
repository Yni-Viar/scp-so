using Godot;
using System;

public partial class Scp173PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    double blinkTimer = 0d;
    RayCast3D ray;
    AudioStreamPlayer3D interactSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
        {
            GetNode<Node3D>("Merged_PM3D_Sphere3D4").Hide();
        }
        GetParent().GetParent<PlayerScript>().CanMove = true;
        ray = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/RayCast3D");
        interactSound = GetParent().GetParent<PlayerScript>().GetNode<AudioStreamPlayer3D>("InteractSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (blinkTimer < 5.2)
        {
            blinkTimer += delta;
        }
        else //move freely
        {
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }

        if (Input.IsActionJustPressed("attack") && ray.IsColliding())
        {
            var collidedWith = ray.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == -1)
                {
                    interactSound.Stream = GD.Load<AudioStream>("res://Sounds/Character/173/NeckSnap" + rng.RandiRange(1, 3) + ".ogg");
                    interactSound.Play();
                    player.RpcId(int.Parse(player.Name), "HealthManage", -16777216);
                }
            }
        }
	}
    /// <summary>
    /// Method, that holds blinking. Needs reworking...
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    async void Scp173()
    {
        //If SCP-173 is not moving, it should stand still!
        if (GetParent().GetParent<PlayerScript>().CanMove && blinkTimer < 5.2d)
        {
            GetParent().GetParent<PlayerScript>().CanMove = false;
        }
        else //move while blinking.
        {
            GetParent().GetParent<PlayerScript>().CanMove = true;
            await ToSignal(GetTree().CreateTimer(0.3), "timeout");
            blinkTimer = 0d;
        }
	}
}
