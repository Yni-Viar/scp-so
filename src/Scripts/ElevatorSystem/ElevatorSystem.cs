using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// Moving elevator system
/// </summary>
public partial class ElevatorSystem : Node3D
{
	enum LastMove { Up, Down }
	[Export] LastMove lastMove = LastMove.Up; 
	[Export] Godot.Collections.Array<ElevatorFloor> floors = new Godot.Collections.Array<ElevatorFloor>();
	[Export] string[] elevatorDoors;
	[Export] float speed = 2f;
	[Export] bool isMoving = false;
	[Export] string[] openDoorSounds;
	[Export] string[] closeDoorSounds;
    [Export] Godot.Collections.Array<string> objectsToTeleport = new Godot.Collections.Array<string>();
    [Export] int currentFloor;
	[Export] int targetFloor;
    [Export] Godot.Collections.Array<Vector3[]> waypoints = new Godot.Collections.Array<Vector3[]>();
	int counter = 0;
    bool passFloor = false;
	Vector3 rotation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rotation = Rotation;
		if (!isMoving)
		{
            if (elevatorDoors != null)
            {
			    GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor]).DoorOpen();
            }
			DoorOpen();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (isMoving && waypoints != null)
		{
            if (waypoints.Count > 0)
            {
                GlobalPosition = GlobalPosition.MoveToward(waypoints[counter][0], speed * (float)delta);
			    GlobalRotation = GlobalRotation.MoveToward(waypoints[counter][1] + rotation, speed * 0.25f * (float) delta);
			    for (int i = 0; i < objectsToTeleport.Count; i++)
			    {
			    	Node3D node = GetNode<Node3D>(objectsToTeleport[i]);
                    // Player can't move.
			    	//node.GlobalPosition = GetNode<Marker3D>("ObjectPos").GlobalPosition;
                    // Player CAN move! Yay!
                    node.GlobalPosition = node.GlobalPosition.MoveToward(waypoints[counter][0], speed * (float)delta);
			    	//node.Rotation = GetNode<Marker3D>("ObjectPos").GlobalRotation;
                }
                //remember, floating numbers needs IsEqualApprox, Yni!
			    if (GlobalPosition.IsEqualApprox(waypoints[counter][0]))
			    {
			    	if (counter < waypoints.Count - 1)
			    	{
			    		counter++;
			    	}
			    	else
			    	{
			    		counter = 0;
			    		waypoints.Clear();
                        if (passFloor)
                        {
                            if (lastMove == LastMove.Down && Multiplayer.IsServer())
                            {
                                if (currentFloor < targetFloor - 1)
                                {
                                    ElevatorMove(true);
                                }
                                else
                                {
                                    ElevatorMove(false);
                                }
                            }
                            else if (lastMove == LastMove.Up && Multiplayer.IsServer())
                            {
                                if (currentFloor > targetFloor - 1)
                                {
                                    ElevatorMove(true);
                                }
                                else
                                {
                                    ElevatorMove(false);
                                }
                            }
                        }
                        else
                        {
                            isMoving = false;
                            GetNode<AudioStreamPlayer3D>("Move").Stop();
                            Rpc(nameof(OpenDestDoors));
                        }
			    	}
			    }
            }
		}
	}
	/// <summary>
	/// Open the door
	/// </summary>
	void DoorOpen()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open");
        if (openDoorSounds != null)
        {
            AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
            sfx.Stream = GD.Load<AudioStream>(openDoorSounds[rng.RandiRange(0, openDoorSounds.Length - 1)]);
            sfx.Play();
        }
	}
	/// <summary>
	/// Closes the door
	/// </summary>
	void DoorClose()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open", -1, -1, true);
        if (closeDoorSounds != null)
        {
            AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
            sfx.Stream = GD.Load<AudioStream>(closeDoorSounds[rng.RandiRange(0, closeDoorSounds.Length - 1)]);
            sfx.Play();
        }
	}
    /*
	/// <summary>
	/// Moves elevator (network method)
	/// </summary>
	/// <param name="floor">Up or down (0.8.1 change)</param>

    */

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    internal void CallElevator(int floor)
    {
		if (isMoving || floor == currentFloor)
		{
			return;
		}
        targetFloor = floor;
        if (floors.Count == 1 || Mathf.Abs(targetFloor - currentFloor) == 1)
        {
            ElevatorMove(false, true);
        }
        else
        {
            ElevatorMove(true, true);
        }
    }

	internal void ElevatorMove(bool pass, bool first = false)
	{
        passFloor = pass;
        if (first)
        {
            if (elevatorDoors != null)
            {
                GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor]).DoorClose();
            }
		    DoorClose();
        }
        int floor;
        if (targetFloor < currentFloor)
        {
            lastMove = LastMove.Up;
            floor = currentFloor - 1;
            //check if upper point of current floor exist
            if (!string.IsNullOrEmpty(floors[currentFloor].UpHelperPoint))
		    {
                waypoints.Add(new Vector3[]
                {
                    GetNode<Marker3D>(floors[currentFloor].UpHelperPoint).GlobalPosition,
                    GetNode<Marker3D>(floors[currentFloor].UpHelperPoint).GlobalRotation
                });
		    }
            //check if lower point of next floor exist
            if (!string.IsNullOrEmpty(floors[floor].DownHelperPoint))
		    {
                waypoints.Add(new Vector3[]
                {
                    GetNode<Marker3D>(floors[floor].DownHelperPoint).GlobalPosition,
                    GetNode<Marker3D>(floors[floor].DownHelperPoint).GlobalRotation
                });
		    }
            //destination point
            waypoints.Add(new Vector3[]
            {
                GetNode<Marker3D>(floors[floor].DestinationPoint).GlobalPosition,
                GetNode<Marker3D>(floors[floor].DestinationPoint).GlobalRotation
            });
            currentFloor = floor;
        }
        else if (targetFloor > currentFloor)
        {
            lastMove = LastMove.Down;
            floor = currentFloor + 1;
            //check if lower point of current floor exist
            if (!string.IsNullOrEmpty(floors[currentFloor].DownHelperPoint))
		    {
                waypoints.Add(new Vector3[]
                {
                    GetNode<Marker3D>(floors[currentFloor].DownHelperPoint).GlobalPosition,
                    GetNode<Marker3D>(floors[currentFloor].DownHelperPoint).GlobalRotation
                });
		    }
            //check if upper point of next floor exist
            if (!string.IsNullOrEmpty(floors[floor].UpHelperPoint))
		    {
                waypoints.Add(new Vector3[]
                {
                    GetNode<Marker3D>(floors[floor].UpHelperPoint).GlobalPosition,
                    GetNode<Marker3D>(floors[floor].UpHelperPoint).GlobalRotation
                });
		    }
            //destination point
            waypoints.Add(new Vector3[]
            {
                GetNode<Marker3D>(floors[floor].DestinationPoint).GlobalPosition,
                GetNode<Marker3D>(floors[floor].DestinationPoint).GlobalRotation
            });
            currentFloor = floor;
        }
		isMoving = true;
		GetNode<AudioStreamPlayer3D>("Move").Play();
	}
	/// <summary>
	/// Opens destination doors.
	/// </summary>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	void OpenDestDoors()
	{
        if (elevatorDoors != null)
        {
		    GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor]).DoorOpen();
        }
		DoorOpen();
	}
	private void InteractUp(Node3D player)
	{
		int dir = currentFloor - 1;
        if (currentFloor >= 0 && !isMoving)
		{
			Rpc(nameof(CallElevator), dir); //move the elevator up.
		}
	}

	private void InteractDown(Node3D player)
	{
		int dir = currentFloor + 1;
        if (currentFloor < floors.Count && !isMoving)
		{
			Rpc(nameof(CallElevator), dir); //move the elevator down.
		}
	}
	
	private void OnPlayerAreaBodyEntered(Node3D body)
	{
        if (body is CharacterBody3D || body is Pickable || body is LootableAmmo)
        {
            //playersToTeleport.Add(character.Name);
            Rpc("AddObject", body.GetPath());
        }
    }
	
	private void OnPlayerAreaBodyExited(Node3D body)
	{
        if (body is CharacterBody3D || body is Pickable || body is LootableAmmo)
        {
            //playersToTeleport.Remove(character.Name);
            Rpc("RemoveObject", body.GetPath());
        }
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void AddObject(string name)
    {
        objectsToTeleport.Add(name);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void RemoveObject(string name)
    {
        objectsToTeleport.Remove(name);
    }
}
