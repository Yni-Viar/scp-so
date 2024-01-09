using Godot;
using System;
/// <summary>
/// MapGen main algorithm.
/// </summary>
public partial class MapGenerator : Node
{
    //(c) juanjp600. License - CC-BY-SA 3.0.
    [Export] ulong seed;
    internal ulong Seed { get => seed; set => seed = value; }
    [Export] bool generateMoreR1s = false;
    [Export] bool generateMoreR4s = false;
    [Export] int zone;
    [Export] bool enableDoors;
    [Export] string doorPrefabPath = "res://MapGen/Resources/Doors/DoorLCZ.tscn";

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    internal ulong RandomSeed()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random.Randi();
    }
    /// <summary>
    /// Type of room
    /// </summary>
    public enum RoomTypes { ROOM1, ROOM2, ROOM2C, ROOM3, ROOM4, EMPTY };
    int room1Amount, room2Amount, room2cAmount, room3Amount, room4Amount;
    internal struct TempRoom
    {
        internal RoomTypes type;
        /* angle can be:
           * -1: do not spawn a room
           * 0: means 0 rotation; facing east
           * 1: means 90 rotation; facing north
           * 2: means 180 rotation; facing west
           * 3: means 270 rotation; facing south
        */
        internal float angle;
        internal string roomName;
    };

    internal TempRoom[,] roomTemp = new TempRoom[12, 12];
    /// <summary>
    /// Main algorithm
    /// </summary>
    internal virtual void CreateMap()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Seed = seed;
        int x, y, temp;
        int x2, y2;
        int width, height;

        for (x = 0; x < 12; x++)
        {
            for (y = 0; y < 12; y++)
            {
                roomTemp[x, y].type = RoomTypes.ROOM1; //fill the data with values
                roomTemp[x, y].angle = -1;
            }
        }

        x = 6;
        y = 11;

        /*for (int i = y; i < 16; i++)
        {
            roomTemp[x, i].angle = 0; //fill angles
        }*/

        while (y >= 2)
        {
            width = (rand.RandiRange(0, 2)) + 6; //map width

            if (x > 8)
            {
                width = -width;
            }
            else if (x > 6)
            {
                x = x - 6;
            }

            //make sure the hallway doesn't go outside the array
            if (x + width > 10)
            {
                width = 10 - x;
            }
            else if (x + width < 2)
            {
                width = -x + 2;
            }

            x = Mathf.Min(x, (x + width));
            width = Mathf.Abs(width);
            for (int i = x; i <= x + width; i++)
            {
                roomTemp[Mathf.Min(i, 11), y].angle = 0;
            }

            //height is random
            height = (rand.RandiRange(0, 1)) + 2;
            if (y - height < 1) height = y;
            //height for each zone
            int yhallways = (rand.RandiRange(0, 1)) + 1;

            //this FOR loop cleans the mapgen from loops.
            for (int i = 1; i <= yhallways; i++)
            {
                x2 = Mathf.Max(Mathf.Min((rand.RandiRange(0, width - 2)) + x, 8), 2);
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
        for (x = 0; x < 12; x++)
        {
            for (y = 0; y < 12; y++)
            {
                bool hasNorth, hasSouth, hasEast, hasWest;
                hasNorth = hasSouth = hasEast = hasWest = false;
                if (roomTemp[x, y].angle == 0)
                { //this is not a checkpoint room
                    if (x > 0)
                    {
                        hasWest = (roomTemp[x - 1, y].angle > -1);
                    }
                    if (x < 11)
                    {
                        hasEast = (roomTemp[x + 1, y].angle > -1);
                    }
                    if (y > 0)
                    {
                        hasNorth = (roomTemp[x, y - 1].angle > -1);
                    }
                    if (y < 11)
                    {
                        hasSouth = (roomTemp[x, y + 1].angle > -1);
                    }
                    if (hasNorth && hasSouth)
                    {
                        if (hasEast && hasWest)
                        { //room4
                            float[] avAngle = new float[] { 0, 90, 180, 270 };
                            roomTemp[x, y].type = RoomTypes.ROOM4;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 3)];
                            room4Amount++;
                        }
                        else if (hasEast && !hasWest)
                        { //room3, pointing east
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 90;
                            room3Amount++;
                        }
                        else if (!hasEast && hasWest)
                        { //room3, pointing west
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 270;
                            room3Amount++;
                        }
                        else
                        { //vertical room2
                            float[] avAngle = new float[] { 0, 180 };
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                            room2Amount++;
                        }
                    }
                    else if (hasEast && hasWest)
                    {
                        if (hasNorth && !hasSouth)
                        { //room3, pointing north
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 180;
                            room3Amount++;
                        }
                        else if (!hasNorth && hasSouth)
                        { //room3, pointing south
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 0;
                            room3Amount++;
                        }
                        else
                        { //horizontal room2
                            float[] avAngle = new float[] { 90, 270 };
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                            room2Amount++;
                        }
                    }
                    else if (hasNorth)
                    {
                        if (hasEast)
                        { //room2c, north-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 90;
                            room2cAmount++;
                        }
                        else if (hasWest)
                        { //room2c, north-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 180;
                            room2cAmount++;
                        }
                        else
                        { //room1, north
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 180;
                            room1Amount++;
                        }
                    }
                    else if (hasSouth)
                    {
                        if (hasEast)
                        { //room2c, south-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 0;
                            room2cAmount++;
                        }
                        else if (hasWest)
                        { //room2c, south-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 270;
                            room2cAmount++;
                        }
                        else
                        { //room1, south
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 0;
                            room1Amount++;
                        }
                    }
                    else if (hasEast)
                    { //room1, east
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 90;
                        room1Amount++;
                    }
                    else
                    { //room1, west
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 270;
                        room1Amount++;
                    }
                }
                else
                {
                    roomTemp[x, y].type = RoomTypes.EMPTY;
                }
            }
        }

        if (room1Amount < 5 && generateMoreR1s)
        {
            GD.Print("Forcing some ROOM1s");
            for (y = 2; y < 10 && room1Amount < 5; y++)
            {
                //if (getZone(y+2) == i && getZone(y-2) == i) {
                for (x = 2; x < 10 && room1Amount < 5; x++)
                {
                    if (roomTemp[x, y].angle < 0)
                    {
                        bool freeSpace = ((roomTemp[x + 1, y].angle >= 0) != (roomTemp[x - 1, y].angle >= 0)) != ((roomTemp[x, y + 1].angle >= 0) != (roomTemp[x, y - 1].angle >= 0));
                        freeSpace = freeSpace && (((roomTemp[x + 2, y].angle >= 0) != (roomTemp[x - 2, y].angle >= 0)) != ((roomTemp[x, y + 2].angle >= 0) != (roomTemp[x, y - 2].angle >= 0)));
                        freeSpace = freeSpace && (((roomTemp[x + 1, y + 1].angle >= 0) != (roomTemp[x - 1, y - 1].angle >= 0)) != ((roomTemp[x - 1, y + 1].angle >= 0) != (roomTemp[x + 1, y - 1].angle >= 0)));
                        if (freeSpace)
                        {
                            TempRoom adjRoom;
                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x + 1, y];
                                roomTemp[x, y].angle = 90;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x - 1, y];
                                roomTemp[x, y].angle = 270;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                adjRoom = roomTemp[x, y + 1];
                                roomTemp[x, y].angle = 0;
                            }
                            else
                            {
                                adjRoom = roomTemp[x, y - 1];
                                roomTemp[x, y].angle = 180;
                            }

                            switch (adjRoom.type)
                            {
                                /*case RoomTypes.ROOM2:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    room1Amount++;
                                    room2Amount--;
                                    room3Amount++;
                                    adjRoom.type = RoomTypes.ROOM3;
                                    switch (roomTemp[x, y].angle)
                                    {
                                        case 0:
                                            adjRoom.angle = 0;
                                            break;
                                        case 1:
                                            adjRoom.angle = 90;
                                            break;
                                        case 2:
                                            adjRoom.angle = 180;
                                            break;
                                        case 3:
                                            adjRoom.angle = 270;
                                            break;
                                    }
                                    break;*/
                                case RoomTypes.ROOM3:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    adjRoom.type = RoomTypes.ROOM4;
                                    room1Amount++;
                                    room3Amount--;
                                    room4Amount++;
                                    break;
                                default:
                                    roomTemp[x, y].angle = -1;
                                    break;
                            }

                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                roomTemp[x + 1, y] = adjRoom;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                roomTemp[x - 1, y] = adjRoom;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                roomTemp[x, y + 1] = adjRoom;
                            }
                            else
                            {
                                roomTemp[x, y - 1] = adjRoom;
                            }
                        }
                    }
                }
                //\}
            }
        }

        if (room4Amount < 3 && generateMoreR4s)
        {
            GD.Print("Forcing some ROOM4s\n");
            for (y = 2; y < 10 && room4Amount < 3; y++)
            {
                //if (getZone(y+2) == i && getZone(y-2) == i) {
                for (x = 2; x < 10 && room4Amount < 3; x++)
                {
                    if (roomTemp[x, y].angle < 0)
                    {
                        bool freeSpace = ((roomTemp[x + 1, y].angle >= 0) != (roomTemp[x - 1, y].angle >= 0)) != ((roomTemp[x, y + 1].angle >= 0) != (roomTemp[x, y - 1].angle >= 0));
                        freeSpace = freeSpace && (((roomTemp[x + 2, y].angle >= 0) != (roomTemp[x - 2, y].angle >= 0)) != ((roomTemp[x, y + 2].angle >= 0) != (roomTemp[x, y - 2].angle >= 0)));
                        freeSpace = freeSpace && (((roomTemp[x + 1, y + 1].angle >= 0) != (roomTemp[x - 1, y - 1].angle >= 0)) != ((roomTemp[x - 1, y + 1].angle >= 0) != (roomTemp[x + 1, y - 1].angle >= 0)));
                        if (freeSpace)
                        {
                            TempRoom adjRoom;

                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x + 1, y];
                                roomTemp[x, y].angle = 3;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x - 1, y];
                                roomTemp[x, y].angle = 1;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                adjRoom = roomTemp[x, y + 1];
                                roomTemp[x, y].angle = 2;
                            }
                            else
                            {
                                adjRoom = roomTemp[x, y - 1];
                                roomTemp[x, y].angle = 0;
                            }

                            switch (adjRoom.type)
                            {
                                case RoomTypes.ROOM3:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    adjRoom.type = RoomTypes.ROOM4;
                                    room1Amount++;
                                    room3Amount--;
                                    room4Amount++;
                                    break;
                                default:
                                    roomTemp[x, y].angle = -1;
                                    break;
                            }

                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                roomTemp[x + 1, y] = adjRoom;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                roomTemp[x - 1, y] = adjRoom;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                roomTemp[x, y + 1] = adjRoom;
                            }
                            else
                            {
                                roomTemp[x, y - 1] = adjRoom;
                            }
                        }
                    }
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
        int currRoom1 = 0;
        int currRoom2 = 0;
        int currRoom2C = 0;
        int currRoom3 = 0;
        int currRoom4 = 0;

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                StaticBody3D rm;
                switch (roomTemp[i, j].type)
                {
                    case RoomTypes.ROOM1:
                        if (currRoom1 < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room1.Count)
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room1[currRoom1].Instantiate<StaticBody3D>();
                        }
                        else
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms1[rand.RandiRange(0, GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms1.Count - 1)].Instantiate<StaticBody3D>();
                        }
                        currRoom1++;
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM2:
                        if (currRoom2 < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room2.Count)
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room2[currRoom2].Instantiate<StaticBody3D>();
                        }
                        else
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms2[rand.RandiRange(0, GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms2.Count - 1)].Instantiate<StaticBody3D>();
                        }
                        currRoom2++;
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM2C:
                        if (currRoom2C < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room2C.Count)
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room2C[currRoom2C].Instantiate<StaticBody3D>();
                        }
                        else
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms2C[rand.RandiRange(0, GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms2C.Count - 1)].Instantiate<StaticBody3D>();
                        }
                        currRoom2C++;
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM3:
                        if (currRoom3 < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room3.Count)
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room3[currRoom3].Instantiate<StaticBody3D>();
                        }
                        else
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms3[rand.RandiRange(0, GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms3.Count - 1)].Instantiate<StaticBody3D>();
                        }
                        currRoom3++;
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                    case RoomTypes.ROOM4:
                        if (currRoom4 < GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room4.Count)
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Room4[currRoom4].Instantiate<StaticBody3D>();
                        }
                        else
                        {
                            rm = GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms4[rand.RandiRange(0, GetTree().Root.GetNode<FacilityManager>("Main/Game/").data.MapGenRooms[zone].Rooms4.Count - 1)].Instantiate<StaticBody3D>();
                        }
                        currRoom4++;
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm, true);
                        roomTemp[i, j].roomName = rm.Name;
                        break;
                }
            }
        }

        if (enableDoors)
        {
            for (int k = 0; k < 12; k++)
            {
                for (int l = 0; l < 12; l++)
                {
                    bool southC, eastC;
                    southC = eastC = false;
                    if (k < 11)
                    {
                        eastC = (roomTemp[k + 1, l].type != RoomTypes.EMPTY) && (roomTemp[k, l].type != RoomTypes.EMPTY);
                    }
                    if (l < 11)
                    {
                        southC = (roomTemp[k, l + 1].type != RoomTypes.EMPTY) && (roomTemp[k, l].type != RoomTypes.EMPTY);
                    }
                    Node3D d; //doors
                    if (eastC)
                    {
                        d = (Node3D)ResourceLoader.Load<PackedScene>(doorPrefabPath).Instantiate();
                        d.Position = new Vector3(k * 20.48f + 10.24f, 0, l * 20.48f);
                        d.RotationDegrees = new Vector3(0, 90, 0);
                        AddChild(d, true);
                    }
                    if (southC)
                    {
                        d = (Node3D)ResourceLoader.Load<PackedScene>(doorPrefabPath).Instantiate();
                        d.Position = new Vector3(k * 20.48f, 0, l * 20.48f + 10.24f);
                        d.RotationDegrees = new Vector3(0, 0, 0);
                        AddChild(d, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get map data.
    /// </summary>
    /// <returns>array with all information about map</returns>
    internal string[][] GetMapData()
    {
        string[][] tmp = new string[144][];
        int i = 0;
        for (int k = 0; k < 12; k++)
        {
            for (int l = 0; l < 12; l++)
            {
                tmp[i] = new string[2];
                switch (roomTemp[k, l].type)
                {
                    case RoomTypes.ROOM1:
                        tmp[i][0] = "room1_";
                        break;
                    case RoomTypes.ROOM2:
                        tmp[i][0] = "room2_";
                        break;
                    case RoomTypes.ROOM2C:
                        tmp[i][0] = "room2c_";
                        break;
                    case RoomTypes.ROOM3:
                        tmp[i][0] = "room3_";
                        break;
                    case RoomTypes.ROOM4:
                        tmp[i][0] = "room4_";
                        break;
                    default:
                        tmp[i][0] = "empty";
                        tmp[i][1] = "empty";
                        break;
                }
                switch (roomTemp[k, l].angle)
                {
                    case 0f:
                        tmp[i][0] += "0";
                        break;
                    case 90f:
                        tmp[i][0] += "90";
                        break;
                    case 180f:
                        tmp[i][0] += "180";
                        break;
                    case 270f:
                        tmp[i][0] += "270";
                        break;
                }
                tmp[i][1] = roomTemp[k, l].roomName;
                i++;
            }
        }
        return tmp;
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
