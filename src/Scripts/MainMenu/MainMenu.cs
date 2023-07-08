using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/LoadingScreen.tscn");
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
		GetNode<Control>("Settings").Show();
	}
}