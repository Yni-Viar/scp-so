using Godot;
using System;

public partial class HumanPlayerScript : Node3D
{
    RayCast3D vision;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
        }
	}
}
