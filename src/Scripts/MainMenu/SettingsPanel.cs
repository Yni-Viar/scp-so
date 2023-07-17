using Godot;
using System;

public partial class SettingsPanel : Panel
{
	public static bool HQSetting;
	public static bool MusicSetting;
	public static bool FullscreenSetting;
    public static Vector2I WindowSizeSetting = new Vector2I((int)ProjectSettings.GetSetting("display/window/size/width"), (int)ProjectSettings.GetSetting("display/window/size/height"));
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (FileAccess.FileExists("user://settings_0.1.0.ini"))
		{
			LoadIni();
		}
		else
		{
			IniSaver ini = new IniSaver();
			ini.SaveIni("Settings", new Godot.Collections.Array<string>{
				"HQSetting",
				"MusicSetting",
				"FullscreenSetting",
                "WindowSizeSetting"
			}, new Godot.Collections.Array{
				true,
				true,
				false,
                new Vector2(1920, 1080)
			}, "user://settings_0.1.0.ini");

			LoadIni();
		}

		if (HQSetting)
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/HQSet").ButtonPressed = true;
		}
		else
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/HQSet").ButtonPressed = false;
		}

		if (MusicSetting)
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/MusicSet").ButtonPressed = true;
		}
		else
		{
			GetNode<CheckButton>("ScrollContainer/VBoxContainer/MusicSet").ButtonPressed = false;
		}

        if (FullscreenSetting)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void OnBackPressed()
	{
		GetParent().GetParent().GetNode<Control>("Settings").Hide();
	}

	private void OnHqSetToggled(bool button_pressed)
	{
		if (button_pressed)
		{
			HQSetting = true;
		}
		else
		{
			HQSetting = false;
		}
	}


	private void OnMusicSetToggled(bool button_pressed)
	{
		if (button_pressed)
		{
			MusicSetting = true;
		}
		else
		{
			MusicSetting = false;
		}
	}
	private void OnSavePressed()
	{
		IniSaver ini = new IniSaver();
		ini.SaveIni("Settings", new Godot.Collections.Array<string>{
			"HQSetting",
			"MusicSetting",
			"FullscreenSetting",
            "WindowSizeSetting"
		}, new Godot.Collections.Array{
			HQSetting,
			MusicSetting,
			FullscreenSetting,
            WindowSizeSetting
		}, "user://settings_0.1.0.ini");

		LoadIni();
	}

	
	private void OnFullscreenSetToggled(bool button_pressed)
	{
		if (button_pressed)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            FullscreenSetting = true;
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            FullscreenSetting = false;
		}
	}
	
	private void OnWindowSizeSetItemSelected(long index)
    {
        switch (index)
        {
            case 0:
                WindowSizeSetting = new Vector2I(1920, 1080);
                break;
            case 1:
                WindowSizeSetting = new Vector2I(1600, 900);
                break;
            case 2:
                WindowSizeSetting = new Vector2I(1366, 768);
                break;
            case 3:
                WindowSizeSetting = new Vector2I(1280, 720);
                break;
            case 4:
                WindowSizeSetting = new Vector2I(1024, 768);
                break;
            case 5:
                WindowSizeSetting = new Vector2I(800, 600);
                break;
        }

        DisplayServer.WindowSetSize(WindowSizeSetting);
    }

	public static void LoadIni()
	{
		var config = new ConfigFile();

		// Load data from a file.
		Error err = config.Load("user://settings_0.1.0.ini");

		// If the file didn't load, ignore it.
		if (err != Error.Ok)
		{
			return;
		}
		// Fetch the data for each section.
		SettingsPanel.HQSetting = (bool)config.GetValue("Settings", "HQSetting");
		SettingsPanel.MusicSetting = (bool)config.GetValue("Settings", "MusicSetting");
		SettingsPanel.FullscreenSetting = (bool)config.GetValue("Settings", "FullscreenSetting");
        SettingsPanel.WindowSizeSetting = (Vector2I)config.GetValue("Settings", "WindowSizeSetting");
	}
}
