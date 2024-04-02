using Godot;
using System;
/// <summary>
/// Elevator call sequence.
/// </summary>
public partial class ElevatorButton : InteractableCommon
{
    [Export] string group;
    [Export] int floor;
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
        if (GetTree().GetFirstNodeInGroup(group) != null)
        {
            ElevatorSystem elevator = (ElevatorSystem)GetTree().GetFirstNodeInGroup(group);
            elevator.Rpc("CallElevator", floor);
        }
    }
}
