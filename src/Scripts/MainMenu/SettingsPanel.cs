using Godot;
using System;

public partial class SettingsPanel : Panel
{
	public static bool HQSetting;
	public static bool MusicSetting;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (FileAccess.FileExists("user://settings_0.1.0dev.ini"))
		{
			LoadIni();
		}
		else
		{
			IniSaver ini = new IniSaver();
			ini.SaveIni("Settings", new Godot.Collections.Array<string>{
				"HQSetting",
				"MusicSetting"
			}, new Godot.Collections.Array{
				true,
				true
			}, "user://settings_0.1.0dev.ini");

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
			"MusicSetting"
		}, new Godot.Collections.Array{
			HQSetting,
			MusicSetting
		}, "user://settings_0.1.0dev.ini");

		LoadIni();
	}

    public static void LoadIni()
	{
		var config = new ConfigFile();

		// Load data from a file.
		Error err = config.Load("user://settings_0.1.0dev.ini");

		// If the file didn't load, ignore it.
		if (err != Error.Ok)
		{
			return;
		}
		// Fetch the data for each section.
		SettingsPanel.HQSetting = (bool)config.GetValue("Settings", "HQSetting");
		SettingsPanel.MusicSetting = (bool)config.GetValue("Settings", "MusicSetting");
	}
}

