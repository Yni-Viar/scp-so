using Godot;
using System;

public partial class SettingsPanel : Panel
{
    Settings settings;

    public static bool HQSetting;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        settings = GetTree().Root.GetNode<Settings>("Settings");

        GetNode<CheckButton>("ScrollContainer/VBoxContainer/SDFGISet").ButtonPressed = settings.SdfgiSetting;
        GetNode<CheckButton>("ScrollContainer/VBoxContainer/SSAOSet").ButtonPressed = settings.SsaoSetting;
        GetNode<CheckButton>("ScrollContainer/VBoxContainer/SSILSet").ButtonPressed = settings.SsilSetting;
        GetNode<CheckButton>("ScrollContainer/VBoxContainer/SSRSet").ButtonPressed = settings.SsrSetting;
        GetNode<CheckButton>("ScrollContainer/VBoxContainer/FogSet").ButtonPressed = settings.FogSetting;
        GetNode<CheckButton>("ScrollContainer/VBoxContainer/FullscreenSet").ButtonPressed = settings.FullscreenSetting;
        GetNode<HSlider>("ScrollContainer/VBoxContainer/MouseSensSet").Value = settings.MouseSensitivity;
        GetNode<HSlider>("ScrollContainer/VBoxContainer/MusicSet").Value = settings.MusicSetting;
        GetNode<HSlider>("ScrollContainer/VBoxContainer/SoundSet").Value = settings.SoundSetting;


        if (settings.FullscreenSetting)
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
        GetParent().GetParent().GetNode<CanvasLayer>("Settings").Hide();
    }

    private void OnMusicSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.MusicSetting = 1f;
        }
        else
        {
            settings.MusicSetting = 0f;
        }
    }
    private void OnSavePressed()
    {
        SaveCurrentSettings();
    }

    private void SaveCurrentSettings()
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
            10, 
            settings.SdfgiSetting,
            settings.SsaoSetting,
            settings.SsilSetting,
            settings.SsrSetting,
            settings.FogSetting,
            settings.MusicSetting,
            settings.SoundSetting,
            settings.FullscreenSetting,
            settings.MouseSensitivity, 
            settings.WindowSizeSetting
        }, "user://settings_" + Globals.settingsCompatibility + ".ini");
        settings.LoadIni();
    }

    
    private void OnFullscreenSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            settings.FullscreenSetting = true;
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            settings.FullscreenSetting = false;
            
        }
        SaveCurrentSettings();
    }
    
    private void OnWindowSizeSetItemSelected(long index)
    {
        switch (index)
        {
            case 0:
                settings.WindowSizeSetting = new Vector2I(1920, 1080);
                break;
            case 1:
                settings.WindowSizeSetting = new Vector2I(1600, 900);
                break;
            case 2:
                settings.WindowSizeSetting = new Vector2I(1366, 768);
                break;
            case 3:
                settings.WindowSizeSetting = new Vector2I(1280, 720);
                break;
            case 4:
                settings.WindowSizeSetting = new Vector2I(1024, 768);
                break;
            case 5:
                settings.WindowSizeSetting = new Vector2I(800, 600);
                break;
        }
        DisplayServer.WindowSetSize(settings.WindowSizeSetting);
        SaveCurrentSettings();
    }

    private void OnSdfgiSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.SdfgiSetting = true;
        }
        else
        {
            settings.SdfgiSetting = false;
        }
        SaveCurrentSettings();
    }

    private void OnSsaoSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.SsaoSetting = true;
        }
        else
        {
            settings.SsaoSetting = false;
        }
        SaveCurrentSettings();
    }

    private void OnSsilSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.SsilSetting = true;
        }
        else
        {
            settings.SsilSetting = false;
        }
        SaveCurrentSettings();
    }

    private void OnSsrSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.SsilSetting = true;
        }
        else
        {
            settings.SsilSetting = false;
        }
        SaveCurrentSettings();
    }

    private void OnFogSetToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            settings.FogSetting = true;
        }
        else
        {
            settings.FogSetting = false;
        }
        SaveCurrentSettings();
    }

    private void OnMouseSensSetDragEnded(bool value_changed)
    {
        if (value_changed)
        {
            settings.MouseSensitivity = (float)GetNode<HSlider>("ScrollContainer/VBoxContainer/MouseSensSet").Value;
            SaveCurrentSettings();
        }
    }

    private void OnMusicSetDragEnded(bool value_changed)
    {
        if (value_changed)
        {
            settings.MusicSetting = (float)GetNode<HSlider>("ScrollContainer/VBoxContainer/MusicSet").Value;
            AudioSetting(1, settings.MusicSetting);
            SaveCurrentSettings();
        }
    }


    private void OnSoundSetDragEnded(bool value_changed)
    {
        if (value_changed)
        {
            settings.SoundSetting = (float)GetNode<HSlider>("ScrollContainer/VBoxContainer/SoundSet").Value;
            AudioSetting(0, settings.SoundSetting);
            SaveCurrentSettings();
            AudioStreamPlayer test = GetNode<AudioStreamPlayer>("SoundTest");
            test.Play();
        }
    }

    /// <summary>
    /// Sets the audio bus.
    /// </summary>
    /// <param name="bus">Bus index</param>
    /// <param name="v">Value</param>
    internal void AudioSetting(int bus, float v)
    {
        AudioServer.SetBusVolumeDb(bus, Mathf.LinearToDb(v));
        if (v < 0.01)
        {
            AudioServer.SetBusMute(bus, true);
        }
        else
        {
            AudioServer.SetBusMute(bus, false);
        }
    }
}
