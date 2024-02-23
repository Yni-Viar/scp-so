using Godot;
using System;
using System.Drawing;
/// <summary>
/// Moving elevator system
/// </summary>
public partial class ElevatorSystem : Node3D
{
	enum LastMove { Up, Down }
	[Export] LastMove lastMove = LastMove.Up;
	[Export] string[] floors;
	[Export] string[] elevatorDoors;
	[Export] float speed = 2f;
	[Export] bool isMoving = false;
	[Export] string[] openDoorSounds;
	[Export] string[] closeDoorSounds;
    [Export] Godot.Collections.Array<string> objectsToTeleport = new Godot.Collections.Array<string>();
    [Export] int currentFloor;
	int nextFloor;
	Godot.Collections.Array<Vector3> targetPos = new Godot.Collections.Array<Vector3>();
	int counter = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!isMoving)
		{
			if (floors.Length == 1)
			{
				switch (lastMove)
				{
					case LastMove.Up:
						GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor] + "_Up").DoorOpen();
						break;
					case LastMove.Down:
						GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor] + "_Bottom").DoorOpen();
						break;
				}
			}
			else
			{
				GetTree().Root.GetNode<Door>(elevatorDoors[currentFloor]).DoorOpen();
			}
			DoorOpen();
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isMoving)
		{
			GlobalPosition = GlobalPosition.MoveToward(targetPos[counter], speed * (float)delta);
			for (int i = 0; i < objectsToTeleport.Count; i++)
			{
				Node3D node = GetNode<Node3D>(objectsToTeleport[i]);
				node.GlobalPosition += GetNode<Marker3D>("ObjectPos").GlobalPosition;
			}
			if (GlobalPosition == targetPos[counter])
			{
				if (counter < targetPos.Count - 1)
				{
					counter++;
				}
				else
				{
					Rpc(nameof(OpenDestDoors));
					isMoving = false;
					GetNode<AudioStreamPlayer3D>("Move").Stop();
					targetPos.Clear();
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
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>(openDoorSounds[rng.RandiRange(0, openDoorSounds.Length - 1)]);
		sfx.Play();
	}
	/// <summary>
	/// Closes the door
	/// </summary>
	void DoorClose()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("door_open", -1, -1, true);
		AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
		sfx.Stream = GD.Load<AudioStream>(closeDoorSounds[rng.RandiRange(0, closeDoorSounds.Length - 1)]);
		sfx.Play();
	}
	/// <summary>
	/// Moves elevator (network method)
	/// </summary>
	/// <param name="floor">Which floor (if there is only floor, then 0)</param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	internal void ElevatorMove(int floor)
	{
		if (isMoving)
		{
			return;
		}
		if (floors.Length == 1)
		{
			switch (lastMove)
			{
				case LastMove.Up:
					GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[0] + "_Up").DoorClose();
					break;
				case LastMove.Down:
					GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[0] + "_Bottom").DoorClose();
					break;
			}
		}
		else
		{
			GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[currentFloor]).DoorClose();
		}
		DoorClose();
		
		if (floors.Length == 1)
		{
			switch (lastMove)
			{
				case LastMove.Up:
					lastMove = LastMove.Down;
					targetPos.Add(FindNearestPoint(floor, false));
					break;
				case LastMove.Down:
					targetPos.Add(FindNearestPoint(floor, true));
					lastMove = LastMove.Up;
					break;
			}
		}
		else
		{
			if (floor < currentFloor)
			{
				for (int i = GetTree().GetNodesInGroup(floors[floor] + "Up").Count; i > 0; i--)
				{
					targetPos.Add(FindNearestPoint(floor, true));
				}
			}
			else if (floor > currentFloor)
			{
				for (int i = GetTree().GetNodesInGroup(floors[floor] + "Bottom").Count; i > 0; i--)
				{
					targetPos.Add(FindNearestPoint(floor, false));
				}
			}
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
		if (floors.Length == 1)
		{
			switch (lastMove)
			{
				case LastMove.Up:
					GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[0] + "_Up").DoorOpen();
					break;
				case LastMove.Down:
					GetTree().Root.GetNode<Door>("Main/" + elevatorDoors[0] + "_Bottom").DoorOpen();
					break;
			}
		}
		else
		{
			GetTree().Root.GetNode<Door>(elevatorDoors[currentFloor]).DoorOpen();
		}

		DoorOpen();
	}
	/// <summary>
	/// Finds nearest point.
	/// </summary>
	/// <param name="floor">Which floor (if there is only floor, then 0)</param>
	/// <param name="direction">Which direction is elevator going</param>
	/// <returns>Nearest point, where elevator will go through MoveToward</returns>
	Vector3 FindNearestPoint(int floor, bool direction)
	{
		float temp = 0f;
		float result = 0f;
		Vector3 resultPoint = Vector3.Zero;
		foreach (Node item in GetTree().GetNodesInGroup(floors[floor] + (direction ? "Up" : "Bottom")))
		{
			if (item is Marker3D pos)
			{
				temp = GlobalPosition.DistanceTo(pos.GlobalPosition);
				if (result > temp || result == 0f)
				{
					result = temp;
					resultPoint = pos.GlobalPosition;
				}
			}
		}
		return resultPoint;
	}

	private void InteractUp(Node3D player)
	{
		int dir = currentFloor--;
		if (floors.Length == 1 && lastMove == LastMove.Down)
		{
			Rpc("ElevatorMove", 0);
		}
		else if (dir >= 0 && !isMoving)
		{
			Rpc("ElevatorMove", dir); //move the elevator up.
		}
	}

	private void InteractDown(Node3D player)
	{
		int dir = currentFloor++;
		if (floors.Length == 1 && lastMove == LastMove.Up)
		{
			Rpc("ElevatorMove", 0);
		}
		else if (dir < floors.Length && !isMoving)
		{
			Rpc("ElevatorMove", dir); //move the elevator down.
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
