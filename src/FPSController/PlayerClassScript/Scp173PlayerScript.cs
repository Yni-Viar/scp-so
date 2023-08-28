using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Scp173PlayerScript : Node3D
{
    RandomNumberGenerator rng = new RandomNumberGenerator();
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
                    if (player.GetNode("PlayerModel").GetChildOrNull<HumanPlayerScript>(0).isWatchingAtScp173)
                    {
                        LookingAtScp173(true, delta);
                    }
                    else
                    {
                        LookingAtScp173(false, delta);
                    }
                }
                else
                {
                    LookingAtScp173(false, delta);
                }
            }
            else
            {
                LookingAtScp173(false, delta);
            }
        }
    }

    /// <summary>
    /// Method, that holds blinking.
    /// </summary>
    void LookingAtScp173(bool isWatching, double delta)
    {
        if (isWatching)
        {
            //If SCP-173 is not moving, it should stand still!
            if (GetParent().GetParent<PlayerScript>().CanMove)
            {
                GetParent().GetParent<PlayerScript>().CanMove = false;
            }
        }
        else
        { //move freely
            GetParent().GetParent<PlayerScript>().CanMove = true;
        }
	}
}
