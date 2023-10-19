using Godot;
using System;

public partial class Settings : Node
{
    public bool SdfgiSetting;
    public bool SsaoSetting;
    public bool SsilSetting;
    public bool SsrSetting;
    public bool GlowSetting;
    public bool FogSetting;
    public float MusicSetting;
    public float SoundSetting;
    public bool FullscreenSetting;
    public float MouseSensitivity; 
    public Vector2I WindowSizeSetting = new Vector2I((int)ProjectSettings.GetSetting("display/window/size/width"), (int)ProjectSettings.GetSetting("display/window/size/height"));
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (FileAccess.FileExists("user://version.txt"))
        {
            if (TxtParser.Load("user://version.txt") == Globals.milestone)
            {
                if (FileAccess.FileExists("user://settings.ini"))
                {
                    LoadIni();
                }
                else
                {
                    SaveSettings();
                }
            }
            else
            {
                TxtParser.Save("user://version.txt", Globals.milestone);
                SaveSettings();
            }
        }
        else
        {
            TxtParser.Save("user://version.txt", Globals.milestone);
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
            "SdfgiSetting",
            "SsaoSetting",
            "SsilSetting",
            "SsrSetting",
            "GlowSetting", 
            "FogSetting",
            "MusicSetting",
            "SoundSetting", 
            "FullscreenSetting",
            "MouseSensitivity",
            "WindowSizeSetting"
        }, new Godot.Collections.Array{
            true, //sdfgi
            true, //ssao
            true, //ssil
            false, //ssr
            true, //glow
            true, //fog
            1f, //music
            1f, //sound
            false, //fullscreen
            0.05f, //sensivity
            new Vector2(1920, 1080) //resolution
        }, "user://settings.ini");
        LoadIni();
    }

    public void LoadIni()
    {
        var config = new ConfigFile();

        // Load data from a file.
        Error err = config.Load("user://settings.ini");

        // If the file didn't load, ignore it.
        if (err != Error.Ok)
        {
            return;
        }
        // Fetch the data for each section.
        SdfgiSetting = (bool)config.GetValue("Settings", "SdfgiSetting");
        SsaoSetting = (bool)config.GetValue("Settings", "SsaoSetting");
        SsilSetting = (bool)config.GetValue("Settings", "SsilSetting");
        SsrSetting = (bool)config.GetValue("Settings", "SsrSetting");
        GlowSetting = (bool)config.GetValue("Settings", "GlowSetting");
        FogSetting = (bool)config.GetValue("Settings", "FogSetting");
        MusicSetting = (float)config.GetValue("Settings", "MusicSetting");
        SoundSetting = (float)config.GetValue("Settings", "SoundSetting");
        FullscreenSetting = (bool)config.GetValue("Settings", "FullscreenSetting");
        MouseSensitivity = (float)config.GetValue("Settings", "MouseSensitivity");
        WindowSizeSetting = (Vector2I)config.GetValue("Settings", "WindowSizeSetting");
    }
}
