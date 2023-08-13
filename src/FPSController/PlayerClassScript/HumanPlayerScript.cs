using Godot;
using System;

public partial class HumanPlayerScript : Node3D
{
    PlayerScript scp650;
    RayCast3D vision;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GetParent().GetParent<PlayerScript>().CanMove = true;
        vision = GetParent().GetParent<PlayerScript>().GetNode<RayCast3D>("PlayerHead/VisionRadius");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (vision.IsColliding())
        {
            var collidedWith = vision.GetCollider();
            if (collidedWith is PlayerScript player)
            {
                if (player.scpNumber == 173)
                {
                    player.GetNode<Scp173PlayerScript>("PlayerModel/scp173").RpcId(int.Parse(player.Name), "Scp173");
                }
            }
                /* THIS SECTION IS BUGGY!
                if (player.scpNumber == 650)
                {
                    player.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(player.Name), "Scp650", true);
                    scp650 = player; //if watched at 650, invoke its method and save it in var.
                }
                else
                {
                    if (scp650 != null) //free 650 from being watched.
                    {
                        scp650.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(scp650.Name), "Scp650", false);
                    }
                }
            }
            else
            {
                if (scp650 != null) //free 650 from being watched.
                {
                    scp650.GetNode<Scp650PlayerScript>("PlayerModel/scp650").RpcId(int.Parse(scp650.Name), "Scp650", false);
                }
            }*/
        }
	}
}
