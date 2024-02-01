using Godot;
using System;

public partial class Settings : Node
{
    internal string[] availableLanguages = new string[] { "en", "ru" };
    public int LoadingScreens = 11; //how much is loading screens.
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
    public string LanguageSetting = "en";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        LoadIni(true);
    }
    /*
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    */
    public void SaveDefaultSettings()
    {
        IniSaver ini = new IniSaver();
        ini.SaveIni("Settings", new Godot.Collections.Array<string>{
            "SdfgiSetting",
            "SsaoSetting",
            "SsilSetting",
            "SsrSetting",
            "FogSetting",
            "MusicSetting",
            "SoundSetting", 
            "FullscreenSetting",
            "MouseSensitivity",
            "WindowSizeSetting",
            "LanguageSetting"
        }, new Godot.Collections.Array{
            true, //sdfgi
            true, //ssao
            true, //ssil
            false, //ssr
            true, //fog
            1f, //music
            1f, //sound
            false, //fullscreen
            0.05f, //sensivity
            new Vector2(1920, 1080), //resolution
            availableLanguages[0] //language
        }, "user://settings.ini");
        LoadIni();
    }

    public void LoadIni(bool checkForMissing = false)
    {
        var config = new ConfigFile();

        // Load data from a file.
        Error err = config.Load("user://settings.ini");

        // If the file didn't load, ignore it.
        if (err != Error.Ok)
        {
            SaveDefaultSettings();
            GD.Print("The settings has not saved, saving default settings...");
            return;
        }
        if (checkForMissing)
        {
            CheckIfASettingExists(config, new Godot.Collections.Array<string>{
                "SdfgiSetting",
                "SsaoSetting",
                "SsilSetting",
                "SsrSetting",
                "FogSetting",
                "MusicSetting",
                "SoundSetting",
                "FullscreenSetting",
                "MouseSensitivity",
                "WindowSizeSetting",
                "LanguageSetting"
                }, new Godot.Collections.Array{
                true, //sdfgi
                true, //ssao
                true, //ssil
                false, //ssr
                true, //fog
                1f, //music
                1f, //sound
                false, //fullscreen
                0.05f, //sensivity
                new Vector2(1920, 1080), //resolution
                availableLanguages[0] //language
                });
        }


        // Fetch the data for each section.
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
    }

    void CheckIfASettingExists(ConfigFile config, Godot.Collections.Array<string> settingValues, Godot.Collections.Array defaultValues)
    {
        for (int i = 0;i < settingValues.Count;i++)
        {
            if (!config.HasSectionKey("Settings", settingValues[i]))
            {
                config.SetValue("Setting", settingValues[i], defaultValues[i]);
            }
        }
    }
}
