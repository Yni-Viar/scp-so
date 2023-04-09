using Godot;
using System;

public partial class CreditsPanel : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnBackPressed()
    {
        this.Hide();
    }


    private void OnMapGenPressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("MapGenCredits").Show();
    }


    private void OnFpsPressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("FPSCredits").Show();
    }


    private void OnOtherCodePressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("OtherCodeCredits").Show();
    }


    private void OnModelsPressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("ModelsCredits").Show();
    }


    private void OnSoundsPressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("SoundsCredits").Show();
    }


    private void OnOtherArtPressed()
    {
        GetNode<Control>("CreditsMenu").Hide();
        GetNode<Panel>("OtherArtCredits").Show();
    }
}


