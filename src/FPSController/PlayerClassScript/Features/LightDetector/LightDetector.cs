using Godot;
using System;
/*
 * MIT License

Copyright (c) 2022 Reun Media Partnership

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/// <summary>
/// Light Detector is available since 0.8.0-dev (and in 0.7.0)
/// </summary>
public partial class LightDetector : Node3D
{
	SubViewport view;
	Camera3D cam;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsMultiplayerAuthority())
        {
            cam = GetNode<Camera3D>("SubViewport/DetectorCam");
		    view = GetNode<SubViewport>("SubViewport");
            view.DebugDraw = Viewport.DebugDrawEnum.Lighting;
        }
		else
        {
            GetNode<SubViewport>("SubViewport").QueueFree();
            Hide();
            SetProcess(false);
        }
    }

    public override void _Process(double delta)
    {
        if (GetTree().Root.GetNode<PlayerScript>("Main/Game/" + Multiplayer.GetUniqueId()).IsMultiplayerAuthority())
        {
            cam.GlobalPosition = GlobalPosition;
        }
    }
    /// <summary>
    /// Detect the lightness.
    /// </summary>
    /// <returns>Lightness of the environment</returns>
    internal float LightnessDetect()
	{
        Image texture = view.GetTexture().GetImage();
        texture.Resize(1, 1, Image.Interpolation.Lanczos);
        Color color = texture.GetPixel(0, 0);
		return color.Luminance;
    }
}
