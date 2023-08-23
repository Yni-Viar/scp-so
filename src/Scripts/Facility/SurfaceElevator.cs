using Godot;
using System;


public partial class SurfaceElevator : Node3D
{
    [Export] internal string[] elevators; // need to handle open-close doors
    [Export] internal string[] destinationPoints; // need for teleporting, NEED DIFFERENT NAMES DUE TO TELEPORTING BEING DEPENDENT OF NAMES!
    [Export] int currentFloor;
    [Export] Godot.Collections.Array<string> playersToTeleport  = new Godot.Collections.Array<string>();
    [Export] string[] openDoorSounds;
    [Export] string[] closeDoorSounds;
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
        sfx.Stream = GD.Load<AudioStream>(openDoorSounds[rng.RandiRange(0, openDoorSounds.Length - 1)]);
        sfx.Play();
    }

    void DoorClose()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayer.Play("door_open", -1, -1, true);
        AudioStreamPlayer3D sfx = GetNode<AudioStreamPlayer3D>("DoorSound");
        sfx.Stream = GD.Load<AudioStream>(closeDoorSounds[rng.RandiRange(0, closeDoorSounds.Length - 1)]);
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
        if (fromTeleport is SurfaceElevator liftFrom)
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
        if (toTeleport is SurfaceElevator liftTo)
        {
            liftTo.DoorOpen();
        }
    }

    private void OnButtonInteractInteracted(GodotObject player)
    {
        if (destinationPoints.Length == 2 && canInteract) // call elevator method.
        {
            if (GetChild<Marker3D>(0).Name == destinationPoints[0])
            {
                switch (currentFloor)
                {
                    case 1:
                        //ElevatorMove(-1);
                        Rpc("ElevatorMove", 1);
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
                        Rpc("ElevatorMove", -1); // increment floor for reaching the player.
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
        if (GetChild<Marker3D>(0).Name != destinationPoints[1] && canInteract)
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