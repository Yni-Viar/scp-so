using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Scp173PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
    double blinkTimer = 0d;
    RayCast3D ray;
    RayCast3D vision;
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
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
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

        if (vision.IsColliding())
        {
            var collidedWith = vision.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == -1 && player.GetNode("PlayerModel").GetChildOrNull<HumanPlayerScript>(0) != null)
                {
                    player.GetNode("PlayerModel").GetChild<HumanPlayerScript>(0).Rpc("WatchingAtScp173");
                    if (player.GetNode("PlayerModel").GetChildOrNull<HumanPlayerScript>(0).isWatchingAtScp173)
                    {
                        LookingAtScp173(true);
                    }
                    else
                    {
                        LookingAtScp173(false);
                    }
                }
                else
                {
                    LookingAtScp173(false);
                }
            }
            else
            {
                LookingAtScp173(false);
            }
        }
    }

    /// <summary>
    /// Method, that holds blinking.
    /// </summary>
    async void LookingAtScp173(bool isWatching)
    {
        if (isWatching)
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
        else
        {
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
	}
}
