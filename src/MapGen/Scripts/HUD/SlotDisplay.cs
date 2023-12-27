using Godot;
using System;

public partial class SlotDisplay : CenterContainer
{
    string whichZone = "";
    bool itemHover = false;
    int index;
    TextureRect mapData = new();

    Godot.Collections.Dictionary<string, string> imagePath = new Godot.Collections.Dictionary<string, string>
    { // so twisted...
        {"room1_0", "res://Assets/HUD/MapGen/room1_90.png"},
        {"room1_90", "res://Assets/HUD/MapGen/room1.png"},
        {"room1_180", "res://Assets/HUD/MapGen/room1_270.png"},
        {"room1_270", "res://Assets/HUD/MapGen/room1_180.png"},
        {"room2_0", "res://Assets/HUD/MapGen/room2_90.png"},
        {"room2_90", "res://Assets/HUD/MapGen/room2.png"},
        {"room2_180", "res://Assets/HUD/MapGen/room2_90.png"},
        {"room2_270", "res://Assets/HUD/MapGen/room2.png"},
        {"room2c_0", "res://Assets/HUD/MapGen/room2C.png"},
        {"room2c_90", "res://Assets/HUD/MapGen/room2C_270.png"},
        {"room2c_180", "res://Assets/HUD/MapGen/room2C_180.png"},
        {"room2c_270", "res://Assets/HUD/MapGen/room2C_90.png"},
        {"room3_0", "res://Assets/HUD/MapGen/room3_90.png"},
        {"room3_90", "res://Assets/HUD/MapGen/room3.png"},
        {"room3_180", "res://Assets/HUD/MapGen/room3_270.png"},
        {"room3_270","res://Assets/HUD/MapGen/room3_180.png"},
        {"room4_0", "res://Assets/HUD/MapGen/room4.png"},
        {"room4_90", "res://Assets/HUD/MapGen/room4.png"},
        {"room4_180", "res://Assets/HUD/MapGen/room4.png"},
        {"room4_270", "res://Assets/HUD/MapGen/room4.png"},
    };
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("fire") && itemHover && mapData.Name != "empty")
		{
            if (GetParent().GetParent().GetParent().GetParentOrNull<ComputerPlayerScript>() != null)
            {
                if (GetParent().GetParent().GetParent().GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
                {
                    GetParent().GetParent().GetParent().GetParent<ComputerPlayerScript>().SwitchCamera(whichZone, mapData.Name);
                }
            }
        }
	}
    /// <summary>
    /// Displays data in the slot.
    /// </summary>
    /// <param name="type">Room type and angle</param>
    /// <param name="name">Room name</param>
    /// <param name="zone">Current zone</param>
    internal void DisplayData(string type, string name, string zone)
    {
        whichZone = zone;
        if (name != "empty" && name != null && type != "empty" && type != null)
        {
            mapData.Texture = ResourceLoader.Load<Texture2D>(imagePath[type]);
            mapData.Name = name;
            mapData.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(mapData);
        }
    }

    private void OnFakeButtonMouseEntered()
    {
        itemHover = true;
    }


    private void OnFakeButtonMouseExited()
    {
        itemHover = false;
    }
}



