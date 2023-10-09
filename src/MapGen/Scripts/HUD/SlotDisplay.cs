using Godot;
using System;

public partial class SlotDisplay : CenterContainer
{
    string whichZone = "";
    bool itemHover = false;
    int index;
    TextureRect mapData = new();
    string[] imagePath = new string[] {
            "res://Assets/HUD/MapGen/room1.png", 
            "res://Assets/HUD/MapGen/room2.png", 
            "res://Assets/HUD/MapGen/room2C.png",
            "res://Assets/HUD/MapGen/room3.png", 
            "res://Assets/HUD/MapGen/room4.png"
            };
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("fire") && itemHover && mapData.Name != "empty" && mapData.Name != "LC_room1_endroom")
		{
            if (GetParent().GetParent().GetParent().GetParentOrNull<Scp079PlayerScript>() != null)
            {
                if (GetParent().GetParent().GetParent().GetParent().GetParent().GetParent<PlayerScript>().IsMultiplayerAuthority())
                {
                    GetParent().GetParent().GetParent().GetParent<Scp079PlayerScript>().SwitchCamera(whichZone, mapData.Name);
                }
            }
        }
	}

    internal void DisplayData(string type, string rotation, string name, string zone)
    {
        whichZone = zone;
        if (name != "empty" && name != null && type != "empty" && type != null && rotation != null)
        {
            mapData.Texture = ResourceLoader.Load<Texture2D>(imagePath[int.Parse(type)]);
            mapData.RotationDegrees = float.Parse(rotation);
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



