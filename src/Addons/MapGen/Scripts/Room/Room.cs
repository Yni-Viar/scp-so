using Godot;
using System;

//Made by Yni (License CC-BY-SA)

public partial class Room : Node3D
{
    int zone;
    //MapGenerator.RoomType type;
    string roomName;
    public Resource roomData;

    public PackedScene GetRoomMesh(string roomName, MapGenerator.RoomType type, int zone)
    {
        //spawn usual room
        if (string.IsNullOrEmpty(roomName))
        {
            if (type == MapGenerator.RoomType.ROOM1)
            {
                switch (zone)
                {
                    case 3:
                        //HCZ
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/hc_room_1_endroom.tscn");
                    default:
                        // both Research Zone and Light Containment Zone has the same endroom...
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/lc_room_1_endroom.tscn");
                }
            }
            else if (type == MapGenerator.RoomType.ROOM2)
            {
                switch (zone)
                {
                    //RZ
                    case 1:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2.tscn");
                    case 3:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/hc_room_2.tscn");
                    default: //LCZ is by-default
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_room_2.tscn");
                }
            }
            else if (type == MapGenerator.RoomType.ROOM2C)
            {
                switch (zone)
                {
                    case 1:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2C/rz_room_2c.tscn");
                    case 3:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2C/hc_room_2c.tscn");
                    default:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2C/lc_room_2c.tscn");
                }
            }
            else if (type == MapGenerator.RoomType.ROOM3)
            {
                switch (zone)
                {
                    case 1:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM3/rz_room_3.tscn");
                    case 3:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM3/hc_room_3.tscn");
                    default:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM3/lc_room_3.tscn");
                }
            }
            else if (type == MapGenerator.RoomType.ROOM4)
            {
                switch (zone)
                {
                    case 1:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM4/rz_room_4.tscn");
                    case 3:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM4/hc_room_4.tscn");
                    default:
                        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM4/lc_room_4.tscn");
                }
            }
            else
            {
                GD.Print("There is no room name and no type");
                return null;
            }
        }
        else
        {
            //and there, switch-case of the special rooms.
            switch (roomName)
            {
                case "checkpoint1":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/ck_rz_lcz.tscn");
                case "checkpoint2":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_room_2.tscn");
                case "RZ_exit1_gatea":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/rz_exit_1_gatea.tscn");
                case "RZ_exit1_gateb":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/rz_exit_1_gateb.tscn");
                case "RZ_room2_offices":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_offices.tscn");
                case "RZ_room2_offices2":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_offices_2.tscn");
                case "RZ_room2_poffices":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_poffices.tscn");
                case "RZ_room2_servers":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_servers.tscn");
                case "RZ_room2_medibay":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_medibay.tscn");
                case "RZ_room2_toilets":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/rz_room_2_toilets.tscn");
                case "LC_room1_archive":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/lc_room_1_archive.tscn");
                case "LC_cont1_049":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/lc_cont_1_049.tscn");
                case "LC_cont1_079":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/lc_cont_1_079.tscn");
                case "LC_cont2_012":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_cont_2_012.tscn");
                case "LC_cont2_650":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_cont_2_650.tscn");
                case "LC_room2_hall":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_room_2_hall.tscn");
                case "LC_room2_sl":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_room_2_sl.tscn");
                case "LC_room2_vent":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/lc_room_2_vent.tscn");
                case "HC_cont1_173":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/hc_cont_1_173.tscn");
                case "HC_cont1_049":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM1/hc_cont_1_049.tscn");
                case "HC_cont2_testroom":
                    return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM2/hc_cont_2_testroom.tscn");
                default:
                    GD.Print("Could not load an asset");
                    return null;
            }
            
        }
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
