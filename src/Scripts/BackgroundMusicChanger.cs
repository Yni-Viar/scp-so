using Godot;
using System;

public partial class BackgroundMusicChanger : Area3D
{
    [Export] string inRoomMusic;
    [Export] string outsideMusic;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerScript player)
        {
            player.customMusic = true;
            player.GetParent<FacilityManager>().SetBackgroundMusic(inRoomMusic);
        }
    }


    private void OnBodyExited(Node3D body)
    {
        if (body is PlayerScript player)
        {
            player.customMusic = false;
            player.GetParent<FacilityManager>().SetBackgroundMusic(outsideMusic);
        }
    }
}