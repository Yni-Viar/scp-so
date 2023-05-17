using Godot;
using System;
//BIG THANKS to: TheRadMatt and their repo under the MIT License: https://github.com/TheRadMatt/3D-FPP-Interaction-Demo
public partial class Pickable : RigidBody3D
{
    [Export] bool carriable = false;
    bool pickedUp = false;

    PlayerScript holder;

    Inventory inv;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (pickedUp)
        {
            this.GlobalTransform = holder.GetNode<Marker3D>("PlayerHead/PlayerHand").GlobalTransform;
        }
	}

    internal void PickUpItem(PlayerScript player)
    {
        holder = player;

        if (carriable)
        {
            if (pickedUp)
            {
                DropPickable();
            }
            else
            {
                CarryPickable();
            }
        }
        else
        {
            inv = holder.GetNode<Inventory>("UI/InventoryContainer/Inventory");
            switch (Name)
            {
                case "PDA":
                    inv.AddItem(ResourceLoader.Load("res://InventorySystem/Items/pda.tres"));
                    break;
                default:
                    GD.PrintErr("Unknown item. Cannot add to inventory.");
                    break;
            }
            this.QueueFree();
        }
    }

    internal void CarryPickable()
    {
        GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
        holder.holdingItem = this;
        this.Freeze = true;
        pickedUp = true;
    }

    internal void DropPickable()
    {
        GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
        holder.holdingItem = null;
        this.Freeze = false;
        pickedUp = false;
    }
}
