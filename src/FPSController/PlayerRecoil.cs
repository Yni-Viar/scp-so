using Godot;
using System;
/// <summary>
/// PlayerRecoil.
/// Made by AceSpectre under MIT license.
/// </summary>
/// 

/*
MIT License

Copyright (c) 2024 AceSpectre

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
public partial class PlayerRecoil : Node3D
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	//Rotations
	Vector3 currentRotation;
	Vector3 targetRotation;
	//Recoil vectors.
	[Export] Vector3 recoil;
	[Export] Vector3 aimRecoil;
	//Settings
	[Export] float snappiness;
	[Export] float returnSpeed;
	/*
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}
	*/
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		targetRotation = targetRotation.Lerp(Vector3.Zero, returnSpeed * (float)delta);
		currentRotation = currentRotation.Lerp(targetRotation, snappiness * (float)delta);
		Rotation = currentRotation;

		//fix tilt.
		if (recoil.Z == 0 && aimRecoil.Z == 0)
		{
			//Note for Yni: this is solution for assigning global vectors in C#
			Vector3 fix = GlobalRotation;
			fix.Z = 0;
			GlobalRotation = fix;
		}
	}

	internal void Recoil(bool isAiming =false)
	{
		targetRotation += isAiming ? new Vector3(aimRecoil.X, rng.RandfRange(-aimRecoil.Y, aimRecoil.Y), rng.RandfRange(-aimRecoil.Z, aimRecoil.Z))
			: new Vector3(recoil.X, rng.RandfRange(-recoil.Y, recoil.Y), rng.RandfRange(-recoil.Z, recoil.Z));

    }

	void SetRecoil(Vector3 _recoil, Vector3 _aimRecoil, float _snappiness, float _returnSpeed)
	{
		recoil = _recoil;
		aimRecoil = _aimRecoil;
		snappiness = _snappiness;
		returnSpeed = _returnSpeed;
	}
}
