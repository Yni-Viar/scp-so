using Godot;
using System;
/// <summary>
/// RZ-special generation parameters.
/// </summary>
public partial class MapGeneratorRz : MapGenerator
{
    internal override void CreateMap()
    {
        base.CreateMap();
        RandomNumberGenerator rng = new RandomNumberGenerator();
        bool checkpointSpawned = false;
        bool exitSpawned = false;
        string selectedRoom;
        int currRoom2 = 0;
        // you can add more vars for different needs, e.g. currRoom3 or currRoom2c
        
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                StaticBody3D rm;
                switch (roomTemp[i, j].type)
                {
                    case RoomTypes.ROOM1:
                        if (!checkpointSpawned)
                        {
                            rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/rz_ckpt_1.tscn").Instantiate();
                            rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                            rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                            AddChild(rm, true);
                            roomTemp[i, j].roomName = rm.Name;
                            checkpointSpawned = true;
                        }
                        else if (!exitSpawned)
                        {
                            rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/rz_exit_1.tscn").Instantiate();
                            rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                            rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                            AddChild(rm, true);
                            roomTemp[i, j].roomName = rm.Name;
                            exitSpawned = true;
                        }
                        else
                        {
                            rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_endroom.tscn").Instantiate();
                            rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                            rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                            AddChild(rm, true);
                            roomTemp[i, j].roomName = rm.Name;
                        }
                        break;
                    case RoomTypes.ROOM2:
                        if (currRoom2 >= RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["RzSingle2"].Count)
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["RzCommon2"][rng.RandiRange(0, RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczCommon1"].Count - 1)];
                        }
                        else
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["RzSingle2"][currRoom2];
                        }
                        currRoom2++;
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/" + selectedRoom + ".tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM2C:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/rz_room_2c.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM3:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/rz_room_3.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM4:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/rz_room_4.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                }
            }
        }

        for (int k = 0; k < 12; k++)
        {
            for (int l = 0; l < 12; l++)
            {
                bool southC, eastC;
                southC = eastC = false;
                if (k < 11)
                {
                    eastC = (roomTemp[k + 1,l].type != RoomTypes.EMPTY) && (roomTemp[k,l].type != RoomTypes.EMPTY);
                }
                if (l < 11)
                {
                    southC = (roomTemp[k,l + 1].type != RoomTypes.EMPTY) && (roomTemp[k,l].type != RoomTypes.EMPTY);
                }
                Node3D d; //doors
                if (eastC)
                {
                    d = (Node3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/Doors/DoorLCZ.tscn").Instantiate();
                    d.Position = new Vector3(k * 20.48f + 10.24f, 0, l*20.48f);
                    d.RotationDegrees = new Vector3(0, 90, 0);
                    AddChild(d, true);
                }
                if (southC)
                {
                    d = (Node3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/Doors/DoorLCZ.tscn").Instantiate();
                    d.Position = new Vector3(k * 20.48f, 0, l*20.48f + 10.24f);
                    d.RotationDegrees = new Vector3(0, 0, 0);
                    AddChild(d, true);
                }
            }
        }
    }

    public override void _Ready()
    {
        if (Multiplayer.IsServer())
        {
            Seed = RandomSeed();
        }
        CreateMap();
    }
}