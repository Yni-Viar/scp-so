using Godot;
using System;
//BIG THANKS to: TheRadMatt and their repo under the MIT License: https://github.com/TheRadMatt/3D-FPP-Interaction-Demo
public partial class Pickable : RigidBody3D
{
    bool pickedUp = false;

    PlayerScript holder;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (pickedUp)
        {
            this.GlobalTransform = holder.GetNode<Marker3D>("PlayerHand").GlobalTransform;
        }
	}

    internal void PickUpItem(PlayerScript player)
    {
        holder = player;

        if (pickedUp)
        {
            Drop();
        }
        else
        {
            Carry();
        }
    }

    internal void Carry()
    {
        GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
        holder.holdingItem = this;
        this.Freeze = true;
        pickedUp = true;
    }

    internal void Drop()
    {
        GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
        holder.holdingItem = null;
        this.Freeze = false;
        pickedUp = false;
    }
}
