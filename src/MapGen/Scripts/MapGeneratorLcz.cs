using Godot;
using System;
/// <summary>
/// LCZ-special generator parameters
/// </summary>
public partial class MapGeneratorLcz : MapGenerator
{
    internal override void CreateMap()
    {
        base.CreateMap();
        RandomNumberGenerator rng = new RandomNumberGenerator();
        bool checkpointSpawned = false;
        string selectedRoom;
        int currRoom1 = 0;
        int currRoom2 = 0;
        // you can add more vars for different needs, e.g. currRoom3 or currRoom2c
        /* old 0.7.x code. Use MapGenerator.cs instead.
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
                            rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_ckpt_1.tscn").Instantiate();
                            rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                            rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                            AddChild(rm, true);
                            roomTemp[i, j].roomName = rm.Name;
                            checkpointSpawned = true;
                            break;
                        }
                        else
                        {
                            if (currRoom1 >= RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczSingle1"].Count)
                            {
                                selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczCommon1"][rng.RandiRange(0, RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczCommon1"].Count - 1)];
                            }
                            else
                            {
                                selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczSingle1"][currRoom1];
                            }

                            currRoom1++;
                            rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/" + selectedRoom + ".tscn").Instantiate();
                            rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                            rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                            AddChild(rm, true);
                            roomTemp[i, j].roomName = rm.Name;
                        }

                        break;
                    case RoomTypes.ROOM2:
                        if (currRoom2 >= RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczSingle2"].Count)
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczCommon2"][rng.RandiRange(0, RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczCommon2"].Count - 1)];
                        }
                        else
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms_" + Globals.roomsCompatibility + ".json")["LczSingle2"][currRoom2];
                        }
                        currRoom2++;
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/" + selectedRoom + ".tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM2C:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/lc_room_2c.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM3:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/lc_room_3.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM4:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/lc_room_4.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                }
            }
        }
        */
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