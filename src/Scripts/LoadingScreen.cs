using Godot;
using System;

public partial class LoadingScreen : CanvasLayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateText();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void UpdateText()
    {
        int loadingScreenAmount = GetTree().Root.GetNode<Settings>("Settings").LoadingScreens;
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        int randomLoadingScreen = rng.RandiRange(0, loadingScreenAmount - 1);
        GetNode<Label>("MainPanel/Title").Text = ResourceLoader.Load<LoadingScreenResource>("res://LoadingScreens/" + randomLoadingScreen + ".tres").Name;
        GetNode<Label>("MainPanel/Description").Text = ResourceLoader.Load<LoadingScreenResource>("res://LoadingScreens/" + randomLoadingScreen + ".tres").Description;
        GetNode<TextureRect>("MainPanel/LoadingImage").Texture = ResourceLoader.Load<LoadingScreenResource>("res://LoadingScreens/" + randomLoadingScreen + ".tres").Texture;
    }
}