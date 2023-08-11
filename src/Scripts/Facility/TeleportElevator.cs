using Godot;
using System;

// 0.3.0 update: due to map gen change, the teleporting elevator script is unused. Maybe in future this script will be readded.
public partial class TeleportElevator : Node3D
{
    [Export] internal string[] elevators; // need to handle open-close doors
    [Export] internal string[] destinationPoints; // need for teleporting
    [Export] int currentFloor;
    [Export] Godot.Collections.Array<string> playersToTeleport  = new Godot.Collections.Array<string>();
    // bool isOpened = false;
    bool canInteract = true;

    bool CurrentDest() // check if the elevator is on the right floor at start.
    {
        if (destinationPoints[currentFloor] == GetChild<Marker3D>(0).Name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DoorOpen()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("door_open");
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>("res://Sounds/Door/DoorOpen" + Convert.ToString(rng.RandiRange(1, 3)) + ".ogg");
        sfx.Play();
    }

    void DoorClose()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("door_open", -1, -1, true);
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>("res://Sounds/Door/DoorClose" + Convert.ToString(rng.RandiRange(1, 3)) + ".ogg");
        sfx.Play();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (CurrentDest())
        {
            DoorOpen();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    public async void ElevatorMove(int direction)
    {
        Node3D fromTeleport = (Node3D)(GetTree().Root.GetNode("Main/" + elevators[currentFloor])); 
        // find first elevator node and close the door
        if (fromTeleport is TeleportElevator liftFrom)
        {
            liftFrom.DoorClose();
        }
        canInteract = false;
        Node3D toTeleport;
        toTeleport = (Node3D)(GetTree().Root.GetNode("Main/" + elevators[currentFloor + direction]));
        fromTeleport.GetNode<AudioStreamPlayer3D>("FakeMove").Play();
        toTeleport.GetNode<AudioStreamPlayer3D>("FakeMove").Play();
        await ToSignal(GetTree().CreateTimer(5.0), "timeout");
        GD.Print(playersToTeleport.Count);
        if (playersToTeleport.Count != 0)
        {
            for (int i = 0; i < playersToTeleport.Count; i++)
            {
                GD.Print("Yes.");
                // move player to the next zone.
                //playersToTeleport[i].Position = toTeleport.GetNode<Marker3D>(destinationPoints[currentFloor + direction]).GlobalPosition;
                GetTree().Root.GetNode<PlayerScript>("Main/Game/" + playersToTeleport[i]).Position = toTeleport.GetNode<Marker3D>(destinationPoints[currentFloor + direction]).GlobalPosition;
            }
        }
        
        await ToSignal(GetTree().CreateTimer(5.0), "timeout");
        canInteract = true;
        currentFloor += direction;
        // find second elevator node and open the door
        if (toTeleport is TeleportElevator liftTo)
        {
            liftTo.DoorOpen();
        }
    }

    private void OnButtonInteractInteracted(GodotObject player)
    {
        if (destinationPoints.Length == 3 && canInteract) // call elevator method.
        {
            if (GetChild<Marker3D>(0).Name == destinationPoints[0])
            {
                switch (currentFloor)
                {
                    case 1:
                        //ElevatorMove(-1);
                        Rpc("ElevatorMove", -1);
                        break;
                    case 2:
                        //ElevatorMove(-2);
                        Rpc("ElevatorMove", -2);
                        break;
                    default:
                        GD.PrintErr("Could not determine the floor you are in.");
                        break;
                }
            }
            else if (GetChild<Marker3D>(0).Name == destinationPoints[1])
            {
                switch (currentFloor)
                {
                    case 0:
                        //ElevatorMove(1);
                        Rpc("ElevatorMove", 1);
                        break;
                    case 2:
                        //ElevatorMove(-1);
                        Rpc("ElevatorMove", -1);
                        break;
                    default:
                        GD.PrintErr("Could not determine the floor you are in.");
                        break;
                }
            }
            else if (GetChild<Marker3D>(0).Name == destinationPoints[2])
            {
                switch (currentFloor)
                {
                    case 1:
                        //ElevatorMove(2);
                        Rpc("ElevatorMove", 2);
                        break;
                    case 2:
                        //ElevatorMove(1);
                        Rpc("ElevatorMove", 1);
                        break;
                    default:
                        GD.PrintErr("Could not determine the floor you are in.");
                        break;
                }
            }
        }
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void AddPlayer(string name)
    {
        playersToTeleport.Add(name);
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true)]
    void RemovePlayer(string name)
    {
        playersToTeleport.Remove(name);
    }

    private void OnButtonInteractUpInteracted(CharacterBody3D player)
    {
        if (GetChild<Marker3D>(0).Name != destinationPoints[2] && canInteract)
        {
            Rpc("ElevatorMove", 1); //move the elevator up.
        }
    }
    
    private void OnButtonInteractDownInteracted(CharacterBody3D player)
    {
        if (GetChild<Marker3D>(0).Name != destinationPoints[0] && canInteract)
        {
            Rpc("ElevatorMove", -1); //move the elevator down.
        }
    }

    private void OnTeleportAreaBodyEntered(Node3D body)
    {
        if (body is CharacterBody3D character)
        {
            //playersToTeleport.Add(character.Name);
            Rpc("AddPlayer", character.Name);
        }
    }

    private void OnTeleportAreaBodyExited(Node3D body)
    {
        if (body is CharacterBody3D character)
        {
            //playersToTeleport.Remove(character.Name);
            Rpc("RemovePlayer", character.Name);
        }
    }
}