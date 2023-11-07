using Godot;
using System;

public partial class MainMenu : Control
{
    bool playInfoToggle = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Settings settings = GetTree().Root.GetNode<Settings>("Settings");
        if (Globals.currentStage == Globals.Stages.release)
        {
            GetNode<TextureRect>("Background").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Menu/MainMenuBackground.png");
        }
        else if (Globals.currentStage == Globals.Stages.testing)
        {
            GetNode<TextureRect>("Background").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Menu/MainMenuBackgroundTesting.png");
        }
        else
        {
            GetNode<TextureRect>("Background").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Menu/MainMenuBackgroundIndev.png");
        }

        GetWindow().Size = settings.WindowSizeSetting;

        AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(settings.SoundSetting));
        if (settings.SoundSetting < 0.01)
        {
            AudioServer.SetBusMute(0, true);
        }
        else
        {
            AudioServer.SetBusMute(0, false);
        }

        AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb(settings.MusicSetting));
        if (settings.MusicSetting < 0.01)
        {
            AudioServer.SetBusMute(1, true);
        }
        else
        {
            AudioServer.SetBusMute(1, false);
        }

        DisplayServer.WindowSetMode(settings.FullscreenSetting ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnPlayPressed()
    {
        playInfoToggle = !playInfoToggle;
        if (playInfoToggle)
        {
            GetNode<Panel>("PlayPanel").Show();
            GetNode<Panel>("MainPanel").Hide();
        }
        else
        {
            GetNode<Panel>("PlayPanel").Hide();
            GetNode<Panel>("MainPanel").Show();
        }
    }


    private void OnExitPressed()
    {
        GetTree().Quit();
    }


    private void OnCreditsPressed()
    {
        GetNode<Panel>("CreditsPanel").Show();
    }

    private void OnSettingsPressed()
    {
        GetNode<CanvasLayer>("Settings").Show();
    }

    private void OnPlayerNameTextChanged(string new_text)
    {
        TxtParser.Save("user://playername.txt", new_text);
    }
}