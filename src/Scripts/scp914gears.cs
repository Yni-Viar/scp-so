using Godot;
using System;

public partial class scp914gears : AnimatableBody3D
{
    int currentModeCount = 2;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        scp914.currentMode = (scp914.Modes)currentModeCount;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    internal void Interact()
    {
        AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        AudioStreamPlayer3D sound = GetNode<AudioStreamPlayer3D>("LeverFlip");
        if (currentModeCount < 4)
        {
            currentModeCount++;
            switch (currentModeCount)
            {
                case 1:
                    animPlayer.Play("rough_coarse");
                    sound.Play();
                    break;
                case 2:
                    animPlayer.Play("coarse_11");
                    sound.Play();
                    break;
                case 3:
                    animPlayer.Play("11_fine");
                    sound.Play();
                    break;
                case 4:
                    animPlayer.Play("fine_veryfine");
                    sound.Play();
                    break;
            }
            scp914.currentMode = (scp914.Modes)currentModeCount;
        }
        else
        {
            currentModeCount = 0;
            animPlayer.Play("reset");
            scp914.currentMode = (scp914.Modes)currentModeCount;
        }
    }
}
