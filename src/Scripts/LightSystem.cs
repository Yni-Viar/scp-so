using Godot;
using System;
/// <summary>
/// System, that powers lights. Is available since 0.6.2.
/// </summary>
public partial class LightSystem : OmniLight3D
{
    bool anim = false;
    /// <summary>
    /// Power off light.
    /// </summary>
    internal async void TurnLightsOff()
    {
        LightEnergy = 4f;
        await ToSignal(GetTree().CreateTimer(0.2), "timeout");
        LightEnergy = 0f;
    }
    /// <summary>
    /// Power on light.
    /// </summary>
    internal void TurnLightsOn()
    {
        LightEnergy = 2f;
    }

    /*void LightFlicker()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        LightEnergy = rng.Randf();
    }*/ //for future
}
