using Godot;
using System;
/// <summary>
/// Floor data for an elevator
/// </summary>
public partial class ElevatorFloor : Resource
{
    /// <summary>
    /// Destination floor
    /// </summary>
    [Export]
    public string DestinationPoint { get; set; }
    /// <summary>
    /// Helper for upper floor, not necessary if the path is straight
    /// </summary>
    [Export]
    public string UpHelperPoint { get; set; }
    /// <summary>
    /// Helper for lower floor, not necessary if the path is straight
    /// </summary>
    [Export]
    public string DownHelperPoint { get; set; }
    public ElevatorFloor() : this(null, null, null) {}
    public ElevatorFloor(string destinationPoint, string upHelperPoint, string downHelperPoint)
    {
        DestinationPoint = destinationPoint;
        UpHelperPoint = upHelperPoint;
        DownHelperPoint = downHelperPoint;
    }
}
