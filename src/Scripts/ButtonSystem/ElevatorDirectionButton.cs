using Godot;
using System;
/// <summary>
/// Moving elevator button. (If you want to see code for teleporting elevator, see ElevatorCallButton)
/// </summary>
public partial class ElevatorDirectionButton : InteractableCommon
{
    enum Direction { Down, Up }
    [Export] Direction elevatorDirection = Direction.Up;
    /*
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	*/

    internal override void Interact(Node3D player)
    {
        if (player is PlayerScript || player is CctvCamera)
        {
            switch (elevatorDirection)
            {
                case Direction.Down:
                    if (GetParent().HasMethod("InteractDown"))
                    {
                        GetParent().Call("InteractDown", player);
                    }
                    break;
                case Direction.Up:
                    if (GetParent().HasMethod("InteractUp"))
                    {
                        GetParent().Call("InteractUp", player);
                    }
                    break;
            }
            base.Interact(player);
        }
    }
}
