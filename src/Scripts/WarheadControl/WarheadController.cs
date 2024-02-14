using Godot;
using System;
/// <summary>
/// Alpha and Omega warheads controller.
/// </summary>
public partial class WarheadController : Node
{
	[Export] bool omegaWarhead = false;
	[Export] internal bool detonationProgress = false;
	[Export] bool detonated = false;
	[Export] float secondsLeft = 90f;
	[Export] string countdownAudioPath;
	[Export] string boomSoundPath;
	[Export] bool enableWarheadLights;

    public override void _Ready()
    {
		enableWarheadLights = GetTree().Root.GetNode<Settings>("Settings").EnableWarheadLights;
    }
    /// <summary>
    /// Starts the warhead countdown.
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    internal void Countdown()
	{
		if (enableWarheadLights)
		{
            foreach (Node node in GetTree().GetNodesInGroup("Lamp"))
            {
				if (node is LightSystem light)
				{
					light.LightColor = new Color(1, 0, 0);
				}
            }
        }
		detonationProgress = true;
		GetParent().GetNode<PlayerScript>(Multiplayer.GetUniqueId().ToString()).customMusic = true;
		SetWarheadMusic(countdownAudioPath, secondsLeft);
		GetNode<Timer>("WarheadCountdown").WaitTime = secondsLeft;
		GetNode<Timer>("WarheadCountdown").Start();

    }
	/// <summary>
	/// Stops the warhead countdown (if has not detonated)
	/// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    void Cancel()
	{
        if (enableWarheadLights)
        {
            foreach (Node node in GetTree().GetNodesInGroup("Lamp"))
            {
                if (node is LightSystem light)
                {
                    light.LightColor = new Color(1, 1, 1);
                }
            }
        }
        if (detonationProgress && !detonated)
		{
			secondsLeft = (float)GetNode<Timer>("WarheadCountdown").TimeLeft;
			detonationProgress = false;
            GetParent().GetNode<PlayerScript>(Multiplayer.GetUniqueId().ToString()).customMusic = false;
            GetParent().GetNode<AudioStreamPlayer>("BackgroundMusic").Stop();
            GetNode<Timer>("WarheadCountdown").Stop();

            if (secondsLeft >= 30.0f && secondsLeft < 70f)
			{
                secondsLeft += 10f;
			}
			else if (secondsLeft < 30.0f)
			{
				secondsLeft = 30.0f;
            }
            else
			{
                secondsLeft = 90.0f;
            }
        }
	}
	/// <summary>
	/// Detonate!
	/// </summary>
	private void OnWarheadCountdownTimeout()
	{
		if (Multiplayer.IsServer())
		{
			foreach (string item in GetParent<FacilityManager>().playersList)
			{
				if (omegaWarhead ? GetParent().GetNode<PlayerScript>(item).GlobalPosition.Y < 0 : GetParent().GetNode<PlayerScript>(item).GlobalPosition.Y >= 0)
				{
					GetParent<FacilityManager>().Rpc("SetPlayerClass", item, 0, "Detonated by Alpha Warhead");
				}
			}
		}
		GetParent().GetNode<AudioStreamPlayer>("BackgroundMusic").Stop();
		GetParent().GetNode<AudioStreamPlayer>("BackgroundMusic").Stream = ResourceLoader.Load<AudioStream>(boomSoundPath);
		GetParent().GetNode<AudioStreamPlayer>("BackgroundMusic").Play();
		detonated = true;
	}
	/// <summary>
	/// Sets music.
	/// </summary>
	/// <param name="set">Audio source</param>
	/// <param name="time">Time to start</param>
	void SetWarheadMusic(string set, float time)
	{
		AudioStreamPlayer sfx = GetParent().GetNode<AudioStreamPlayer>("BackgroundMusic");
		AudioStream audio = ResourceLoader.Load<AudioStream>(set);
		sfx.Stream = audio;
		sfx.Play(90.0f - time);
	}
}
