﻿using Godot;
using System;

public partial class MapGenerator : Node
{
    //(c) juanjp600. License - CC-BY-SA 3.0.
    [Export] ulong seed;
    ulong Seed { get=> seed; set=>seed=value; }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    ulong RandomSeed()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random.Randi();
    }


    internal enum RoomTypes { ROOM1, ROOM2, ROOM2C, ROOM3, ROOM4, EMPTY };
    int room1Amount, room2Amount, room2cAmount, room3Amount, room4Amount;
    internal struct TempRoom
    {
        internal RoomTypes type;
        /* angle can be:
		   * -1: do not spawn a room
		   * 0: means 0° rotation; facing east
		   * 1: means 90° rotation; facing north
		   * 2: means 180° rotation; facing west
		   * 3: means 270° rotation; facing south
		*/
        internal float angle;
    };
    void CreateMap()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Seed = seed;
        int x, y, temp;
        int x2, y2;
        int width, height;

        TempRoom[,] roomTemp = new TempRoom[20, 20];

        for (x = 0; x < 20; x++)
        {
            for (y = 0; y < 20; y++)
            {
                //roomArray[x][y] = nullptr; - does not work, but is not necessary
                roomTemp[x, y].type = RoomTypes.ROOM1; //fill the data with values
                roomTemp[x, y].angle = -1;
            }
        }

        x = 10;
        y = 18;

        for (int i = y; i < 20; i++)
        {
            roomTemp[x, i].angle = 0; //fill angles
        }

        while (y >= 2)
        {
            width = (rand.RandiRange(0, 5)) + 10; //map width

            if (x > 12)
            {
                width = -width;
            }
            else if (x > 8)
            {
                x = x - 10;
            }

            //make sure the hallway doesn't go outside the array
            if (x + width > 17)
            {
                width = 17 - x;
            }
            else if (x + width < 2)
            {
                width = -x + 2;
            }

            x = Math.Min(x, (x + width));
            width = Math.Abs(width);
            for (int i = x; i <= x + width; i++)
            {
                roomTemp[Math.Min(i, 19), y].angle = 0;
            }

            //height is random
            height = (rand.RandiRange(0, 1)) + 3;
            if (y - height < 1) height = y;
            //height for each zone
            int yhallways = (rand.RandiRange(0, 1)) + 4;

            for (int i = 1; i <= yhallways; i++)
            {
                x2 = Math.Max(Math.Min((rand.RandiRange(0, width - 2)) + x, 18), 2);
                while (roomTemp[x2, y - 1].angle >= 0 || roomTemp[x2 - 1, y - 1].angle >= 0 || roomTemp[x2 + 1, y - 1].angle >= 0)
                {
                    x2++;
                }

                if (x2 < x + width)
                {
                    int tempheight;
                    if (i == 1)
                    {
                        tempheight = height;
                        if (rand.RandiRange(0, 1) == 0) x2 = x; else x2 = x + width;
                    }
                    else
                    {
                        tempheight = (rand.RandiRange(0, height - 1)) + 1;
                    }

                    for (y2 = y - tempheight; y2 <= y; y2++)
                    {
                        roomTemp[x2, y2].angle = 0;
                    }

                    if (tempheight == height) temp = x2;
                }
            }

            y -= height;
        }

        room1Amount = room2Amount = room2cAmount = room3Amount = room4Amount = 0;
        for (x = 0; x < 20; x++)
        {
            for (y = 0; y < 20; y++)
            {
                bool hasNorth, hasSouth, hasEast, hasWest;
                hasNorth = hasSouth = hasEast = hasWest = false;
                if (roomTemp[x,y].angle == 0)
                { //this is not a checkpoint room
                    if (x > 0)
                    {
                        hasWest = (roomTemp[x - 1,y].angle > -1);
                    }
                    if (x < 19)
                    {
                        hasEast = (roomTemp[x + 1,y].angle > -1);
                    }
                    if (y > 0)
                    {
                        hasNorth = (roomTemp[x,y - 1].angle > -1);
                    }
                    if (y < 19)
                    {
                        hasSouth = (roomTemp[x,y + 1].angle > -1);
                    }
                    if (hasNorth && hasSouth)
                    {
                        if (hasEast && hasWest)
                        { //room4
                            float[] avAngle = new float[] {0, 90, 180, 270};
                            roomTemp[x,y].type = RoomTypes.ROOM4;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 3)];
                        }
                        else if (hasEast && !hasWest)
                        { //room3, pointing east
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x,y].angle = 90;
                        }
                        else if (!hasEast && hasWest)
                        { //room3, pointing west
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x,y].angle = 270;
                        }
                        else
                        { //vertical room2
                            float[] avAngle = new float[] {0, 180};
                            roomTemp[x,y].type = RoomTypes.ROOM2;
                            roomTemp[x,y].angle = avAngle[rand.RandiRange(0, 1)];
                        }
                    }
                    else if (hasEast && hasWest)
                    {
                        if (hasNorth && !hasSouth)
                        { //room3, pointing north
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 180;
                        }
                        else if (!hasNorth && hasSouth)
                        { //room3, pointing south
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 0;
                        }
                        else
                        { //horizontal room2
                            float[] avAngle = new float[] {90, 270};
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                        }
                    }
                    else if (hasNorth)
                    {
                        if (hasEast)
                        { //room2c, north-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 90;
                        }
                        else if (hasWest)
                        { //room2c, north-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 180;
                        }
                        else
                        { //room1, north
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 180;
                        }
                    }
                    else if (hasSouth)
                    {
                        if (hasEast)
                        { //room2c, south-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 0;
                        }
                        else if (hasWest)
                        { //room2c, south-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 270;
                        }
                        else
                        { //room1, south
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 0;
                        }
                    }
                    else if (hasEast)
                    { //room1, east
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 90;
                    }
                    else
                    { //room1, west
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 270;
                    }
                }
                else
                {
                    roomTemp[x, y].type = RoomTypes.EMPTY;
                }
            }
        }

        /*for (x = 0; x < roomTemp.GetLength(0); x++)
        {
            for (y = 0; y < roomTemp.GetLength(1); y++)
            {
                switch (roomTemp[y, x].angle)
                {
                    case -1:
                        Console.Write("#" + " ");
                        break;
                    default:
                        Console.Write((int)roomTemp[y, x].type + " ");
                        break;
                }
                
            }
            Console.WriteLine();
        }*/

        int Hcz1 = 0; //Hcz/lcz/rz is zone where room will be spawned, 1/2 - type of rooms
        int Hcz2 = 0;
        int Lcz1 = 0;
        int Lcz2 = 0;
        // int Rz1 = 0;
        int Rz2 = 0;
        // int Rz2c = 0;
        int zone = 0; //0 is HCZ, 1 is LCZ, 2 is RZ.
        
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (j < 7)
                {
                    zone = 0;
                }
                if (j >= 7 && j < 14)
                {
                    zone = 1;
                }
                if (j >= 14 && j < 20)
                {
                    zone = 2;
                }
                StaticBody3D rm;
                switch (roomTemp[i, j].type)
                {
                    case RoomTypes.ROOM1:
                        switch (zone)
                        {
                            case 0:
                                switch (Hcz1)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/hc_cont_1_173.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Hcz1++;
                                        break;
                                    case 1:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/hc_cont_1_049.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Hcz1++;
                                        break;
                                    default:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/hc_room_1_endroom.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        break;
                                }
                                break;
                            case 1:
                                switch (Lcz1)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_cont_1_079.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz1++;
                                        break;
                                    case 1:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_archive.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz1++;
                                        break;
                                    default:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_endroom.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        break;
                                }
                                break;
                            case 2:
                                /*switch (currRoom1)
                                {
                                    default:*/
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_endroom.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        // break;
                                // }
                                break;
                        }
                        
                        break;
                    case RoomTypes.ROOM2:
                        switch (zone)
                        {
                            case 0:
                                switch (Hcz2)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/hc_room_2_nuke.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Hcz2++;
                                        break;
                                    case 1:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/hc_cont_2_testroom.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Hcz2++;
                                        break;
                                    default:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/hc_room_2.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        break;
                                }
                                break;
                            case 1:
                                switch (Lcz2)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_cont_2_650.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz2++;
                                        break;
                                    case 1:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_cont_2_012.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz2++;
                                        break;
                                    case 2:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2_vent.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz2++;
                                        break;
                                    case 3:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2_sl.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Lcz2++;
                                        break;
                                    default:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        break;
                                }
                                break;
                            case 2:
                                switch (Rz2)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_offices.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    case 1:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_offices_2.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    case 2:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_poffices.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    case 3:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_toilets.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    case 4:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_medibay.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    case 5:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2_servers.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2++;
                                        break;
                                    default:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/rz_room_2.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        break;
                                        }
                                        break;
                                }
                        
                        break;
                    case RoomTypes.ROOM2C:
                        switch (zone)
                        {
                            case 0:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/hc_room_2c.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 1:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/lc_room_2c.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 2:
                                /*switch (Rz2c)
                                {
                                    case 0:
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/rz_room_2c_ec.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        Rz2c++;
                                        break;
                                    default:*/
                                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/rz_room_2c.tscn").Instantiate();
                                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                        AddChild(rm);
                                        // break;
                                // }
                                break;
                        }
                        
                        break;
                    case RoomTypes.ROOM3:
                        switch (zone)
                        {
                            case 0:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/hc_room_3.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 1:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/lc_room_3.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 2:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/rz_room_3.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                        }
                        break;
                    case RoomTypes.ROOM4:
                        switch (zone)
                        {
                            case 0:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/hc_room_4.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 1:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/lc_room_4.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                            case 2:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/rz_room_4.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                        }
                        break;
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