using Godot;
using System;

public partial class Settings : Node
{
    public int LoadingScreens; //how much is loading screens.
    public bool SdfgiSetting;
    public bool SsaoSetting;
    public bool SsilSetting;
    public bool SsrSetting;
    public bool FogSetting;
    public float MusicSetting;
    public float SoundSetting;
    public bool FullscreenSetting;
    public float MouseSensitivity; 
    public Vector2I WindowSizeSetting = new Vector2I((int)ProjectSettings.GetSetting("display/window/size/width"), (int)ProjectSettings.GetSetting("display/window/size/height"));
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (FileAccess.FileExists("user://settings_" + Globals.settingsCompatibility + ".ini"))
        {
            LoadIni();
        }
        else
        {
            SaveSettings();
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SaveSettings()
    {
        IniSaver ini = new IniSaver();
        ini.SaveIni("Settings", new Godot.Collections.Array<string>{
            "LoadingScreens", 
            "SdfgiSetting",
            "SsaoSetting",
            "SsilSetting",
            "SsrSetting",
            "FogSetting",
            "MusicSetting",
            "SoundSetting", 
            "FullscreenSetting",
            "MouseSensitivity",
            "WindowSizeSetting"
        }, new Godot.Collections.Array{
            10, //how many default loading screens will be loaded.
            true, //sdfgi
            true, //ssao
            true, //ssil
            false, //ssr
            true, //fog
            1f, //music
            1f, //sound
            false, //fullscreen
            0.05f, //sensivity
            new Vector2(1920, 1080) //resolution
        }, "user://settings_" + Globals.settingsCompatibility + ".ini");
        LoadIni();
    }

    public void LoadIni()
    {
        var config = new ConfigFile();

        // Load data from a file.
        Error err = config.Load("user://settings_" + Globals.settingsCompatibility + ".ini");

        // If the file didn't load, ignore it.
        if (err != Error.Ok)
        {
            return;
        }
        // Fetch the data for each section.
        LoadingScreens = (int)config.GetValue("Settings", "LoadingScreens");
        SdfgiSetting = (bool)config.GetValue("Settings", "SdfgiSetting");
        SsaoSetting = (bool)config.GetValue("Settings", "SsaoSetting");
        SsilSetting = (bool)config.GetValue("Settings", "SsilSetting");
        SsrSetting = (bool)config.GetValue("Settings", "SsrSetting");
        FogSetting = (bool)config.GetValue("Settings", "FogSetting");
        MusicSetting = (float)config.GetValue("Settings", "MusicSetting");
        SoundSetting = (float)config.GetValue("Settings", "SoundSetting");
        FullscreenSetting = (bool)config.GetValue("Settings", "FullscreenSetting");
        MouseSensitivity = (float)config.GetValue("Settings", "MouseSensitivity");
        WindowSizeSetting = (Vector2I)config.GetValue("Settings", "WindowSizeSetting");

        DisplayServer.WindowSetSize(WindowSizeSetting);

        AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(SoundSetting));
        if (SoundSetting < 0.01)
        {
            AudioServer.SetBusMute(0, true);
        }
        else
        {
            AudioServer.SetBusMute(0, false);
        }

        AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb(MusicSetting));
        if (MusicSetting < 0.01)
        {
            AudioServer.SetBusMute(1, true);
        }
        else
        {
            AudioServer.SetBusMute(1, false);
        }

        DisplayServer.WindowSetMode(FullscreenSetting ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }
}
